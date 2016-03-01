using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Trakkr.Core;

namespace Trakkr.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TrakkrEntry<string>> entries = new List<TrakkrEntry<string>>();

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
            }

            System.Console.ReadLine();
        }
    }
}
