using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.ViewModels
{
    public class EventViewModel
    {
        public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;

        public DateTime LocaTimestamp
        {
            get {return UtcTimestamp.ToLocalTime(); }
            set { UtcTimestamp = value.ToUniversalTime(); }
        }

        //public IEnumerable<ITrackingRepositoryReference> TrackingRepositoryReferenceses { get; set; } = new ITrackingRepositoryReference[0];

        public string Description { get; set; }

    }

    //public interface ITrackingRepositoryReference
    //{
    //}
}
