using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Agent
    {
        private const string _url = "http://localhost:17063";
        private const string _fileName = "Microsoft.AspNet.SignalR.DCrank.Crank.exe";
        private readonly ControllerClient _client;

        public Agent()
        {
            _client = new ControllerClient(_url);
            _client.Initialize();
        }

        public async Task Run()
        {
            await _client.Start();

            var startInfo = new ProcessStartInfo()
            {
                FileName = _fileName,
                Arguments = "",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(startInfo))
            {
                var inputWriter = process.StandardInput;
                var outputReader = process.StandardOutput;

                process.WaitForExit();
            }
        }

    }
}
