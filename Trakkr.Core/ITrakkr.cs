using System;

namespace Trakkr.Core
{
    public interface ITrakkr<T>
    {
        TrakkrEntry<T> HandleStartEvent(DateTime dateTime, T mark);
        TrakkrEntry<T> HandleStopEvent(DateTime dateTime);
    }
}