using System;

namespace Trakkr.Core
{
    public class TrakkrEntry<T> : ITrakkrEntry<T>
    {
        public DateTime Day { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public TimeSpan Duration { get; set; }

        public T Mark { get; set; }
    }
}
