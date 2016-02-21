using System;
using System.ComponentModel;

namespace Trakkr.Model
{
    public interface IEvent : INotifyPropertyChanged
    {
        EventType Type { get; set; }
        DateTime UtcTimestamp { get; set; }
        string Description { get; set; }
    }
}