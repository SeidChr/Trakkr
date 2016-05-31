namespace Trakkr.YouTrack.Universal
{
    public interface IConnection
    {
        bool PostXml(string command, string data);
    }
}
