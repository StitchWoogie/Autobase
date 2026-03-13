using AutoLib;
using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportBasicLib
{
    public class ReportRuntimeFacade
    {
        public async Task<ReportExecutionResult> ExecuteFileAsync(ReportExecutionRequest request)
        {
            if (request == null)
            {
                return Fail("실행 요청 정보가 없습니다.");
            }

            if (string.IsNullOrWhiteSpace(request.TemplateFile))
            {
                return Fail("리포트 양식 파일 경로가 없습니다.");
            }

            string templateFile = ResolveTemplatePath(request.TemplateFile);
            if (!File.Exists(templateFile))
            {
                return Fail("리포트 양식 파일을 찾을 수 없습니다. " + templateFile);
            }

            RuntimeStateSnapshot snapshot = CaptureState();

            try
            {
                ApplyRequestState(request);

                MakeRunReport maker = new MakeRunReport();
                REPORT_STRUCT report = await maker.MakeByFile(templateFile, request.HandAuto).ConfigureAwait(false);
                if (report == null)
                {
                    return Fail("리포트 실행 결과가 비어 있습니다.");
                }

                return new ReportExecutionResult
                {
                    Success = true,
                    Report = report
                };
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
            finally
            {
                RestoreState(snapshot);
            }
        }

        public bool SaveResultToRptx(REPORT_STRUCT report, string fileName)
        {
            if (report == null || string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            return ReportFile.Save(report, fileName);
        }

        public bool SaveResultToCsv(REPORT_STRUCT report, string fileName)
        {
            if (report == null || string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            return ReportFile.SaveToCSV(report, fileName);
        }

        private static string ResolveTemplatePath(string templateFile)
        {
            if (Path.IsPathRooted(templateFile))
            {
                return templateFile;
            }

            return Path.Combine(MakeFilePath.GetProjectDirectory(), "Report", templateFile);
        }

        private static void ApplyRequestState(ReportExecutionRequest request)
        {
            ReportConfig.bRunByIIS = true;
            ReportConfig.tHandReportTime = request.HandTime;
            ReportConfig.tAutoReportTime = request.AutoTime;
            ReportConfig.SetMinListTimeFr(request.MinListFrom);
            ReportConfig.SetMinListTimeTo(request.MinListTo);

            if (request.StringVariables == null || request.StringVariables.Count == 0)
            {
                ConfigVarTotal.varKeys = null;
                ConfigVarTotal.varValues = null;
                return;
            }

            List<string> keys = new List<string>();
            List<string> values = new List<string>();

            foreach (KeyValuePair<string, string> item in request.StringVariables)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }

            ConfigVarTotal.varKeys = keys.ToArray();
            ConfigVarTotal.varValues = values.ToArray();
        }

        private static RuntimeStateSnapshot CaptureState()
        {
            return new RuntimeStateSnapshot
            {
                RunByIis = ReportConfig.bRunByIIS,
                MinListFrom = ReportConfig.tMinListTimeFrAtIIS,
                MinListTo = ReportConfig.tMinListTimeToAtIIS,
                HandTime = ReportConfig.tHandReportTime,
                AutoTime = ReportConfig.tAutoReportTime,
                VarKeys = ConfigVarTotal.varKeys,
                VarValues = ConfigVarTotal.varValues
            };
        }

        private static void RestoreState(RuntimeStateSnapshot snapshot)
        {
            ReportConfig.bRunByIIS = snapshot.RunByIis;
            ReportConfig.tMinListTimeFrAtIIS = snapshot.MinListFrom;
            ReportConfig.tMinListTimeToAtIIS = snapshot.MinListTo;
            ReportConfig.tHandReportTime = snapshot.HandTime;
            ReportConfig.tAutoReportTime = snapshot.AutoTime;
            ConfigVarTotal.varKeys = snapshot.VarKeys;
            ConfigVarTotal.varValues = snapshot.VarValues;
        }

        private static ReportExecutionResult Fail(string message)
        {
            return new ReportExecutionResult
            {
                Success = false,
                ErrorMessage = message
            };
        }

        private sealed class RuntimeStateSnapshot
        {
            public bool RunByIis { get; set; }

            public DateTime MinListFrom { get; set; }

            public DateTime MinListTo { get; set; }

            public DateTime HandTime { get; set; }

            public DateTime AutoTime { get; set; }

            public string[] VarKeys { get; set; }

            public string[] VarValues { get; set; }
        }
    }
}
