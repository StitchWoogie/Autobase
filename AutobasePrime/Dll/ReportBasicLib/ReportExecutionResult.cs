namespace ReportBasicLib
{
    public class ReportExecutionResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public REPORT_STRUCT Report { get; set; }
    }
}
