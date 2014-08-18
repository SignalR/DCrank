using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public interface IAgent
    {
        Task Pong(int id, int value);
        Task Log(int id, string text);
        Task Status(int id, StatusInformation statusInformation);
    }
}
