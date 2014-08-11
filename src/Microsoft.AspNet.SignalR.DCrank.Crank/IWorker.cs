using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public interface IWorker
    {
        Task Ping(int value);
        Task Connect(string targetAddress, int numberOfConnections);
        Task StartTest(int sendInterval, int sendBytes);
        Task Stop();
    }
}
