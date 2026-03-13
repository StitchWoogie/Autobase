using AutoLibLocal;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace GraphicModule
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

    [ServiceContract]
    public interface IServiceExcelWorkerQueue
    {
        [OperationContract]
        ExcelWorkerJobInfo EnqueueExcelReportJobWithDic(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        ExcelWorkerJobInfo GetExcelReportJob(string jobId);
    }

    public class ServiceExcelWorkerClient : IDisposable
    {
        private readonly ChannelFactory<IServiceExcelWorkerQueue> _factory;
        private readonly IClientChannel _clientChannel;
        private readonly IServiceExcelWorkerQueue _service;

        public ServiceExcelWorkerClient(int timeoutSeconds)
        {
            string url = ConfigVarTotal.GetServicePath("ServiceExcelReport.svc");

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.OpenTimeout = TimeSpan.FromSeconds(timeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(timeoutSeconds);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(timeoutSeconds);
            binding.MaxReceivedMessageSize = 1024 * 1024;

            EndpointAddress address = new EndpointAddress(url);

            _factory = new ChannelFactory<IServiceExcelWorkerQueue>(binding, address);
            _service = _factory.CreateChannel();
            _clientChannel = (IClientChannel)_service;
        }

        public ExcelWorkerJobInfo EnqueueWithDic(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int handAuto)
        {
            return _service.EnqueueExcelReportJobWithDic(keys, values, filename, tHand, tAuto, tMinListFr, tMinListTo, handAuto);
        }

        public ExcelWorkerJobInfo GetJob(string jobId)
        {
            return _service.GetExcelReportJob(jobId);
        }

        public ExcelWorkerJobInfo WaitForCompleted(string jobId, int timeoutSeconds, int pollMilliseconds)
        {
            DateTime begin = DateTime.Now;

            while (true)
            {
                ExcelWorkerJobInfo job = GetJob(jobId);
                if (job == null)
                {
                    throw new Exception("Excel Worker 작업 정보를 가져오지 못했습니다.");
                }

                if (String.Compare(job.Status, "Completed", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return job;
                }

                if (String.Compare(job.Status, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    throw new Exception(String.IsNullOrEmpty(job.ErrorMessage) ? "Excel Worker 작업이 실패했습니다." : job.ErrorMessage);
                }

                if ((DateTime.Now - begin).TotalSeconds >= timeoutSeconds)
                {
                    throw new TimeoutException("Excel Worker 작업 대기 시간이 초과되었습니다.");
                }

                Thread.Sleep(pollMilliseconds);
            }
        }

        public void Dispose()
        {
            try
            {
                if (_clientChannel != null)
                {
                    _clientChannel.Close();
                }
            }
            catch
            {
                if (_clientChannel != null)
                {
                    _clientChannel.Abort();
                }
            }

            try
            {
                if (_factory != null)
                {
                    _factory.Close();
                }
            }
            catch
            {
                if (_factory != null)
                {
                    _factory.Abort();
                }
            }
        }
    }
}
