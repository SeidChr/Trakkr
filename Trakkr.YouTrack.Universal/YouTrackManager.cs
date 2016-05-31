namespace Trakkr.YouTrack.Universal
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
