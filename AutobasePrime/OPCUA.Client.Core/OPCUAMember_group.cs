using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    public sealed class OPCUAMember_group
    {
        public OPCUAMember_server Server { get; }
        public string sName { get; private set; }
        public int nInterval { get; private set; }

        public Subscription sbSubscription { get; private set; }
        public List<OPCUAMember_item> arrItem { get; }


        public bool IsDirty { get; private set; }

        internal void MarkDirty()
        {
            IsDirty = true;
        }


        public OPCUAMember_group(
            OPCUAMember_server server,
            string name,
            int publishingInterval)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            sName = name;
            nInterval = publishingInterval;
            arrItem = new List<OPCUAMember_item>();
        }

        // =========================
        // Item 관리 (순수 구조)
        // =========================

        public void AddItem(OPCUAMember_item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.SetGroup(this);
            arrItem.Add(item);

            MarkDirty();
        }

        public void RemoveItem(OPCUAMember_item item)
        {
            if (item == null)
                return;

            if (arrItem.Remove(item))
            {
                item.ClearGroup();
                MarkDirty();
            }
        }

        //  설정 변경용 메서드
        public void Update(string name, int interval)
        {
            bool intervalChanged = nInterval != interval;

            sName = name;
            nInterval = interval;

            if (intervalChanged)
            {
                MarkDirty();
                Server?.OnGroupIntervalChanged(this);
            }
        }

        internal void ClearDirty()
        {
            IsDirty = false;
        }


        // =========================
        // Subscription 제어 (명시적)
        // =========================

        /// <summary>
        /// Host/Runtime에서 호출
        /// </summary>
        public async Task CreateSubscriptionAsync(
              ISession session,
              CancellationToken ct)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            ct.ThrowIfCancellationRequested();

            // 기존 Subscription 제거
            await ClearSubscriptionAsync().ConfigureAwait(false);

            if (arrItem.Count == 0)
                return;

            var sub = new Subscription(session.DefaultSubscription)
            {
                PublishingInterval = nInterval
            };

            foreach (var item in arrItem)
            {
                ct.ThrowIfCancellationRequested();

                var mi = new MonitoredItem(sub.DefaultItem)
                {
                    StartNodeId = new NodeId(item.sNode),
                    AttributeId = Attributes.Value,
                    DisplayName = item.sName,

                    SamplingInterval = -1,
                    QueueSize = 1,
                    DiscardOldest = true,

                    Handle = item              // ⭐ 핵심
                };

                mi.Notification += OnNotification;

                item.moItem = mi;
                sub.AddItem(mi);
            }

            session.AddSubscription(sub);

            try
            {
                await sub.CreateAsync().ConfigureAwait(false);
                sbSubscription = sub; // 성공 후에만 할당
                ClearDirty();
            }
            catch
            {
                try
                {
                    await sub.DeleteAsync(true).ConfigureAwait(false);
                    sub.Dispose();
                }
                catch { }

                throw;
            }
        }
        public async Task ClearSubscriptionAsync()
        {
            var sub = sbSubscription;
            sbSubscription = null;

            if (sub == null)
                return;

            try { await sub.DeleteItemsAsync().ConfigureAwait(false); } catch { }
            try { await sub.DeleteAsync(true).ConfigureAwait(false); } catch { }
            try { sub.Dispose(); } catch { }
        }

        // =========================
        // Notification (캐시만)
        // =========================

        private void OnNotification(
           MonitoredItem monitoredItem,
           MonitoredItemNotificationEventArgs args)
        {
            try
            {
                var n = args.NotificationValue as MonitoredItemNotification;
                if (n == null) return;

                var item = monitoredItem.Handle as OPCUAMember_item;
                if (item == null) return;

                item.UpdateFromNotification(n.Value);
            }
            catch
            {
            }
        }

        public OPCUAMember_item FindItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return null;

            var items = arrItem;
            if (items == null)
                return null;

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                if (it != null &&
                    string.Equals(it.sName, itemName, StringComparison.OrdinalIgnoreCase))
                {
                    return it;
                }
            }

            return null;
        }

        /// <summary>
        /// 초기값 읽기
        /// 서버 최초 연결, 그룹 최초 생성, 아이템 추가 시 실행.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task ReadOnceAsync(
            ISession session,
            CancellationToken ct)
        {
            if (session == null || !session.Connected)
                return;

            if (arrItem.Count == 0)
                return;

            try
            {
                var nodesToRead = new ReadValueIdCollection();

                foreach (var item in arrItem)
                {
                    ct.ThrowIfCancellationRequested();

                    nodesToRead.Add(new ReadValueId
                    {
                        NodeId = new NodeId(item.sNode),
                        AttributeId = Attributes.Value
                    });
                }

                var response = await session.ReadAsync(
                     null,
                     0,
                     TimestampsToReturn.Source,
                     nodesToRead,
                     ct
                 ).ConfigureAwait(false);

                var map = arrItem.ToDictionary(
                       i => new NodeId(i.sNode),
                       i => i
                   );

                int count = Math.Min(response.Results.Count, nodesToRead.Count);

                for (int i = 0; i < count; i++)
                {
                    var nid = nodesToRead[i].NodeId;

                    if (map.TryGetValue(nid, out var item))
                    {
                        item.UpdateFromNotification(response.Results[i]);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        //그룹 최초 생성 (구독 + 초기값)
        public async Task InitializeAsync(
            ISession session,
            CancellationToken ct)
        {
            await CreateSubscriptionAsync(session, ct).ConfigureAwait(false);
            await ReadOnceAsync(session, ct).ConfigureAwait(false);
            ClearDirty();
        }

        //재연결 전용 (구독만 재생성)
        public async Task RecreateSubscriptionAsync(
            ISession session,
            CancellationToken ct)
        {
            await CreateSubscriptionAsync(session, ct).ConfigureAwait(false);
            ClearDirty();
        }


        public async Task AddItemAsync(
    OPCUAMember_item item,
    ISession session,
    CancellationToken ct)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (sbSubscription == null || session == null || !session.Connected)
                return;

            try
            {
                ct.ThrowIfCancellationRequested();

                var mi = new MonitoredItem(sbSubscription.DefaultItem)
                {
                    StartNodeId = new NodeId(item.sNode),
                    AttributeId = Attributes.Value,
                    DisplayName = item.sName,

                    SamplingInterval = -1,
                    QueueSize = 1,
                    DiscardOldest = true,

                    Handle = item
                };

                mi.Notification += OnNotification;

                item.moItem = mi;

                sbSubscription.AddItem(mi);
                await sbSubscription.ApplyChangesAsync().ConfigureAwait(false);

                //  신규 아이템만 초기값 Read
                await ReadItemOnceAsync(item, session, ct).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task ReadItemOnceAsync(
            OPCUAMember_item item,
            ISession session,
            CancellationToken ct)
        {
            if (item == null || session == null || !session.Connected)
                return;

            ct.ThrowIfCancellationRequested();

            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId
                {
                    NodeId = new NodeId(item.sNode),
                    AttributeId = Attributes.Value
                }
            };

            var response = await session.ReadAsync(
                null,
                0,
                TimestampsToReturn.Source,
                nodesToRead,
                ct
            ).ConfigureAwait(false);

            if (response.Results.Count > 0)
            {
                item.UpdateFromNotification(response.Results[0]);
            }
        }

        //아이템 증분 삭제
        public async Task RemoveItemAsync(
            OPCUAMember_item item,
            CancellationToken ct)
        {
            if (item == null)
                return;

            ct.ThrowIfCancellationRequested();

            if (sbSubscription != null && item.moItem != null)
            {
                try
                {
                    sbSubscription.RemoveItem(item.moItem);
                    await sbSubscription.ApplyChangesAsync().ConfigureAwait(false);
                }
                catch
                {
                    // ignore
                }
            }

            item.moItem = null;
            RemoveItem(item);
        }

        /// <summary>
        /// 이 그룹의 Subscription을 완전히 삭제한다.
        /// - 그룹 삭제
        /// - 서버 Uninit
        /// - Runtime 정리 시 사용
        /// </summary>
        public async Task DeleteSubscriptionAsync()
        {
            var sub = sbSubscription;
            sbSubscription = null;

            if (sub == null)
                return;

            try
            {
                // 1️⃣ MonitoredItem 제거 (best-effort)
                try
                {
                    if (sub.MonitoredItemCount > 0)
                        await sub.DeleteItemsAsync().ConfigureAwait(false);
                }
                catch
                {
                    // ignore (best-effort)
                }

                // 2️⃣ Subscription 삭제 (서버 반영)
                try
                {
                    await sub.DeleteAsync(true).ConfigureAwait(false);
                }
                catch
                {
                    // ignore (best-effort)
                }
            }
            finally
            {
                // 3️⃣ 로컬 리소스 정리
                try
                {
                    foreach (var item in arrItem)
                    {
                        if (item != null)
                            item.moItem = null;
                    }
                }
                catch { }

                try { sub.Dispose(); } catch { }
            }
        }
    }
}