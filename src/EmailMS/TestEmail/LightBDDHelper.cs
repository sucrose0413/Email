﻿using LightBDD.Core.Configuration;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.XUnit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: TestEmail.ConfiguredLightBddScope]

namespace TestEmail
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ReportWritersConfiguration()
                .Clear()
                .AddFileWriter<XmlReportFormatter>(@"~/Reports/FeaturesReport.xml")
                .AddFileWriter<PlainTextReportFormatter>("~/Reports/{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt")
                .AddFileWriter<HtmlReportFormatter>(@"~/Reports/LightBDDHtmlReport.html")
                .AddFileWriter<MarkdownReportFormatter>(@"~/Reports/LightBDDReport.md")
                ;

        }
        public class MarkdownReportFormatter : IReportFormatter
        {
            public string WriteSubSteps(IStepResult step, string prefixStep)
            {
                var subSteps = step.GetSubSteps();
                if ((subSteps?.Count() ?? 0) == 0)
                    return null;
                StringBuilder sb = new StringBuilder();



                foreach (var subStep in subSteps)
                {
                    string comments = string.Join(";", step.Comments);
                    sb.AppendLine($"|{prefixStep}{subStep.Info.Number}|{subStep.Info.Name}|{subStep.Status}|{comments}|");

                }
                return sb.ToString();
            }
            public void Format(Stream stream, params IFeatureResult[] features)
            {
                var categories = GroupCategories(features);
                var sb = new StringBuilder();
                var parents = categories
                        .Select(it => it.Info.Parent)
                        .Distinct()
                        .ToArray();
                sb.AppendLine("");
                sb.AppendLine("# Results of tests");
                sb.AppendLine("");
                foreach (var par in parents)
                {
                    var items = categories.Where(it => it.Info.Parent == par).ToArray();
                    if (items.Length == 0)
                        continue;
                    sb.AppendLine("");
                    sb.AppendLine($"## {par.Name}");
                    sb.AppendLine("");
                    foreach (var sc in items)
                    {
                        sb.AppendLine("");
                        sb.AppendLine($"### {sc.Info.ToString()}");
                        sb.AppendLine("");
                        sb.AppendLine("| Number| Name|Status|Comments|");
                        sb.AppendLine("| ----------- | ----------- |----------- |----------- |");
                        foreach (var step in sc.GetSteps())
                        {
                            string comments = string.Join(";", step.Comments);
                            var status = step.Status.ToString();
                            if (step.Status == ExecutionStatus.Failed)
                            {
                                status = $"<span style='color: red'>*{step.Status}*</span>";
                            }
                            sb.AppendLine($"|{step.Info.Number}|{step.Info.Name}|{status}|{comments}|");
                            //put also step sub steps
                            var subSteps = step.GetSubSteps();
                            if ((subSteps?.Count() ?? 0) == 0)
                                continue;
                            //foreach (var subStep in subSteps)
                            //{
                            //    sb.AppendLine($"|a{subStep.Info.Number}|{subStep.Info.Name}|{subStep.Status}|");
                            //}
                            sb.Append(WriteSubSteps(step, step.Info.Number + "."));

                        }
                    }

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    var sw = new StreamWriter(ms);
                    try
                    {
                        sw.Write(sb);
                        sw.Flush();//otherwise you are risking empty stream
                        ms.Seek(0, SeekOrigin.Begin);

                        ms.WriteTo(stream);
                    }
                    finally
                    {
                        sw.Dispose();
                    }
                }
            }
            private static IScenarioResult[] GroupCategories(IEnumerable<IFeatureResult> features)
            {
                var f = features
                    .SelectMany(f => f.GetScenarios())
                    .ToArray()
                    ;
                return f;
            }
        }
    }


}
