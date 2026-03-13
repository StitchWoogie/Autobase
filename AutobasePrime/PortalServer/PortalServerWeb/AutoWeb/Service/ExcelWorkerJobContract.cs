using System;
using System.Runtime.Serialization;

namespace PortalServerWeb.AutoWeb.Service
{
    [DataContract]
    public class ExcelWorkerJobInfo
    {
        [DataMember(Order = 1)]
        public string JobId { get; set; }

        [DataMember(Order = 2)]
        public string Status { get; set; }

        [DataMember(Order = 3)]
        public string Filename { get; set; }

        [DataMember(Order = 4)]
        public DateTime THand { get; set; }

        [DataMember(Order = 5)]
        public DateTime TAuto { get; set; }

        [DataMember(Order = 6)]
        public DateTime TMinListFr { get; set; }

        [DataMember(Order = 7)]
        public DateTime TMinListTo { get; set; }

        [DataMember(Order = 8)]
        public int HandAuto { get; set; }

        [DataMember(Order = 9)]
        public string[] Keys { get; set; }

        [DataMember(Order = 10)]
        public string[] Values { get; set; }

        [DataMember(Order = 11)]
        public string WorkerName { get; set; }

        [DataMember(Order = 12)]
        public DateTime CreatedAtUtc { get; set; }

        [DataMember(Order = 13)]
        public DateTime? StartedAtUtc { get; set; }

        [DataMember(Order = 14)]
        public DateTime? FinishedAtUtc { get; set; }

        [DataMember(Order = 15)]
        public string ResultUrl { get; set; }

        [DataMember(Order = 16)]
        public string ResultFileName { get; set; }

        [DataMember(Order = 17)]
        public string ErrorMessage { get; set; }
    }
}
