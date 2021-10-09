using System.Threading.Tasks;

namespace MyLights.Bridges.Udp2
{
    public interface IClient
    {
        bool IsConnected { get; }
        bool IsListening { get; }

        event MessageReceivedEventHandler MessageReceived;

        Task Connect();
        Task<int> SendMessage(string message);
        Task<int> SendMessage(string verb, string resource, string data = null);
    }
}