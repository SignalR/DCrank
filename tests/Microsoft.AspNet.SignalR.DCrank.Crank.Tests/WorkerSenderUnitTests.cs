using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class WorkerSenderUnitTests
    {
        [Fact]
        public async Task PingWritesJsonFormattedPingMessageToStreamWriter()
        {
            // arrange
            var expectedValue = new
            {
                Value = 40
            };

            var expectedResult = new Message()
            {
                Command = "ping",
                Value = JToken.FromObject(expectedValue)
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var worker = new WorkerSender(streamWriter);

            // act
            await worker.Ping(expectedValue.Value);

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }

        [Fact]
        public async Task ConnectWritesJsonFormattedConnectMessageToStreamWriter()
        {
            // arrange
            var expectedValue = new
            {
                TargetAddress = "http://localhost",
                NumberOfConnections = 10
            };

            var expectedResult = new Message()
            {
                Command = "connect",
                Value = JToken.FromObject(expectedValue)
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var worker = new WorkerSender(streamWriter);

            // act
            await worker.Connect(
                expectedValue.TargetAddress,
                expectedValue.NumberOfConnections);

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }

        [Fact]
        public async Task StartTestWritesJsonFormattedStartTestMessageToStreamWriter()
        {
            // arrange
            var expectedValue = new
            {
                SendInterval = 20,
                SendBytes = 30
            };

            var expectedResult = new Message()
            {
                Command = "starttest",
                Value = JToken.FromObject(expectedValue)
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var worker = new WorkerSender(streamWriter);

            // act
            await worker.StartTest(
                expectedValue.SendInterval,
                expectedValue.SendBytes);

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }

        [Fact]
        public async Task StopWritesJsonFormattedStopMessageToStreamWriter()
        {
            // arrange
            var expectedResult = new Message()
            {
                Command = "stop",
                Value = null
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var worker = new WorkerSender(streamWriter);

            // act
            await worker.Stop();

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }
    }
}
