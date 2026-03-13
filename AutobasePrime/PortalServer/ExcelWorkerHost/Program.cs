using PortalServerWeb.AutoWeb.Service;
using PortalServerWeb.Library;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ExcelWorkerHost
{
    internal static class Program
    {
        private static string _runtimeDir;

        private sealed class WorkerOptions
        {
            public string WorkDir { get; set; }

            public string BaseUrl { get; set; }

            public string RuntimeDir { get; set; }

            public string WorkerName { get; set; }

            public bool RunOnce { get; set; }

            public int PollSeconds { get; set; }
        }

        private static int Main(string[] args)
        {
            WorkerOptions options;

            try
            {
                options = ParseOptions(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                PrintUsage();
                return 1;
            }

            _runtimeDir = ResolveRuntimeDir(options.RuntimeDir);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Console.WriteLine("ExcelWorkerHost 시작");
            Console.WriteLine("WorkDir   : {0}", options.WorkDir);
            Console.WriteLine("RuntimeDir: {0}", _runtimeDir);
            Console.WriteLine("Worker    : {0}", options.WorkerName);
            Console.WriteLine("Mode      : {0}", options.RunOnce ? "Once" : "Loop");

            ExcelWorkerJobStore store = new ExcelWorkerJobStore(options.WorkDir);

            while (true)
            {
                ExcelWorkerJobInfo job = store.ClaimNext(options.WorkerName);
                if (job == null)
                {
                    if (options.RunOnce)
                    {
                        Console.WriteLine("처리할 작업이 없습니다.");
                        return 0;
                    }

                    Thread.Sleep(options.PollSeconds * 1000);
                    continue;
                }

                ProcessJob(store, options, job);

                if (options.RunOnce)
                {
                    return 0;
                }
            }
        }

        private static void ProcessJob(ExcelWorkerJobStore store, WorkerOptions options, ExcelWorkerJobInfo job)
        {
            Console.WriteLine("작업 시작: {0}", job.JobId);

            try
            {
                string file = Path.GetFileName(job.Filename ?? string.Empty);
                string sourceFile = Path.Combine(options.WorkDir, "Report", file);
                string targetName = MakeTargetFileName(file, job.THand);
                string targetFile = Path.Combine(options.WorkDir, "Result", targetName);

                if (!File.Exists(sourceFile))
                {
                    throw new FileNotFoundException("양식 파일을 찾을 수 없습니다.", sourceFile);
                }

                ApplyStringVariables(job.Keys, job.Values);
                ExecuteExcelMakeResult(sourceFile, targetFile, job.THand, job.TMinListFr, job.TMinListTo);

                string resultUrl = BuildResultUrl(options.BaseUrl, targetName);
                store.Complete(job.JobId, options.WorkerName, resultUrl, targetName);
                Console.WriteLine("작업 완료: {0}", job.JobId);
            }
            catch (Exception ex)
            {
                store.Fail(job.JobId, options.WorkerName, ex.ToString());
                Console.Error.WriteLine("작업 실패: {0}", job.JobId);
                Console.Error.WriteLine(ex);
            }
        }

        private static void ExecuteExcelMakeResult(string sourceFile, string targetFile, DateTime tData, DateTime tMinListFr, DateTime tMinListTo)
        {
            string assemblyPath = Path.Combine(_runtimeDir, "OfficeExcelLibrary.dll");
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type type = assembly.GetType("OfficeExcelLibrary.ClassExcel", true);
            MethodInfo method = type.GetMethod("MakeResult", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                throw new MissingMethodException("OfficeExcelLibrary.ClassExcel.MakeResult");
            }

            method.Invoke(null, new object[]
            {
                sourceFile,
                targetFile,
                tData,
                tMinListFr,
                tMinListTo,
                true
            });
        }

        private static void ApplyStringVariables(string[] keys, string[] values)
        {
            Type configType = Type.GetType("AutoLibLocal.ConfigVarTotal, AutoLibLocal", false);
            if (configType == null)
            {
                return;
            }

            FieldInfo keysField = configType.GetField("varKeys", BindingFlags.Public | BindingFlags.Static);
            FieldInfo valuesField = configType.GetField("varValues", BindingFlags.Public | BindingFlags.Static);

            if (keysField != null)
            {
                keysField.SetValue(null, keys);
            }

            if (valuesField != null)
            {
                valuesField.SetValue(null, values);
            }
        }

        private static string MakeTargetFileName(string fileName, DateTime t)
        {
            return string.Format("{6}_{0}{1:00}{2:00}-{3:00}{4:00}{5:00}{7}",
                t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second,
                Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
        }

        private static string BuildResultUrl(string baseUrl, string targetName)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            return baseUrl.TrimEnd('/') + "/AutoWeb/Project/Result/" + targetName;
        }

        private static string ResolveRuntimeDir(string runtimeDir)
        {
            string[] candidates = new[]
            {
                runtimeDir,
                Environment.GetEnvironmentVariable("AUTOBASE_EXCEL_RUNTIME_DIR"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoWeb", "Runtime"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Runtime"),
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Dll\OfficeExcelLibrary"))
            };

            foreach (string candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "OfficeExcelLibrary.dll")))
                {
                    return candidate;
                }
            }

            throw new DirectoryNotFoundException("OfficeExcelLibrary 런타임 폴더를 찾을 수 없습니다.");
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            string path = Path.Combine(_runtimeDir, assemblyName);

            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            return null;
        }

        private static WorkerOptions ParseOptions(string[] args)
        {
            WorkerOptions options = new WorkerOptions
            {
                WorkerName = Environment.MachineName,
                PollSeconds = 5
            };

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg.ToLowerInvariant())
                {
                    case "--workdir":
                        options.WorkDir = RequireValue(args, ref i, arg);
                        break;
                    case "--baseurl":
                        options.BaseUrl = RequireValue(args, ref i, arg);
                        break;
                    case "--runtimedir":
                        options.RuntimeDir = RequireValue(args, ref i, arg);
                        break;
                    case "--worker":
                        options.WorkerName = RequireValue(args, ref i, arg);
                        break;
                    case "--poll":
                        options.PollSeconds = Math.Max(1, int.Parse(RequireValue(args, ref i, arg)));
                        break;
                    case "--once":
                        options.RunOnce = true;
                        break;
                    default:
                        throw new ArgumentException("알 수 없는 옵션: " + arg);
                }
            }

            if (string.IsNullOrWhiteSpace(options.WorkDir))
            {
                throw new ArgumentException("--workdir 옵션이 필요합니다.");
            }

            return options;
        }

        private static string RequireValue(string[] args, ref int index, string optionName)
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException(optionName + " 값이 필요합니다.");
            }

            index++;
            return args[index];
        }

        private static void PrintUsage()
        {
            Console.WriteLine("사용법:");
            Console.WriteLine("  ExcelWorkerHost --workdir <AutoWeb\\\\Project 경로> [--baseurl <사이트기본URL>] [--runtimedir <Runtime 경로>] [--worker <이름>] [--poll <초>] [--once]");
        }
    }
}
