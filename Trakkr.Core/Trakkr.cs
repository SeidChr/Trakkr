using System;
using System.Collections.Generic;
using System.Linq;

namespace Trakkr.Core
{
    public class Trakkr<T> : ITrakkr<T>
    {
        private DateTime lastEvent = DateTime.MaxValue;
        private TimeSpan duration = TimeSpan.Zero;
        private T lastMark = default(T);

        public TrakkrEntry<T> HandleStartEvent(DateTime dateTime, T mark)
        {
            UpdateEventTime(dateTime);
            var result = GetTrakkrEntry();
            lastMark = mark;
            return result;
        }

        public TrakkrEntry<T> HandleStopEvent(DateTime dateTime)
        {
            UpdateEventTime(dateTime);
            var result = GetTrakkrEntry();
            Reset();
            return result;
        }

        private void Reset()
        {
            lastEvent = DateTime.MaxValue;
            duration = TimeSpan.Zero;
            lastMark = default(T);
        }

        private TrakkrEntry<T> GetTrakkrEntry()
        {
            TrakkrEntry<T> result = null;
            if (duration > TimeSpan.Zero)
            {
                result = new TrakkrEntry<T>
                {
                    Day = lastEvent.Date,
                    Start = lastEvent,
                    End = lastEvent + duration,
                    Duration = duration,
                    Mark = lastMark
                };
            }

            return result;
        }

        private void UpdateEventTime(DateTime dateTime)
        {
            if (lastEvent == DateTime.MaxValue)
            {
                // first event
                lastEvent = dateTime;
            }
            else
            {
                duration = dateTime - lastEvent;
                lastEvent = dateTime;
            }
        }

        //static TrakkrEntry<T> sumEntry = null;

        //public static IEnumerable<TrakkrEntry<T>> Combine(IEnumerable<TrakkrEntry<T>> entries)
        //{
            
        //    foreach (var trakkrEntry in entries)
        //    {
        //        if (sumEntry == null)
        //        {
        //            sumEntry = trakkrEntry;
        //            yield break;
        //        }

        //        if (sumEntry.Mark.Equals(trakkrEntry.Mark))
        //        {
        //            sumEntry.End = trakkrEntry.End;
        //            sumEntry.Duration = sumEntry.End - sumEntry.Start;
        //            yield return sumEntry;
        //            sumEntry = null;
        //        }
        //    }
        //}


        public static IEnumerable<TrakkrEntry<T>> Merge(IEnumerable<TrakkrEntry<T>> entries)
        {
            return entries
                .GroupBy(e => e.Mark)
                .Select(g => g.Aggregate(new TrakkrEntry<T>(), (seed, adder) =>
                {
                    seed.Day = adder.Day;
                    seed.Duration += adder.Duration;
                    seed.Mark = adder.Mark;
                    seed.Start = DateTime.MaxValue;
                    seed.End = DateTime.MaxValue;
                    return seed;
                }));
        }
    }
}
