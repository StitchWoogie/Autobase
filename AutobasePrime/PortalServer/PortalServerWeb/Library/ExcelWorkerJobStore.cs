using Newtonsoft.Json;
using PortalServerWeb.AutoWeb.Service;
using System;
using System.IO;
using System.Linq;

namespace PortalServerWeb.Library
{
    public class ExcelWorkerJobStore
    {
        public const string StatusQueued = "Queued";
        public const string StatusRunning = "Running";
        public const string StatusCompleted = "Completed";
        public const string StatusFailed = "Failed";

        private static readonly object SyncRoot = new object();

        private readonly string _jobsDir;

        public ExcelWorkerJobStore(string workDir)
        {
            string rootDir = Path.Combine(workDir, "ExcelWorker");
            _jobsDir = Path.Combine(rootDir, "Jobs");
            Directory.CreateDirectory(_jobsDir);
        }

        public ExcelWorkerJobInfo Enqueue(
            string filename,
            DateTime tHand,
            DateTime tAuto,
            DateTime tMinListFr,
            DateTime tMinListTo,
            int handAuto,
            string[] keys,
            string[] values)
        {
            var job = new ExcelWorkerJobInfo
            {
                JobId = Guid.NewGuid().ToString("N"),
                Status = StatusQueued,
                Filename = filename,
                THand = tHand,
                TAuto = tAuto,
                TMinListFr = tMinListFr,
                TMinListTo = tMinListTo,
                HandAuto = handAuto,
                Keys = keys ?? new string[0],
                Values = values ?? new string[0],
                CreatedAtUtc = DateTime.UtcNow
            };

            lock (SyncRoot)
            {
                Save(job);
            }

            return job;
        }

        public ExcelWorkerJobInfo Get(string jobId)
        {
            lock (SyncRoot)
            {
                return Load(jobId);
            }
        }

        public ExcelWorkerJobInfo ClaimNext(string workerName)
        {
            lock (SyncRoot)
            {
                foreach (string jobDir in Directory.GetDirectories(_jobsDir).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                {
                    string jobId = Path.GetFileName(jobDir);
                    ExcelWorkerJobInfo job = Load(jobId);
                    if (job == null || job.Status != StatusQueued)
                    {
                        continue;
                    }

                    job.Status = StatusRunning;
                    job.WorkerName = workerName;
                    job.StartedAtUtc = DateTime.UtcNow;
                    Save(job);
                    return job;
                }
            }

            return null;
        }

        public ExcelWorkerJobInfo Complete(string jobId, string workerName, string resultUrl, string resultFileName)
        {
            lock (SyncRoot)
            {
                ExcelWorkerJobInfo job = Load(jobId);
                if (job == null)
                {
                    return null;
                }

                job.Status = StatusCompleted;
                job.WorkerName = workerName;
                job.ResultUrl = resultUrl;
                job.ResultFileName = resultFileName;
                job.FinishedAtUtc = DateTime.UtcNow;
                job.ErrorMessage = null;
                Save(job);
                return job;
            }
        }

        public ExcelWorkerJobInfo Fail(string jobId, string workerName, string errorMessage)
        {
            lock (SyncRoot)
            {
                ExcelWorkerJobInfo job = Load(jobId);
                if (job == null)
                {
                    return null;
                }

                job.Status = StatusFailed;
                job.WorkerName = workerName;
                job.ErrorMessage = errorMessage;
                job.FinishedAtUtc = DateTime.UtcNow;
                Save(job);
                return job;
            }
        }

        private ExcelWorkerJobInfo Load(string jobId)
        {
            string file = GetJobFile(jobId);
            if (!File.Exists(file))
            {
                return null;
            }

            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<ExcelWorkerJobInfo>(json);
        }

        private void Save(ExcelWorkerJobInfo job)
        {
            string dir = Path.Combine(_jobsDir, job.JobId);
            Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(job, Formatting.Indented);
            File.WriteAllText(GetJobFile(job.JobId), json);
        }

        private string GetJobFile(string jobId)
        {
            return Path.Combine(_jobsDir, jobId, "job.json");
        }
    }
}
