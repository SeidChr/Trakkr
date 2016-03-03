using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Trakkr.Core;

namespace Trakkr.Parse
{
    public class ShortTrackingFormatParser : IEventParser<ShortTrackingFormatPayload, string>
    {
        public IEnumerable<ITrakkrEntry<ShortTrackingFormatPayload>> Parse(string input)
        {
            var days = Regex.Split(input, @"^(?=\d{4}-\d{2}-\d{2}$)", RegexOptions.Multiline | RegexOptions.CultureInvariant);
            return days.SelectMany(d => ParseDay(d));
        }

        private IEnumerable<ITrakkrEntry<ShortTrackingFormatPayload>> ParseDay(string day)
        {
            yield break;
        }

        private static List<TrakkrEntry<string>> ParseEvents(FileInfo inputFile)
        {
            List<TrakkrEntry<string>> entries = new List<TrakkrEntry<string>>();

            
            using (var reader = inputFile.OpenText())
            {
                var fileContent = reader.ReadToEnd();

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
    }
}
