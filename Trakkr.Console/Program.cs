using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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
            System.Console.Write("R for RESET, Any other key to READ A FILE... ");
            var keyInfo = System.Console.ReadKey();
            System.Console.WriteLine();

            if (keyInfo.Key == ConsoleKey.R || (args.Length > 0 && args[0] == "RESET"))
            {
                ResetSettings();
            }
            else
            {
                var connection = Authenticate();

                System.Console.WriteLine("USER: " + Properties.Settings.Default.Username);
                System.Console.WriteLine("HOST: " + Properties.Settings.Default.Host + ":" + Properties.Settings.Default.Port);

                FileInfo inputFile = null;
                if (args.Length >= 1)
                {
                    inputFile = new FileInfo(args[0]);
                }
                else
                {
                    System.Console.Write("Please enter InputFile: ");
                    var inputFileName = System.Console.ReadLine();
                    inputFileName = inputFileName.Trim(' ', '\"');
                    inputFile = new FileInfo(inputFileName);
                    inputFile.Refresh();
                }

                //if (inputFile.Exists)
                //{
                //    System.Console.WriteLine("No input file given. Nothing to do.");
                //    System.Console.WriteLine($"File {inputFile.FullName} does not exist.");
                //    System.Console.Write("Press any key to exit.");
                //    System.Console.ReadKey(true);
                //}
                //else
                {
                    var namePart = Path.Combine(Path.GetDirectoryName(inputFile.FullName), Path.GetFileNameWithoutExtension(inputFile.FullName));
                    var logfileName = namePart + ".log";

                    System.Console.WriteLine("LOG : " + logfileName);


                    var entries = ParseEvents(inputFile);

                    var logger = new FileAppendLogger(logfileName, $"Updating Workitems : {DateTime.Now.ToString("O")}");

                    UpdateWorkItems(connection, entries, logger);

                    System.Console.WriteLine("Done (press any key).");
                    System.Console.ReadKey(true);
                }
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

        private static void UpdateWorkItems(IConnection connection, IEnumerable<IEntry<ShortTrackingFormatPayload>> entries, FileAppendLogger logger)
        {
            var issueManagement = new IssueManagement(connection);

            if (CanContinue(entries, issueManagement)) foreach (var trakkrEntry in entries)
                {
                    var minutes = (int)Math.Round(trakkrEntry.Duration.TotalMinutes);

                    if (CheckIfIssueExists(issueManagement, trakkrEntry.Payload.Query))
                    {
                        dynamic issue = issueManagement.GetIssue(trakkrEntry.Payload.Query);
                        //var issue = IssueToDict(issueManagement.GetIssue(trakkrEntry.Payload.Query));
                        //System.Console.Write(string.Join("; ", issue.Select((k, v) => "{" + k + " = " + v + "}")));

                        string summary = issue.summary;

                        System.Console.Write($"{trakkrEntry.Timestamp.ToShortDateString()} : {trakkrEntry.Payload.Query} {summary} : {minutes} Minutes ({trakkrEntry.Payload.Descrition}). Add (y/n) ? ");
                        var key = System.Console.ReadKey(false);
                        System.Console.Write(" ");

                        if (key.Key == ConsoleKey.Y)
                        {
                            var doc = "<workItem>"
                                      + $"<date>{GetUnixTimestampMilliseconds(trakkrEntry.Timestamp)}</date>"
                                      + $"<duration>{minutes}</duration>"
                                      + $"<description>{HttpUtility.HtmlEncode(trakkrEntry.Payload.Descrition)}</description>"
                                      + "</workItem>";

                            var response = connection.PostXml($"issue/{trakkrEntry.Payload.Query}/timetracking/workitem", doc);
                            if (response.StatusCode == HttpStatusCode.Created)
                            {
                                logger.Log($"Ticket {trakkrEntry.Payload.Query} ({trakkrEntry.Payload.Descrition}) : {minutes}m : {response.Location}");
                                System.Console.Write("SAVED!");
                            }
                            else
                            {
                                System.Console.Write($"ERROR! Response Code: {response.StatusCode}");
                                logger.Log($"Ticket {trakkrEntry.Payload.Query} ({trakkrEntry.Payload.Descrition})" +
                                           $" : {minutes}m" +
                                           $" : ERROR! Response Code: {response.StatusCode}");
                            }

                            //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
                        }
                        else
                        {
                            System.Console.Write("NOT ADDED.");
                            logger.Log($"Ticket {trakkrEntry.Payload.Query} : {minutes}m : NOT ADDED!");
                        }

                        System.Console.WriteLine();

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
                        var message = $"Ticket {trakkrEntry.Payload.Query} ({trakkrEntry.Payload.Descrition})" +
                           $" : {minutes}m" +
                           " : ERROR! TICKET NOT FOUND!";

                        System.Console.WriteLine(message);
                        logger.Log(message);

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
        }

        private static bool CanContinue(IEnumerable<IEntry<ShortTrackingFormatPayload>> entries, IssueManagement issueManagement)
        {
            var maxTicketDuration = new TimeSpan(0, 10, 0, 0);
            var difference = new TimeSpan(35, 0, 0, 0);
            var now = DateTime.Now;
            var max = now + difference;
            var min = now - difference;
            var @continue = true;

            if (entries.Any(e => e.Timestamp > max || e.Timestamp < min))
            {
                //System.Console.WriteLine(difference.TotalDays + " days off");
                var first = entries.First(e => e.Timestamp > max || e.Timestamp < min);
                System.Console.WriteLine($"There are entries that are very far in future or past. ({first.Payload.Query}: {first.Timestamp.ToShortDateString()}:{first.Timestamp.ToShortTimeString()})");
                @continue = false;
            }

            if (entries.Any(e => string.IsNullOrWhiteSpace(e.Payload.Descrition)))
            {
                System.Console.WriteLine("There are entries that do not have a description.");
                @continue = false;
            }

            if (entries.Any(e => !CheckIfIssueExists(issueManagement, e.Payload.Query)))
            {
                System.Console.WriteLine("There are entries that cannot be found.");
                @continue = false;
            }

            if (entries.Any(e => e.Duration > maxTicketDuration))
            {
                System.Console.WriteLine($"There are entries that are longer than {maxTicketDuration.TotalHours} hours.");
                @continue = false;
            }

            if (!@continue)
            {
                System.Console.Write("There have been warnings. Continue (y/n) ?");
                var key = System.Console.ReadKey(false);
                System.Console.Write(" ");
                @continue = key.Key == ConsoleKey.Y;
                System.Console.WriteLine();
            }

            return @continue;
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

            return entries.ToList();
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
