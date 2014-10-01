using System.IO;
using System.Threading;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentReceiverUnitTests
    {
        private static StreamReader CreateMessageStream(Message message)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            return new StreamReader(stream);
        }

        [Fact]
        public void DeserializesPongMessageAndCallsPongMethodOnAgent()
        {
            // arrange
            var pongObject = new
            {
                Id = 50,
                Value = 60
            };

            var message = new Message
            {
                Command = "pong",
                Value = JToken.FromObject(pongObject)
            };

            var streamReader = CreateMessageStream(message);

            var pongId = 0;
            var pongValue = 0;
            var pongResetEvent = new ManualResetEventSlim();

            var agent = new Mock<IAgent>();
            agent.Setup(instance => instance.Pong(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((id, value) =>
                {
                    pongId = id;
                    pongValue = value;
                    pongResetEvent.Set();
                });

            var agentReceiver = new AgentReceiver(streamReader, agent.Object);

            // act
            agentReceiver.Start();

            // assert
            Assert.True(pongResetEvent.WaitHandle.WaitOne(1000));
            Assert.Equal(pongObject.Id, pongId);
            Assert.Equal(pongObject.Value, pongValue);
        }

        [Fact]
        public void DeserializesLogMessageAndCallsLogMethodOnAgent()
        {
            // arrange
            var logObject = new
            {
                Id = 70,
                Text = "Log message"
            };

            var message = new Message
            {
                Command = "log",
                Value = JToken.FromObject(logObject)
            };

            var streamReader = CreateMessageStream(message);

            var logId = 0;
            var logText = "";
            var logResetEvent = new ManualResetEventSlim();

            var agent = new Mock<IAgent>();
            agent.Setup(instance => instance.Log(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((id, text) =>
                {
                    logId = id;
                    logText = text;
                    logResetEvent.Set();
                });

            var agentReceiver = new AgentReceiver(streamReader, agent.Object);

            // act
            agentReceiver.Start();

            // assert
            Assert.True(logResetEvent.WaitHandle.WaitOne(1000));
            Assert.Equal(logObject.Id, logId);
            Assert.Equal(logObject.Text, logText);
        }

        [Fact]
        public void DeserializesStatusMessageAndCallsStatusMethodOnAgent()
        {
            // arrange
            var statusObject = new
            {
                Id = 80,
                StatusInformation = new StatusInformation
                {
                    ConnectedCount = 90,
                    DisconnectedCount = 100,
                    ReconnectingCount = 110,
                    TargetConnectionCount = 120
                }
            };

            var message = new Message
            {
                Command = "status",
                Value = JToken.FromObject(statusObject)
            };

            var streamReader = CreateMessageStream(message);

            var statusId = 0;
            StatusInformation statusInformation = null;
            var statusResetEvent = new ManualResetEventSlim();

            var agent = new Mock<IAgent>();
            agent.Setup(instance => instance.Status(It.IsAny<int>(), It.IsAny<StatusInformation>()))
                .Callback<int, StatusInformation>((id, status) =>
                {
                    statusId = id;
                    statusInformation = status;
                    statusResetEvent.Set();
                });

            var agentReceiver = new AgentReceiver(streamReader, agent.Object);

            // act
            agentReceiver.Start();

            // assert
            Assert.True(statusResetEvent.WaitHandle.WaitOne(1000));
            Assert.Equal(statusObject.Id, statusId);
            Assert.Equal(JsonConvert.SerializeObject(statusObject.StatusInformation), JsonConvert.SerializeObject(statusInformation));
        }
    }
}
