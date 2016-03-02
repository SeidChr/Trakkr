using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using Trakkr.Core;
using Trakkr.YouTrack;
using YouTrackSharp.Issues;

namespace Trakkr.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TrakkrEntry<string>> entries = new List<TrakkrEntry<string>>();
            System.Console.Write("Youtrack Host: ");
            var host = System.Console.ReadLine();
            System.Console.Write("Username: ");
            var username = System.Console.ReadLine();
            System.Console.Write("Password: ");
            var password = System.Console.ReadLine();


            var connection = YouTrackManager.GetConnection(host, username, password);
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
                System.Console.WriteLine($"{trakkrEntry.Mark}: {(int)Math.Round(trakkrEntry.Duration.TotalMinutes)} Minutes");
                var issueExists = issueManagement.CheckIfIssueExists(trakkrEntry.Mark);
                if (issueExists)
                {
                    System.Console.WriteLine($"Found that Issue: {trakkrEntry.Mark}");

                    //https://confluence.jetbrains.com/display/YTD6/Get+Available+Work+Items+of+Issue

                    // get all workitems
                    // GET /rest/issue/{issue}/timetracking/workitem/
                    //var result = connection.Get<object>($"issue/{trakkrEntry.Mark}/timetracking/workitem/");

                    // add a workitem
                    //connection.Post($"issue/{trakkrEntry.Mark}/timetracking/workitem", "<xml/>");
                    //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
                    // <workItem>
                    //   <date>1353316956611</date>
                    //   <duration>240</duration>
                    //   <description>first work item</description>
                    //   <worktype>
                    //     <name>Development</name>
                    //   </worktype>
                    // </workItem>
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
