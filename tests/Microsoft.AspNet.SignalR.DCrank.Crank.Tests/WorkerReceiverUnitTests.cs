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
        [Fact]
        public void CallsPingMethodWhenPingMessageIsWrittenToStream()
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            var pingValue = 0;
            var pingCts = new CancellationTokenSource();

            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.Ping(It.IsAny<int>()))
                .Callback<int>(value =>
                {
                    pingValue = value;
                    pingCts.Cancel();
                });

            var streamReader = new StreamReader(stream);
            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(pingCts.Token.WaitHandle.WaitOne(1000));
            Assert.Equal(pingObject.Value, pingValue);
        }

        [Fact]
        public void CallsConnectMethodWhenConnectMessageIsWrittenToStream()
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            string connectTargetAddress = null;
            var connectNumberOfConnections = 0;

            var connectCts = new CancellationTokenSource();
            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.Connect(It.IsAny<string>(), It.IsAny<int>()))
                .Callback<string, int>((targetAddress, numberOfConnections) =>
                {
                    connectTargetAddress = targetAddress;
                    connectNumberOfConnections = numberOfConnections;
                    connectCts.Cancel();
                });

            var streamReader = new StreamReader(stream);
            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(connectCts.Token.WaitHandle.WaitOne(1000));
            Assert.Equal(connectObject.TargetAddress, connectTargetAddress);
            Assert.Equal(connectObject.NumberOfConnections, connectNumberOfConnections);
        }

        [Fact]
        public void CallsStartTestMethodWhenStartTestMessageIsWrittenToStream()
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            var startTestSendInterval = 0;
            var startTestSendBytes = 0;

            var startTestCts = new CancellationTokenSource();
            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.StartTest(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((sendInterval, sendBytes) =>
                {
                    startTestSendInterval = sendInterval;
                    startTestSendBytes = sendBytes;
                    startTestCts.Cancel();
                });

            var streamReader = new StreamReader(stream);
            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(startTestCts.Token.WaitHandle.WaitOne(1000));
            Assert.Equal(startTestObject.SendInterval, startTestSendInterval);
            Assert.Equal(startTestObject.SendBytes, startTestSendBytes);
        }

        [Fact]
        public void CallsStopMethodWhenStopMessageIsWrittenToStream()
        {
            // arrange
            var message = new Message()
            {
                Command = "stop",
                Value = null
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            var stopCts = new CancellationTokenSource();
            var worker = new Mock<IWorker>();
            worker.Setup(instance => instance.Stop())
                .Callback(stopCts.Cancel);

            var streamReader = new StreamReader(stream);
            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            // act
            workerReceiver.Start();

            // assert
            Assert.True(stopCts.Token.WaitHandle.WaitOne(1000));
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(stopMessage));
            streamWriter.WriteLine(JsonConvert.SerializeObject(pingMessage));
            streamWriter.Flush();
            stream.Position = 0;

            var stopCts = new CancellationTokenSource();
            var pingCts = new CancellationTokenSource();

            var worker = new Mock<IWorker>();

            var streamReader = new StreamReader(stream);
            var workerReceiver = new WorkerReceiver(streamReader, worker.Object);

            worker.Setup(instance => instance.Stop())
                .Callback(() =>
                {
                    workerReceiver.Stop();
                    stopCts.Cancel();
                });

            worker
                .Setup(instance => instance.Ping(It.IsAny<int>()))
                .Callback<int>(value => pingCts.Cancel());

            // act
            workerReceiver.Start();

            // assert
            Assert.True(stopCts.Token.WaitHandle.WaitOne(1000));
            Assert.False(pingCts.Token.WaitHandle.WaitOne(1000));
        }
    }
}
