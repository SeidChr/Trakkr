using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHttp.Http;

namespace Trakkr.YouTrack
{
    public interface IConnection : YouTrackSharp.Infrastructure.IConnection
    {
        HttpResponse PostXml(string command, string data);
    }
}
