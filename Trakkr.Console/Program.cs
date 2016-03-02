using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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

        public static long GetCurrentUnixTimestampSeconds(DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch).TotalSeconds;
        }

        static void Main(string[] args)
        {
            List<TrakkrEntry<string>> entries = new List<TrakkrEntry<string>>();
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


            var connection = YouTrackManager.GetConnection(Properties.Settings.Default.Host, Properties.Settings.Default.Username, Properties.Settings.Default.Password);

            if (connection.IsAuthenticated)
            {
                Properties.Settings.Default.Save();
            }

            var issueManagement = new IssueManagement(connection);

            var inputFile = new FileInfo(args[0]);
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

            foreach (var trakkrEntry in Trakkr<string>.Merge(entries))
            {
                var minutes = (int) Math.Round(trakkrEntry.Duration.TotalMinutes);
                System.Console.WriteLine($"{trakkrEntry.Mark}: {minutes} Minutes");
                var issueExists = issueManagement.CheckIfIssueExists(trakkrEntry.Mark);
                if (issueExists)
                {
                    System.Console.Write($"Found that Issue: {trakkrEntry.Mark}. Add {minutes} minutes to it? Y/N");
                    var key = System.Console.ReadKey(false);
                    System.Console.WriteLine();
                    if (key.Key == ConsoleKey.Y)
                    {
                        var doc = "<workItem>"
                                  + $"<date>{GetCurrentUnixTimestampSeconds(trakkrEntry.Start)}</date>"
                                  + $"<duration>{minutes}</duration>"
                                  //+ "<description>first work item</description>"
                                  + "</workItem>";
                        connection.Post($"issue/{trakkrEntry.Mark}/timetracking/workitem", doc);
                        //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
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
                    var issues = issueManagement.GetIssuesBySearch(trakkrEntry.Mark);


                    foreach (var issue in issues)
                    {
                        System.Console.WriteLine($"Issue: {issue.Id}");
                        var members = issue.GetDynamicMemberNames();
                        foreach (var member in members)
                        {
                            System.Console.WriteLine($"Member: {member}");
                        }

                    }
                }
            }

            System.Console.ReadLine();
        }
    }
}
