using System.Collections.ObjectModel;
using Trakkr.Core;
using Trakkr.Core.Events;

namespace Trakkr.Universal.ViewModel
{
    public class EventCollection : ObservableCollection<IEvent<IRepositoryPayload>>
    {
    }
}