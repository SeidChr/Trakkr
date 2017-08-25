using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Core.Events
{
    public class EntryRoundUpView<TPayload> : IEntry<TPayload>
    {
        private readonly IEntry<TPayload> target;
        private readonly int minuteRoundUp;

        public EntryRoundUpView(IEntry<TPayload> target, int minuteRoundUp)
        {
            if (minuteRoundUp <= 0)
            {
                throw new ArgumentException("Must be greater than zero", nameof(minuteRoundUp));
            }

            this.target = target;
            this.minuteRoundUp = minuteRoundUp;
        }

        public TimeSpan OriginalDuration => target.Duration;

        public DateTime Timestamp => target.Timestamp;

        public TPayload Payload => target.Payload;

        public TimeSpan Duration
        {
            get
            {
                var originalMinutes = OriginalDuration.TotalMinutes;
                var targetMinutes = Math.Ceiling(originalMinutes / minuteRoundUp) * minuteRoundUp;

                var result = OriginalDuration;

                if (targetMinutes > originalMinutes)
                {
                    result = OriginalDuration.Add(TimeSpan.FromMinutes(targetMinutes - originalMinutes));
                }

                return result;
            }
        }
    }
}
