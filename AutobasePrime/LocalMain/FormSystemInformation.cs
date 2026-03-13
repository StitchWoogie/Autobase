using NetTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain
{
    public partial class FormSystemInformation : Form
    {
        public static Form formThis = null;

        private SystemMonitor _monitor;
        private ListViewItem _lviBootTime;
        private Dictionary<EnumSystemInformation, ListViewItem> _itemMap
            = new Dictionary<EnumSystemInformation, ListViewItem>();

        public FormSystemInformation()
        {
            InitializeComponent();

            formThis = this;
        }

        private void FormSystemInformation_Load(object sender, EventArgs e)
        {
            _monitor = new SystemMonitor();

            // WMI 쿼리를 백그라운드에서 수행 (UI hang 방지)
            _ = _monitor.LoadBootTimeAsync();

            // 정적 항목
            AddStaticItem("OS Version", "", Environment.OSVersion.Version.ToString());
            AddStaticItem(".NET Version", "", Environment.Version.ToString());
            _lviBootTime = AddStaticItem("OS Started Time", "OS 시작 시간", "");
            AddStaticItem("Program Started Time", "프로그램 시작 시간",
                          SystemInformation.tStartup.ToString("yyyy-MM-dd HH:mm:ss"));

            // 동적 항목 (타이머로 갱신)
            AddDynamicItem("Trend Saving Utilization", "트랜드 저장 부하율", EnumSystemInformation.TrendSavingUtilization);
            AddDynamicItem("Trend Saving Count", "트랜드 저장 개수", EnumSystemInformation.TrendSavingCount);
            AddDynamicItem("Now", "현재시간", EnumSystemInformation.DateTimeNow);
            AddDynamicItem("Main Timer", "메인 타이머", EnumSystemInformation.MainTimer);
            AddDynamicItem("CPU Usage", "CPU 사용률", EnumSystemInformation.CpuUsage);
            AddDynamicItem("Memory Usage", "메모리 사용량", EnumSystemInformation.MemoryUsage);

            timer1.Interval = 100;
            timer1.Start();
        }

        private ListViewItem AddStaticItem(string title, string titleKorean, string value)
        {
            string display = title;
            if (Tools.IsLangKorean() && titleKorean.Length > 0)
                display = titleKorean;

            var lvi = new ListViewItem(display);
            lvi.SubItems.Add(value);
            listView1.Items.Add(lvi);
            return lvi;
        }

        private void AddDynamicItem(string title, string titleKorean, EnumSystemInformation type)
        {
            string display = title;
            if (Tools.IsLangKorean() && titleKorean.Length > 0)
                display = titleKorean;

            var lvi = new ListViewItem(display);
            lvi.SubItems.Add("");
            listView1.Items.Add(lvi);
            _itemMap[type] = lvi;
        }

        private void FormSystemInformation_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();

            if (_monitor != null)
            {
                _monitor.Dispose();
                _monitor = null;
            }

            formThis = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var snap = _monitor.GetSnapshot();

            // Snapshot 기반 값
            UpdateItem(EnumSystemInformation.CpuUsage,
                       snap.CpuUsage.ToString("F1") + "%");

            UpdateItem(EnumSystemInformation.MemoryUsage,
                       snap.MemoryMB.ToString("F0") + " MB");

            UpdateItem(EnumSystemInformation.DateTimeNow,
                       snap.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // OS 부팅 시간 (비동기 로드 완료 후 표시)
            if (snap.OsBootTime.HasValue && _lviBootTime != null)
            {
                string bootText = snap.OsBootTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                if (_lviBootTime.SubItems[1].Text != bootText)
                    _lviBootTime.SubItems[1].Text = bootText;
            }

            // 외부에서 SystemInformation.SetValue()로 설정되는 값
            UpdateChangingValue(EnumSystemInformation.TrendSavingUtilization, "%", "{0:F2}", 10);
            UpdateChangingValue(EnumSystemInformation.TrendSavingCount, "", "{0}", 1);
            UpdateChangingValue(EnumSystemInformation.MainTimer, "msec", "{0:F1}", 1);
        }

        /// <summary>
        /// SystemInformation.ChangingValues 에서 값을 읽어 ListView 갱신
        /// </summary>
        private void UpdateChangingValue(EnumSystemInformation e, string unit, string format, double multiply)
        {
            var val = SystemInformation.ChangingValues[(int)e];
            if (val == null) return;

            string text;
            if (val is double d)
                text = string.Format(format, d * multiply);
            else
                text = string.Format(format, val);

            if (unit.Length > 0)
                text += unit;

            UpdateItem(e, text);
        }

        private void UpdateItem(EnumSystemInformation type, string text)
        {
            if (!_itemMap.TryGetValue(type, out var lvi))
                return;

            if (lvi.SubItems[1].Text != text)
                lvi.SubItems[1].Text = text;
        }
    }


        class SystemInformation
    {
        //const int MAX_CHANGING_VALUE = 5;
       // public static object[] ChangingValues = new object[MAX_CHANGING_VALUE];

        public static object[] ChangingValues =
    new object[Enum.GetValues(typeof(EnumSystemInformation)).Cast<int>().Max() + 1]; //251028 PSU 

        public List<SystemInformationItem> arrayItems = new List<SystemInformationItem>();

        public static DateTime tStartup;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="title_korean"></param>
        /// <param name="changing_value_flag">계속 변화되는 값인가?</param>
        /// <param name="e"></param>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <param name="format_string"></param>
        /// <param name="multiply"></param>
        public void Add(string title, string title_korean, bool changing_value_flag, EnumSystemInformation e, object value, string unit, string format_string, double multiply)
        {
            SystemInformationItem sii = new SystemInformationItem();

            sii.title = title;
            sii.title_korean = title_korean;
            sii.bChangingValue = changing_value_flag;
            sii.eChangingValue = e;
            sii.value = value;
            sii.unit = unit;
            sii.format_string = format_string;
            sii.multiply = multiply;

            arrayItems.Add(sii);
        }

        public static void SetValue(EnumSystemInformation e, object value)
        {
            ChangingValues[(int)e] = value;
        }
    }

    class SystemInformationItem
    {
        public string title;
        public string title_korean;
        public bool bChangingValue;    // 변화되는 값이다.
        public EnumSystemInformation eChangingValue;
        public object value;
        public string unit = "";
        public string format_string = "{0}";
        public double multiply = 1; // 곱하는 수
    }

    enum EnumSystemInformation
    {
        TrendSavingUtilization = 0, // 트랜드 저장 부하율
        TrendSavingCount = 1,       // 트랜드 저장 갯수
        DateTimeNow = 2,            // 현재시간
        //BitmapCache = 3,            // 비트맵 캐시    CE에서만 사용
        MainTimer = 4,              // Main Timer   2017-2-23
        CpuUsage = 5,        // ← 프로그램 CPU 사용률 (%)
        MemoryUsage = 6,     // ← 프로그램 메모리 사용량 (MB)
    }

    public class SystemMonitor : IDisposable
    {
        private readonly Process _process;
        private TimeSpan _prevCpuTime;
        private DateTime _prevSampleTime;

        private DateTime? _bootTime;
        private bool _bootTimeLoading;

        public SystemMonitor()
        {
            _process = Process.GetCurrentProcess();
            _prevCpuTime = _process.TotalProcessorTime;
            _prevSampleTime = DateTime.UtcNow;
        }

        public async Task LoadBootTimeAsync()
        {
            if (_bootTimeLoading || _bootTime.HasValue)
                return;

            _bootTimeLoading = true;

            _bootTime = await Task.Run(() =>
            {
                try
                {
                    var query = new SelectQuery(
                        "SELECT LastBootUpTime FROM Win32_OperatingSystem WHERE Primary='true'");

                    using (var searcher = new ManagementObjectSearcher(query))
                    {
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            return ManagementDateTimeConverter.ToDateTime(
                                mo["LastBootUpTime"].ToString());
                        }
                    }
                }
                catch { }

                return (DateTime?)null;
            });
        }

        public SystemSnapshot GetSnapshot()
        {
            _process.Refresh();

            var now = DateTime.UtcNow;
            var currentCpuTime = _process.TotalProcessorTime;

            var cpuUsedMs = (currentCpuTime - _prevCpuTime).TotalMilliseconds;
            var elapsedMs = (now - _prevSampleTime).TotalMilliseconds;

            double cpuUsage = 0;

            if (elapsedMs > 0)
            {
                cpuUsage = cpuUsedMs /
                           (Environment.ProcessorCount * elapsedMs) * 100.0;
            }

            _prevCpuTime = currentCpuTime;
            _prevSampleTime = now;

            return new SystemSnapshot
            {
                CpuUsage = cpuUsage,
                MemoryMB = _process.WorkingSet64 / 1024.0 / 1024.0,
                Now = DateTime.Now,
                OsBootTime = _bootTime
            };
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }

    public class SystemSnapshot
    {
        public double CpuUsage { get; set; }
        public double MemoryMB { get; set; }
        public DateTime Now { get; set; }
        public DateTime? OsBootTime { get; set; }
    }
}
