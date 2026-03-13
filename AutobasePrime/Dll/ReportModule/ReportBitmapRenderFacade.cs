using ReportBasicLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportModule
{
    public class ReportBitmapRenderFacade
    {
        public async Task<List<byte[]>> RenderPngBytesAsync(ReportExecutionRequest request)
        {
            ReportRuntimeFacade runtime = new ReportRuntimeFacade();
            ReportExecutionResult result = await runtime.ExecuteFileAsync(request).ConfigureAwait(false);
            if (!result.Success || result.Report == null)
            {
                throw new InvalidOperationException(result.ErrorMessage ?? "리포트 실행 중 오류가 발생했습니다.");
            }

            ReportPrint printer = new ReportPrint();
            return printer.MakeBitmapPages(result.Report, request.TemplateFile);
        }

        public async Task<List<string>> RenderPngFilesAsync(ReportExecutionRequest request, string outputDirectory, string filePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentException("출력 폴더 경로가 없습니다.", nameof(outputDirectory));
            }

            List<byte[]> pages = await RenderPngBytesAsync(request).ConfigureAwait(false);
            Directory.CreateDirectory(outputDirectory);

            string safePrefix = string.IsNullOrWhiteSpace(filePrefix)
                ? Path.GetFileNameWithoutExtension(request.TemplateFile)
                : filePrefix;

            List<string> files = new List<string>();
            for (int i = 0; i < pages.Count; i++)
            {
                string fileName = string.Format("{0}_{1:000}.png", safePrefix, i + 1);
                string fullPath = Path.Combine(outputDirectory, fileName);
                File.WriteAllBytes(fullPath, pages[i]);
                files.Add(fullPath);
            }

            return files;
        }
    }
}
