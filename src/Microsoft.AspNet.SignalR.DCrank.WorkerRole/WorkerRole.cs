using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Win32;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Microsoft.AspNet.SignalR.DCrank.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private const string _testManagerHost = "http://perf-demo-mgr.perf-demo.net";
        private const string _tcpIpParameters = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\Tcpip\Parameters";
        private const string _maxUserPortValueName = "MaxUserPort";
        private const int _maxUserPortValue = 65534;

        public override void Run()
        {
            Trace.TraceInformation("Microsoft.AspNet.SignalR.DCrank.WorkerRole entry point called");

            while (true)
            {
                var path = Path.Combine(Environment.GetEnvironmentVariable("RoleRoot") + @"\", @"approot\crank.exe");
                var arguments = string.Format("/Mode:agent /ControllerUrl{0}", _testManagerHost);

                var crankStartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments
                };

                Trace.TraceInformation("Microsoft.AspNet.SignalR.DCrank.WorkerRole starting crank process");
                using (var process = Process.Start(crankStartInfo))
                {
                    if (process != null)
                    {
                        Trace.TraceInformation("Microsoft.AspNet.SignalR.DCrank.WorkerRole running");
                        process.WaitForExit();
                    }
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            var maxUserPort = (int?) Registry.GetValue(_tcpIpParameters, _maxUserPortValueName, null);

            if (!maxUserPort.HasValue || maxUserPort.Value != _maxUserPortValue)
            {
                Trace.TraceInformation("Microsoft.AspNet.SignalR.DCrank.WorkerRole setting MaxUserPort registry value");
                Registry.SetValue(_tcpIpParameters, _maxUserPortValueName, _maxUserPortValue, RegistryValueKind.DWord);

                var restartInfo = new ProcessStartInfo
                {
                    FileName = "shutdown.exe",
                    Arguments = "/r /t 0"
                };
                Trace.TraceInformation("Microsoft.AspNet.SignalR.DCrank.WorkerRole rebooting");
                Process.Start(restartInfo);
                while (true) ;
            }
            return base.OnStart();
        }
    }
}
