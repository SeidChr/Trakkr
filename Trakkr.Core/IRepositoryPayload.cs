using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Core
{
    public interface IRepositoryPayload
    {
        string Query { get; set; }

        string TicketId { get; set; }

        string WorkItemId { get; set; }
    }
}
