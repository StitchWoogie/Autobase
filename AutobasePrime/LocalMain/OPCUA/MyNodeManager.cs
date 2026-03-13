using AutoLibLocal;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LocalMain.OPCUA;

namespace LocalMain
{
    public class MyNodeManager : CustomNodeManager2
    {
        private ITelemetryContext _telemetry;
        private ILogger _logger;

        private FolderState _root;
        public List<AutobaseNode> _listNode;

        public class AutobaseNode
        {
            public string sTagName = "";
            public int[] nTagPos = new int[1];
            public EnumTagType nTagType = EnumTagType.none;
            public BaseDataVariableState bdItemNode;

            public AutobaseNode()
            {
            }
        }

        public MyNodeManager(IServerInternal server, ApplicationConfiguration config)
            : base(server, config, "urn:autobase.opcua:NodeManager")
        {
            _telemetry = server.Telemetry;
            _logger = _telemetry.CreateLogger<MyNodeManager>();
        }

        // Called when the server starts to create the address space
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            return new NodeStateCollection();
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            _listNode = new List<AutobaseNode>();
            // Root folder for our custom nodes
            _root = CreateFolder(
                null,
                "Autobase",
                "Autobase",
                externalReferences);

            if (AutoLibLocal.TagLib.groupRoot.arrayTag != null && AutoLibLocal.TagLib.tagListAll.Length > 0)
            {

                RecurseGroup(_root, externalReferences, "");
            }

            AddPredefinedNode(SystemContext, _root);

            OPCUAServerMain.bLoad = true;
            OpcUaServerLogBridge.Info("Server", string.Format("Address space created. Nodes: {0}", _listNode.Count));
        }

        private void RecurseGroup(FolderState _fs, IDictionary<NodeId, IList<IReference>> externalReferences, string groupname)
        {
            TagGrClass group = TagLib.GetGroupArray(groupname);

            for (int i = 0; i < group.arrayTag.Count; i++)
            {

                AutobaseNode tempnode = new AutobaseNode();

                TagPublicClass tp = (TagPublicClass)(group.arrayTag[i]);

                tempnode.sTagName = tp.tag;

                if (tp.enumTagType == EnumTagType.GR)
                {
                    FolderState _folder = CreateFolder(
                    _fs,
                    tp.tag,
                    tp.name,
                    tp.description,
                    externalReferences);

                    RecurseGroup(_folder, externalReferences, tempnode.sTagName);
                }
                else
                {
                    TagLib.GetTagTypeAndPos(tempnode.sTagName, ref tempnode.nTagType, ref tempnode.nTagPos);

                    if (tempnode.nTagType == EnumTagType.AI)
                    {
                        TagAiClass ai = TagLib.GetStructAI(tempnode.sTagName, ref tempnode.nTagPos);

                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Double,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = ai.curr;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            AutoLib.TagWrite.WriteCurrAI(tp.tag, ai, (double)value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    OpcUaServerLogBridge.Error("TagWrite", string.Format("WriteCurrAI error: {0}, {1}", tp.tag, t.Exception.InnerException?.Message ?? t.Exception.Message));
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);

                    }
                    else if (tempnode.nTagType == EnumTagType.AO)
                    {
                        TagAoClass ao = TagLib.GetStructAO(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Double,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = ao.curr;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {

                            AutoLib.TagWrite.WriteCurrAO(tp.tag, ao, (double)value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    OpcUaServerLogBridge.Error("TagWrite", string.Format("WriteCurrAO error: {0}, {1}", tp.tag, t.Exception.InnerException?.Message ?? t.Exception.Message));
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.DI)
                    {
                        TagDiClass di = TagLib.GetStructDI(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Boolean,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = di.curr == 0 ? false : true;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            sbyte _value = (bool)value ? (sbyte)1 : (sbyte)0;

                            AutoLib.TagWrite.WriteCurrDI(tp.tag, di, _value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    OpcUaServerLogBridge.Error("TagWrite", string.Format("WriteCurrDI error: {0}, {1}", tp.tag, t.Exception.InnerException?.Message ?? t.Exception.Message));
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.DO)
                    {
                        TagDoClass d_o = TagLib.GetStructDO(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Boolean,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = d_o.curr == 0 ? false : true;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            sbyte _value = (bool)value ? (sbyte)1 : (sbyte)0;

                            AutoLib.TagWrite.WriteCurrDO(tp.tag, d_o, _value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    OpcUaServerLogBridge.Error("TagWrite", string.Format("WriteCurrDO error: {0}, {1}", tp.tag, t.Exception.InnerException?.Message ?? t.Exception.Message));
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.GDO)
                    {
                        TagDoGroupClass GDO = TagLib.GetStructDoGroup(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Boolean,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = GDO.curr == 0 ? false : true;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            sbyte _value = (bool)value ? (sbyte)1 : (sbyte)0;

                            AutoLib.TagWrite.WriteCurrDoGroup(GDO, _value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    OpcUaServerLogBridge.Error("TagWrite", string.Format("WriteCurrGDO error: {0}, {1}", tp.tag, t.Exception.InnerException?.Message ?? t.Exception.Message));
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.ST)
                    {
                        TagStClass ST = TagLib.GetStructST(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.String,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = ST.curr;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {

                            AutoLib.TagWrite.WriteCurrST(tp.tag, ST, (string)value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    OpcUaServerLogBridge.Error("TagWrite", string.Format("WriteCurrST error: {0}, {1}", tp.tag, t.Exception.InnerException?.Message ?? t.Exception.Message));
                            });

                            return ServiceResult.Good;
                        };


                        _listNode.Add(tempnode);
                    }
                    else
                    {

                    }
                }
            }
        }

        // Helper to create folder
        private FolderState CreateFolder(NodeState parent, string name, string displayName, IDictionary<NodeId, IList<IReference>> externalRefs)
        {
            string buf = "";

            var folder = new FolderState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(buf = (parent == null) ? name : string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                DisplayName = displayName,
                EventNotifier = EventNotifiers.None
            };

            if (parent == null)
            {
                IList<IReference> refs;
                if (!externalRefs.TryGetValue(ObjectIds.ObjectsFolder, out refs))
                {
                    refs = new List<IReference>();
                    externalRefs[ObjectIds.ObjectsFolder] = refs;
                }
                refs.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));
                folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            }
            else
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        private FolderState CreateFolder(NodeState parent, string name, string displayName, string description, IDictionary<NodeId, IList<IReference>> externalRefs)
        {
            string buf = "";

            var folder = new FolderState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(buf = (parent == null) ? name : string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                DisplayName = displayName,
                Description = description,
                EventNotifier = EventNotifiers.None
            };

            if (parent == null)
            {
                IList<IReference> refs;
                if (!externalRefs.TryGetValue(ObjectIds.ObjectsFolder, out refs))
                {
                    refs = new List<IReference>();
                    externalRefs[ObjectIds.ObjectsFolder] = refs;
                }
                refs.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));
                folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            }
            else
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        // Helper to create variable
        private BaseDataVariableState CreateVariable(NodeState parent, string name, string displayName, string description, NodeId dataType, int valueRank, uint opcflag)
        {
            byte flag = AccessLevels.CurrentReadOrWrite;
            bool bHide = false;

            if (OPCUAServerMain.bItemSecurity)
            {
                if ((opcflag & 1) == 0)
                {
                    flag = AccessLevels.None;
                    bHide = true;
                }
                else
                {
                    if (opcflag == 7) flag = AccessLevels.CurrentReadOrWrite;
                    else
                    {
                        flag = AccessLevels.None;

                        if ((opcflag & 2) != 0) flag = AccessLevels.CurrentRead;

                        if ((opcflag & 4) != 0) flag = AccessLevels.CurrentWrite;
                    }
                }
            }

            if (bHide)
            {
                var variable = new BaseDataVariableState(parent)
                {
                    SymbolicName = name,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                    NodeId = new NodeId(string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                    BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                    DisplayName = displayName,
                    Description = description,
                    DataType = dataType,
                    ValueRank = valueRank,
                    AccessLevel = flag,
                    UserAccessLevel = flag,
                    RolePermissions = new RolePermissionType[]
                    {
                    new RolePermissionType
                    {
                        RoleId = Objects.WellKnownRole_Anonymous,
                        Permissions = 0
                    }
                    },
                    Historizing = false
                };

                parent?.AddChild(variable);
                return variable;

               // return null; //  노드 자체를 만들지 않음
            }
            else
            {
                var variable = new BaseDataVariableState(parent)
                {
                    SymbolicName = name,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                    NodeId = new NodeId(string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                    BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                    DisplayName = displayName,
                    Description = description,
                    DataType = dataType,
                    ValueRank = valueRank,
                    AccessLevel = flag,
                    UserAccessLevel = flag,
                    Historizing = false
                };

                parent?.AddChild(variable);
                return variable;
            }
        }

        private BaseDataVariableState CreateVariable(NodeState parent, string name, string displayName, NodeId dataType, int valueRank, uint opcflag)
        {

            byte flag = AccessLevels.CurrentReadOrWrite;
            if (OPCUAServerMain.bItemSecurity)
            {
                if (opcflag % 1 == 0)
                {
                    flag = AccessLevels.None;
                }
                else
                {
                    if (opcflag == 7) flag = AccessLevels.CurrentReadOrWrite;
                    else
                    {
                        if (opcflag % 2 != 0) flag = AccessLevels.CurrentRead;

                        if (opcflag % 4 != 0) flag = AccessLevels.CurrentWrite;
                    }
                }
            }

            var variable = new BaseDataVariableState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                DisplayName = displayName,
                DataType = dataType,
                ValueRank = valueRank,
                AccessLevel = flag,
                UserAccessLevel = flag,
                Historizing = false
            };

            parent?.AddChild(variable);
            return variable;
        }

        // Example: method to update the variable
        public void UpdateItem(int index, double newValue, string tag)
        {
            if (!OPCUAServerMain.bEnabled) return;
            if (index < 0 || index >= _listNode.Count) return; // 범위 체크

            try
            {
                AutobaseNode abn = this._listNode[index];

                if (abn.sTagName != tag)
                {
                    _logger.LogWarning(
                        "Tag mismatch. Expected={Expected}, Got={Got}",
                        tag, abn.sTagName);
                    return;
                }

                BaseDataVariableState itemNode = abn.bdItemNode;

                if ((double)itemNode.Value == newValue) return;

                // 값 비교 시 부동소수점 허용오차 고려
                if (Math.Abs((double)itemNode.Value - newValue) < double.Epsilon)
                    return;

                lock (Lock) // SystemContext 접근 시 락 필요
                {
                    itemNode.Value = newValue;
                    itemNode.Timestamp = DateTime.UtcNow;
                    itemNode.StatusCode = StatusCodes.Good;
                    itemNode.ClearChangeMasks(SystemContext, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "UpdateItem failed. Index={Index}, Tag={Tag}",
                    index, tag);
            }
        }

        public void UpdateItem(int index, sbyte newValue, string tag)
        {
            if (!OPCUAServerMain.bEnabled) return;
            if (index < 0 || index >= _listNode.Count) return; // 범위 체크

            try
            {
                bool _value = newValue == 0 ? false : true;
                AutobaseNode abn = this._listNode[index];

                if (abn.sTagName != tag)
                {
                    _logger.LogWarning(
                        "Tag mismatch. Expected={Expected}, Got={Got}",
                        tag, abn.sTagName);
                    OpcUaServerLogBridge.Warn("TagUpdate", string.Format("Tag mismatch. Expected={0}, Got={1}", tag, abn.sTagName));
                    return;
                }

                BaseDataVariableState itemNode = abn.bdItemNode;
                if ((bool)itemNode.Value == _value) return;

                lock (Lock) // SystemContext 접근 시 락 필요
                {
                    itemNode.Value = _value;
                    itemNode.Timestamp = DateTime.UtcNow;
                    itemNode.StatusCode = StatusCodes.Good;
                    itemNode.ClearChangeMasks(SystemContext, false);
                }
            }
            catch (Exception ex)
            {
                OpcUaServerLogBridge.Error("TagUpdate", string.Format("UpdateItem failed. Tag={0}, {1}", tag, ex.Message));
            }
        }

        public void UpdateItem(int index, string newValue, string tag)
        {
            if (!OPCUAServerMain.bEnabled) return;
            if (index < 0 || index >= _listNode.Count) return; // 범위 체크

            try
            {

                AutobaseNode abn = this._listNode[index];

                if (abn.sTagName != tag)
                {
                    _logger.LogWarning(
                        "Tag mismatch. Expected={Expected}, Got={Got}",
                        tag, abn.sTagName);
                    OpcUaServerLogBridge.Warn("TagUpdate", string.Format("Tag mismatch. Expected={0}, Got={1}", tag, abn.sTagName));
                    return;
                }

                BaseDataVariableState itemNode = abn.bdItemNode;
                if ((string)itemNode.Value == newValue) return;

                lock (Lock) // SystemContext 접근 시 락 필요
                {
                    itemNode.Value = newValue;
                    itemNode.Timestamp = DateTime.UtcNow;
                    itemNode.StatusCode = StatusCodes.Good;
                    itemNode.ClearChangeMasks(SystemContext, false);
                }
            }
            catch (Exception ex)
            {
                OpcUaServerLogBridge.Error("TagUpdate", string.Format("UpdateItem failed. Tag={0}, {1}", tag, ex.Message));
            }
        }

    }


}
