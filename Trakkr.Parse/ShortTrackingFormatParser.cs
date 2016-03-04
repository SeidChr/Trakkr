using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Trakkr.Core;
using Trakkr.Core.Events;

namespace Trakkr.Parse
{
    public class ShortTrackingFormatParser : IEventParser<ShortTrackingFormatPayload, string>
    {
        public IEnumerable<IEvent<ShortTrackingFormatPayload>> Parse(string input)
        {
            return Regex
                .Split(input, @"^(?=\d{4}-\d{2}-\d{2}\s+)", RegexOptions.Multiline | RegexOptions.CultureInvariant)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .SelectMany(ParseDay);
        }

        private static IEnumerable<IEvent<ShortTrackingFormatPayload>> ParseDay(string dayString)
        {
            var reader = new StringReader(dayString);

            // first line is date
            var dateLine = reader.ReadLine();
            DateTime date;
            if (DateTime.TryParseExact(dateLine, "yyyy-MM-dd", null, DateTimeStyles.None, out date))
            {
                date = date.Date;
                string timeLine;
                string[] timeSplit;
                while ((timeLine = reader.ReadLine()) != null)
                {
                    timeSplit = timeLine.Split(new []{' '}, 3);
                
                    if (timeSplit.Length >= 2)
                    {
                        var time = DateTime.ParseExact(timeSplit[0], "HHmm", null, DateTimeStyles.None).TimeOfDay;
                        var eventTime = date.Add(time);
                        var ticket = timeSplit[1];
                        var description = string.Empty;
                        if (timeSplit.Length > 2)
                        {
                            description = timeSplit[2];
                        }
                        var isStopEvent = string.Equals(ticket, "stop");
                        yield return new Event<ShortTrackingFormatPayload>()
                        {
                            Type = isStopEvent ? EventType.Stop : EventType.Start,
                            Timestamp = eventTime,
                            Payload = new ShortTrackingFormatPayload()
                            {
                                Query = ticket,
                                Descrition = description,
                            },
                        };
                    }
                }
            }
        }
    }
}
