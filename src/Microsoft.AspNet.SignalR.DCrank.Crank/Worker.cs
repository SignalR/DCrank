using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Worker
    {
        public Worker() { }

        public async Task Run()
        {
            while (true)
            {
                await Task.Delay(1000);
            }
        }
    }
}
