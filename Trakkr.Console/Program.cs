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

                inputFile.MoveTo(namePart + ".done");
                File.WriteAllLines(namePart + ".log", log);

                System.Console.ReadLine();
            }
        }

        private static IEnumerable<string> UpdateWorkItems(IConnection connection, List<TrakkrEntry<string>> entries)
        {
            var log = new List<string>();
            var issueManagement = new IssueManagement(connection);
            foreach (var trakkrEntry in entries)
            {
                var minutes = (int) Math.Round(trakkrEntry.Duration.TotalMinutes);
                
                var issueExists = issueManagement.CheckIfIssueExists(trakkrEntry.Mark);
                if (issueExists)
                {
                    System.Console.Write($"{trakkrEntry.Mark}: {minutes} Minutes. Add (y/n) ? ");
                    var key = System.Console.ReadKey(false);
                    System.Console.WriteLine();

                    if (key.Key == ConsoleKey.Y)
                    {
                        var doc = "<workItem>"
                                  + $"<date>{GetUnixTimestampMilliseconds(trakkrEntry.Day)}</date>"
                                  + $"<duration>{minutes}</duration>"
                                  + "</workItem>";

                        var response = connection.PostXml($"issue/{trakkrEntry.Mark}/timetracking/workitem", doc);
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            System.Console.WriteLine($"Work item saved here: {response.Location}");
                            log.Add($"Ticket {trakkrEntry.Mark} : {minutes}m : {response.Location}");
                        }

                        //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
                    }
                    else
                    {
                        log.Add($"Ticket {trakkrEntry.Mark} : {minutes}m : NOT ADDED!");
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
                    log.Add($"Unable to find issue {trakkrEntry.Mark}");

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

        private static List<TrakkrEntry<string>> ParseEvents(FileInfo inputFile)
        {
            List<TrakkrEntry<string>> entries = new List<TrakkrEntry<string>>();
            
            using (var reader = inputFile.OpenText())
            {
                // first line is date
                var dateLine = reader.ReadLine();
                DateTime date;
                if (DateTime.TryParseExact(dateLine, "yyyy-MM-dd", null, DateTimeStyles.None, out date))
                {
                    var trakkr = new Trakkr<string>();
                    date = date.Date;
                    string timeLine;
                    string[] timeSplit;
                    TrakkrEntry<string> trakkrEntry;
                    while (!reader.EndOfStream)
                    {
                        timeLine = reader.ReadLine();
                        timeSplit = timeLine.Split(' ');
                        if (timeSplit.Length < 2)
                        {
                            System.Console.WriteLine("skipping line: " + timeLine);
                        }
                        else
                        {
                            var time = DateTime.ParseExact(timeSplit[0], "HHmm", null, DateTimeStyles.None).TimeOfDay;
                            var eventTime = date.Add(time);
                            var ticket = timeSplit[1];
                            var isStopEvent = string.Equals(ticket, "stop");

                            if (isStopEvent)
                            {
                                trakkrEntry = trakkr.HandleStopEvent(eventTime);
                            }
                            else
                            {
                                trakkrEntry = trakkr.HandleStartEvent(eventTime, ticket);
                            }

                            if (trakkrEntry != null)
                            {
                                entries.Add(trakkrEntry);
                            }
                            //var comment = timeSplit[2];
                        }
                    }
                }
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
