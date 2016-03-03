using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Core
{
    public interface ITrakkrEntry<T>
    {
        DateTime Day { get; }

        DateTime Start { get; }

        DateTime End { get; }

        TimeSpan Duration { get; }

        T Mark { get; }
    }
}
