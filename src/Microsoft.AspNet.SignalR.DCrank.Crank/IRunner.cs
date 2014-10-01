using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public interface IRunner
    {
        Task PongWorker(int workerId, int value);

        Task LogAgent(string format, params object[] arguments);

        Task LogWorker(int workerId, string format, params object[] arguments);
    }
}
