using System;
using System.Collections.Generic;
using System.Linq;
using Trakkr.Core.Events;

namespace Trakkr.Core
{
    public class Trakkr<TPayload> : ITrakkr<TPayload>
    {
        private DateTime lastEvent = DateTime.MaxValue;
        private TimeSpan duration = TimeSpan.Zero;
        private TPayload lastPayload = default(TPayload);


        public IEntry<TPayload> HandleEvent(EventType type, DateTime timestamp, TPayload payload = default(TPayload))
        {
            UpdateEventTime(timestamp);
            IEntry<TPayload> result = GetTrakkrEntry();

            switch (type)
            {
                case EventType.Start:
                    lastPayload = payload;
                    break;
                case EventType.Stop:
                    Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            return result;
        }

        public IEntry<TPayload> HandleEvent(IEvent<TPayload> @event)
        {
            return HandleEvent(@event.Type, @event.Timestamp, @event.Payload);
        }

        public IEnumerable<IEntry<TPayload>> HandleEvents(IEnumerable<IEvent<TPayload>> events)
        {
            return events.Select(HandleEvent).Where(entry => entry != null);
        }

        private void Reset()
        {
            lastEvent = DateTime.MaxValue;
            duration = TimeSpan.Zero;
            lastPayload = default(TPayload);
        }

        private Entry<TPayload> GetTrakkrEntry()
        {
            Entry<TPayload> result = null;
            if (duration > TimeSpan.Zero)
            {
                result = new Entry<TPayload>
                {
                    Timestamp = lastEvent,
                    Duration = duration,
                    Payload = lastPayload
                };
            }

            return result;
        }

        private void UpdateEventTime(DateTime dateTime)
        {
            if (lastEvent == DateTime.MaxValue)
            {
                // first event after reset
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

        public static IEnumerable<Entry<TPayload>> Merge(IEnumerable<Entry<TPayload>> entries)
        {
            return entries
                .GroupBy(e => e.Payload)
                .Select(g => g.Aggregate(new Entry<TPayload>(), (seed, adder) =>
                {
                    seed.Duration += adder.Duration;
                    seed.Payload = adder.Payload;
                    if (seed.Timestamp == default(DateTime))
                    {
                        seed.Timestamp = adder.Timestamp;
                    }

                    return seed;
                }));
        }
    }
}
