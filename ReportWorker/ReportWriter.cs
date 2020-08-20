using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ReportWorker
{

    public interface IReportWriter
    {
        void Save(string text);
    }

    public class ReportWriter : IReportWriter
    {
        public void Save(string text)
        {
            var logFile = "Reports/"+DateTime.Now.ToString("yyyy.MM.dd") + ".txt";
            if (!File.Exists(logFile))
            {
                File.Create(logFile);
            }

            using var sw  = new StreamWriter(logFile);
            sw.Write(text);
        }
    }
}