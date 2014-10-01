using System.IO;
using System.Threading;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class WorkerReceiverUnitTests
    {
        private static StreamReader CreateMessageStream(params Message[] messages)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            foreach (var message in messages)
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            }

            streamWriter.Flush();
            stream.Position = 0;

            return new StreamReader(stream);
        }

        [Fact]
        public void DeserializesPingMessageAndCallsCallsPingMethodOnWorker()
        {
            // arrange
            var pingObject = new
            {
                Value = 40
            };

            var message = new Message()
            {
                Command = "ping",
                Value = JToken.FromObject(pingObject)
            };

            var streamReader = CreateMessageStream(message);

            var pingValue = 0;
            var pingResetEvent = new ManualResetEventSlim();

            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.Ping(It.IsAny<int>()))
                .Callback<int>(value =>
                {
                    pingValue = value;
                    pingResetEvent.Set();
                });

            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(pingResetEvent.WaitHandle.WaitOne(1000));
            Assert.Equal(pingObject.Value, pingValue);
        }

        [Fact]
        public void DeserializesConnectMessageAndCallsConnectMethodOnWorker()
        {
            // arrange
            var connectObject = new
            {
                TargetAddress = "http://localhost",
                NumberOfConnections = 10
            };

            var message = new Message()
            {
                Command = "connect",
                Value = JToken.FromObject(connectObject)
            };

            var streamReader = CreateMessageStream(message);

            string connectTargetAddress = null;
            var connectNumberOfConnections = 0;

            var connectResetEvent = new ManualResetEventSlim();
            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.Connect(It.IsAny<string>(), It.IsAny<int>()))
                .Callback<string, int>((targetAddress, numberOfConnections) =>
                {
                    connectTargetAddress = targetAddress;
                    connectNumberOfConnections = numberOfConnections;
                    connectResetEvent.Set();
                });

            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(connectResetEvent.WaitHandle.WaitOne(1000));
            Assert.Equal(connectObject.TargetAddress, connectTargetAddress);
            Assert.Equal(connectObject.NumberOfConnections, connectNumberOfConnections);
        }

        [Fact]
        public void DeserializesStartTestMessageAndCallsStartTestMethodOnWorker()
        {
            // arrange
            var startTestObject = new
            {
                SendInterval = 20,
                SendBytes = 30
            };

            var message = new Message()
            {
                Command = "starttest",
                Value = JToken.FromObject(startTestObject)
            };

            var streamReader = CreateMessageStream(message);

            var startTestSendInterval = 0;
            var startTestSendBytes = 0;

            var startTestResetEvent = new ManualResetEventSlim();
            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.StartTest(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((sendInterval, sendBytes) =>
                {
                    startTestSendInterval = sendInterval;
                    startTestSendBytes = sendBytes;
                    startTestResetEvent.Set();
                });

            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(startTestResetEvent.WaitHandle.WaitOne(1000));
            Assert.Equal(startTestObject.SendInterval, startTestSendInterval);
            Assert.Equal(startTestObject.SendBytes, startTestSendBytes);
        }

        [Fact]
        public void DeserializesStopMessageAndCallsStopMethodOnWorker()
        {
            // arrange
            var message = new Message()
            {
                Command = "stop",
                Value = null
            };

            var streamReader = CreateMessageStream(message);

            var stopResetEvent = new ManualResetEventSlim();
            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.Stop())
                .Callback(stopResetEvent.Set);

            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(stopResetEvent.WaitHandle.WaitOne(1000));
        }

        [Fact]
        public void StopsReadingWhenStopIsCalled()
        {
            // arrange
            var stopMessage = new Message()
            {
                Command = "stop",
                Value = null
            };

            var pingObject = new
            {
                Value = 40
            };

            var pingMessage = new Message()
            {
                Command = "ping",
                Value = JToken.FromObject(pingObject)
            };

            var streamReader = CreateMessageStream(stopMessage, pingMessage);

            var stopResetEvent = new ManualResetEventSlim();
            var pingResetEvent = new ManualResetEventSlim();

            var worker = new Mock<IWorker>();

            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            worker.Setup(instance => instance.Stop())
                .Callback(() =>
                {
                    workerReceiver.Stop();
                    stopResetEvent.Set();
                });

            worker.Setup(instance => instance.Ping(It.IsAny<int>()))
                .Callback<int>(value => pingResetEvent.Set());

            // act
            workerReceiver.Start();

            // assert
            Assert.True(stopResetEvent.WaitHandle.WaitOne(1000));
            Assert.False(pingResetEvent.WaitHandle.WaitOne(1000));
        }
    }
}
