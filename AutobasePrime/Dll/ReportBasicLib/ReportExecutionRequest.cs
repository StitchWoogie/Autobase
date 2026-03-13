using System;
using System.Collections.Generic;

namespace ReportBasicLib
{
    public class ReportExecutionRequest
    {
        public string TemplateFile { get; set; }

        public EnumHandAuto HandAuto { get; set; } = EnumHandAuto.HAND_MODE;

        public DateTime HandTime { get; set; } = DateTime.Now;

        public DateTime AutoTime { get; set; } = DateTime.Now;

        public DateTime MinListFrom { get; set; } = DateTime.Now;

        public DateTime MinListTo { get; set; } = DateTime.Now;

        public IDictionary<string, string> StringVariables { get; set; }
    }
}
