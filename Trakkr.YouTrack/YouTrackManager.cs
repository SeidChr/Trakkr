using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTrackSharp.Infrastructure;

namespace Trakkr.YouTrack
{
    public static class YouTrackManager
    {
        public static IConnection GetConnection(string host, string username, string password)
        {
            var connection = new Connection("youtrack.neveling.net", 443, true);
            connection.Authenticate(username, password);
            return connection;
        }
    }
}
