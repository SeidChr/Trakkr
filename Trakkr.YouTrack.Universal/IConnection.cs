using System.Net;
using System.Threading.Tasks;

namespace Trakkr.YouTrack.Universal
{
    public interface IConnection
    {
        void Authenticate(NetworkCredential credentials);
        void Authenticate(string username, string password);
        void Logout();
        bool IsAuthenticated { get; }
        Task<string> GetCurrentAuthenticatedUser();
    }
}