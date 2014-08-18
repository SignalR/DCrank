using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentSenderUnitTests
    {
        [Fact]
        public async Task PongWritesJsonFormattedPongMessageToStreamWriter()
        {
            // arrange
            var expectedValue = new
            {
                Id = 50,
                Value = 60
            };

            var expectedResult = new Message()
            {
                Command = "pong",
                Value = JToken.FromObject(expectedValue)
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var agent = new AgentSender(streamWriter);

            // act
            await agent.Pong(expectedValue.Id, expectedValue.Value);

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }

        [Fact]
        public async Task LogWritesJsonFormattedLogMessageToStreamWriter()
        {
            // arrange
            var expectedValue = new
            {
                Id=70,
                Text = "Log message"
            };

            var expectedResult = new Message()
            {
                Command = "log",
                Value = JToken.FromObject(expectedValue)
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var agent = new AgentSender(streamWriter);

            // act
            await agent.Log(expectedValue.Id, expectedValue.Text);

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }

        [Fact]
        public async Task StatusWritesJsonFormattedStatusMessageToStreamWriter()
        {
            // arrange
            var expectedStatus = new StatusInformation
            {
                ConnectedCount = 90,
                DisconnectedCount = 100,
                ReconnectingCount = 110,
                TargetConnectionCount = 120
            };

            var expectedValue = new
            {
                Id = 80,
                StatusInformation = expectedStatus
            };

            var expectedResult = new Message()
            {
                Command = "status",
                Value = JToken.FromObject(expectedValue)
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var agent = new AgentSender(streamWriter);

            // act
            await agent.Status(
                expectedValue.Id,
                expectedValue.StatusInformation);

            // assert
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadLine();

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), result);
        }
    }
}
