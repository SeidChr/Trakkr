using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using JsonFx.Serialization.GraphCycles;
using Trakkr.Core;
using Trakkr.Core.Events;
using Trakkr.Parse;
using Trakkr.YouTrack;
using YouTrackSharp.Issues;

namespace Trakkr.Console
{
    class Program
    {
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetUnixTimestampMilliseconds(DateTime dateTime)
        {
            return (long)dateTime.Subtract(UnixEpoch).TotalMilliseconds;
        }

        static void Main(string[] args)
        {
            if (args[0] == "RESET")
            {
                ResetSettings();
            }
            else
            {
                var inputFile = new FileInfo(args[0]);

                var connection = Authenticate();

                var entries = ParseEvents(inputFile);

                var log = UpdateWorkItems(connection, entries);
                var namePart = Path.Combine(Path.GetDirectoryName(inputFile.FullName), Path.GetFileNameWithoutExtension(inputFile.FullName));

                System.Console.WriteLine("Save log to: " + namePart + ".log");

                //inputFile.MoveTo(namePart + ".done");
                File.AppendAllLines(namePart + ".log", log);

                System.Console.ReadKey(true);
            }
        }

        private static bool CheckIfIssueExists(IssueManagement issueManagement, string issue)
        {
            bool result;

            try
            {
                result = issueManagement.CheckIfIssueExists(issue);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private static Dictionary<string, string> IssueToDict(Issue issue)
        {
            var expando = issue.ToExpandoObject();
            return expando.ToDictionary(kvp => kvp.Key, kvp => (string)kvp.Value);
        }

        private static IEnumerable<string> UpdateWorkItems(IConnection connection, IEnumerable<IEntry<ShortTrackingFormatPayload>> entries)
        {
            var log = new List<string>
            {
                $"Updating Workitems : {DateTime.Now.ToString("O")}"
            };

            var issueManagement = new IssueManagement(connection);
            foreach (var trakkrEntry in entries)
            {
                var minutes = (int) Math.Round(trakkrEntry.Duration.TotalMinutes);
                
                if (CheckIfIssueExists(issueManagement, trakkrEntry.Payload.Query))
                {
                    dynamic issue = issueManagement.GetIssue(trakkrEntry.Payload.Query);
                    //var issue = IssueToDict(issueManagement.GetIssue(trakkrEntry.Payload.Query));
                    //System.Console.Write(string.Join("; ", issue.Select((k, v) => "{" + k + " = " + v + "}")));

                    string summary = issue.summary;

                    System.Console.Write($"{trakkrEntry.Timestamp.ToShortDateString()} : {trakkrEntry.Payload.Query} {summary} : {minutes} Minutes ({trakkrEntry.Payload.Descrition}). Add (y/n) ? ");
                    var key = System.Console.ReadKey(false);
                    System.Console.WriteLine();

                    if (key.Key == ConsoleKey.Y)
                    {
                        var doc = "<workItem>"
                                  + $"<date>{GetUnixTimestampMilliseconds(trakkrEntry.Timestamp)}</date>"
                                  + $"<duration>{minutes}</duration>"
                                  + $"<description>{trakkrEntry.Payload.Descrition}</description>"
                                  + "</workItem>";

                        var response = connection.PostXml($"issue/{trakkrEntry.Payload.Query}/timetracking/workitem", doc);
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            System.Console.WriteLine($"Work item saved here: {response.Location}");
                            log.Add($"Ticket {trakkrEntry.Payload.Query} ({trakkrEntry.Payload.Descrition}) : {minutes}m : {response.Location}");
                        }

                        //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
                    }
                    else
                    {
                        log.Add($"Ticket {trakkrEntry.Payload.Query} : {minutes}m : NOT ADDED!");
                    }

                    //https://confluence.jetbrains.com/display/YTD6/Get+Available+Work+Items+of+Issue

                    // get all workitems
                    // GET /rest/issue/{issue}/timetracking/workitem/
                    //var result = connection.Get<object>($"issue/{trakkrEntry.Mark}/timetracking/workitem/");

                    // add a workitem
                    //connection.Post($"issue/{trakkrEntry.Mark}/timetracking/workitem", "<xml/>");
                    //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
                }
                else
                {
                    var message = $"Unable to find issue {trakkrEntry.Payload.Query}";
                    System.Console.WriteLine(message);
                    log.Add(message);

                    //var issues = issueManagement.GetIssuesBySearch(trakkrEntry.Mark);
                    //foreach (var issue in issues)
                    //{

                    //    System.Console.WriteLine($"Issue: {issue.Id}");
                    //    var members = issue.GetDynamicMemberNames();
                    //    foreach (var member in members)
                    //    {
                    //        System.Console.WriteLine($"Member: {member}");
                    //    }
                    //}
                }
            }

            return log;
        }

        private static IEnumerable<IEntry<ShortTrackingFormatPayload>> ParseEvents(FileInfo inputFile)
        {
            IEnumerable<IEntry<ShortTrackingFormatPayload>> entries = new List<IEntry<ShortTrackingFormatPayload>>();
            string input = null;

            var parser = new ShortTrackingFormatParser();
            var trakkr = new Trakkr<ShortTrackingFormatPayload>();

            using (var reader = inputFile.OpenText())
            {
                input = reader.ReadToEnd();
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                entries = trakkr.HandleEvents(parser.Parse(input));
            }

            return entries;
        }

        private static IConnection Authenticate()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Host))
            {
                System.Console.Write("Youtrack Host: ");
                var host = System.Console.ReadLine();
                Properties.Settings.Default.Host = host;
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Username))
            {
                System.Console.Write("Username: ");
                var username = System.Console.ReadLine();
                Properties.Settings.Default.Username = username;
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Password))
            {
                System.Console.Write("Password: ");
                var password = System.Console.ReadLine();
                Properties.Settings.Default.Password = password;
            }

            var connection = YouTrackManager.GetConnection(Properties.Settings.Default.Host,
                Properties.Settings.Default.Username, Properties.Settings.Default.Password);

            if (connection.IsAuthenticated)
            {
                Properties.Settings.Default.Save();
            }
            return connection;
        }

        private static void ResetSettings()
        {
            Properties.Settings.Default.Host = string.Empty;
            Properties.Settings.Default.Username = string.Empty;
            Properties.Settings.Default.Password = string.Empty;
            Properties.Settings.Default.Save();
        }
    }
}
