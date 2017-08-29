using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Web;
using Trakkr.Core;
using Trakkr.Core.Events;
using Trakkr.Parse;
using Trakkr.YouTrack;
using YouTrackSharp.Issues;

namespace Trakkr.Console
{
    class Program
    {
        private const int RoundEntriesUpToXMinutes = 5;

        static void Main(string[] args)
        {
            if (args.Length < 0)
            {
                // click once argument data
                args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
            }

            var reset = false;
            if (args.Length <= 0)
            {
                System.Console.Write("R for RESET, Any other key to READ A FILE... ");
                var keyInfo = System.Console.ReadKey(true);
                reset = keyInfo.Key == ConsoleKey.R;
                System.Console.WriteLine();
            }

            if (reset || (args.Length > 0 && args[0] == "RESET"))
            {
                ResetSettings();
            }
            else
            {
                var connection = Authenticate();

                Interaction.Notice("USER: " + Properties.Settings.Default.Username);
                Interaction.Notice("HOST: " + Properties.Settings.Default.Host + ":" + Properties.Settings.Default.Port);

                FileInfo inputFile = null;
                if (args.Length >= 1)
                {
                    inputFile = new FileInfo(args[0]);
                }
                else
                {
                    var inputFileName = Interaction.GetText("Please enter InputFile");
                    inputFile = new FileInfo(inputFileName);
                    inputFile.Refresh();
                }

                var namePart = Path.Combine(Path.GetDirectoryName(inputFile.FullName), Path.GetFileNameWithoutExtension(inputFile.FullName));
                var logfileName = namePart + ".log";

                Interaction.Notice("LOG : " + logfileName);

                bool failure;
                do
                {
                    var entries = ParseEvents(inputFile);
                    entries = RoundUpEntries(entries, RoundEntriesUpToXMinutes);
                    var logger = new FileAppendLogger(logfileName,
                        $"Updating Workitems : {DateTime.Now.ToString("O")}");

                    failure = !UpdateWorkItems(connection, entries, logger);
                }
                while (failure && Interaction.Confirm("Retry?"));

                if (failure)
                {
                    // do not show message when retry was declined
                    return;
                }

                Interaction.Acknowledge("Done (press any key).");
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

        private static bool UpdateWorkItems(IConnection connection, IEnumerable<IEntry<ShortTrackingFormatPayload>> entries, FileAppendLogger logger)
        {
            var issueManagement = new IssueManagement(connection);

            var errors = !RequiredPreconditionsFulfilled(entries, issueManagement);
            var warnings = !OptionalPreconditionsFulfilled(entries);

            if (errors)
            {
                if (warnings)
                {
                    Interaction.Acknowledge("There have been errors and warnings. Cannot store entries.");
                    return false;
                }

                Interaction.Acknowledge("There have been errors. Cannot store entries.");
                return false;
            }

            if (warnings && !Interaction.Confirm("There have been warnings. Continue?"))
            {
                return false;
            }

            PrintEntryList(entries, issueManagement);

            if (!Interaction.Confirm("Please review the entries. Continue?"))
            {
                return false;
            }

            foreach (var trakkrEntry in entries)
            {
                var minutes = (int)Math.Round(trakkrEntry.Duration.TotalMinutes);
                System.Console.Write($"Saving: {trakkrEntry.Timestamp.ToShortDateString()} : {trakkrEntry.Payload.Query} ... : {minutes} Minutes ({trakkrEntry.Payload.Descrition}) ... ");

                var doc = "<workItem>"
                          + $"<date>{UnixTime.GetUnixTimestampMilliseconds(trakkrEntry.Timestamp)}</date>"
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

                System.Console.WriteLine();

                //https://confluence.jetbrains.com/display/YTD6/Get+Available+Work+Items+of+Issue

                // get all workitems
                // GET /rest/issue/{issue}/timetracking/workitem/
                //var result = connection.Get<object>($"issue/{trakkrEntry.Mark}/timetracking/workitem/");

                // add a workitem
                //connection.Post($"issue/{trakkrEntry.Mark}/timetracking/workitem", "<xml/>");
                //POST http://localhost:8081/rest/issue/HBR-63/timetracking/workitem
            }

            return true;
        }

        private static void PrintEntryList(IEnumerable<IEntry<ShortTrackingFormatPayload>> entries, IssueManagement issueManagement)
        {
            DateTime date = DateTime.MinValue;
            TimeSpan total = TimeSpan.Zero;

            foreach (var trakkrEntry in entries)
            {
                if (trakkrEntry.Timestamp.Date != date)
                {
                    date = trakkrEntry.Timestamp.Date;
                    total = TimeSpan.Zero;
                }

                var minutes = (int)Math.Round(trakkrEntry.Duration.TotalMinutes);

                total += TimeSpan.FromMinutes(minutes);

                dynamic issue = issueManagement.GetIssue(trakkrEntry.Payload.Query);
                string summary = issue.summary;
                Interaction.Notice($"{trakkrEntry.Timestamp.ToShortDateString()}\t// Task: {minutes}m\t// Day:{(int)total.TotalHours}h {total:mm}m\t// {trakkrEntry.Payload.Query}\t >> {summary} >> {trakkrEntry.Payload.Descrition}");
            }
        }

        private static bool RequiredPreconditionsFulfilled(IEnumerable<IEntry<ShortTrackingFormatPayload>> entries,
            IssueManagement issueManagement)
        {
            var @continue = true;

            var entryExistence = entries.Select(e => new { e.Payload.Query, Exists = CheckIfIssueExists(issueManagement, e.Payload.Query) }).ToList();
            if (entryExistence.Any(e => !e.Exists))
            {
                var ticketQueries = string.Join(", ", entryExistence.Where(e => !e.Exists).Select(e => e.Query));
                Interaction.Notice($"ERROR: There are entries that cannot be found: {ticketQueries}");
                @continue = false;
            }

            return @continue;
        }

        private static bool OptionalPreconditionsFulfilled(IEnumerable<IEntry<ShortTrackingFormatPayload>> entries)
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
                Interaction.Notice($"WARNING: There are entries that are very far in future or past. ({first.Payload.Query}: {first.Timestamp.ToShortDateString()}:{first.Timestamp.ToShortTimeString()})");
                @continue = false;
            }

            if (entries.Any(e => string.IsNullOrWhiteSpace(e.Payload.Descrition)))
            {
                Interaction.Notice("WARNING: There are entries that do not have a description.");
                @continue = false;
            }

            if (entries.Any(e => e.Duration > maxTicketDuration))
            {
                Interaction.Notice($"WARNING: There are entries that are longer than {maxTicketDuration.TotalHours} hours.");
                @continue = false;
            }

            var last = DateTime.MinValue;
            foreach (var current in entries.Select(entry => entry.Timestamp))
            {
                if (current < last)
                {
                    Interaction.Notice("WARNING: The entries are not propperly ordered.");
                    @continue = false;
                    break;
                }

                last = current;
            }

            return @continue;
        }

        private static IEnumerable<IEntry<TPayload>> RoundUpEntries<TPayload>(IEnumerable<IEntry<TPayload>> entries, int minutes) 
            => entries.Select(e => new EntryRoundUpView<TPayload>(e, minutes));

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
                Properties.Settings.Default.Host = Interaction.GetText("Youtrack Host");
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Username))
            {
                Properties.Settings.Default.Username = Interaction.GetText("Username");
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Password))
            {
                System.Console.WriteLine("Make sure nobody is watching!");
                Properties.Settings.Default.Password = Interaction.GetText("Password"); ;
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
