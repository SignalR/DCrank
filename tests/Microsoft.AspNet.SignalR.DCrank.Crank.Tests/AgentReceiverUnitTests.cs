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
        [Fact]
        public void CallsPongMethodWhenPongMessageIsWrittenToStream()
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            var pongId = 0;
            var pongValue = 0;
            var pongCts = new CancellationTokenSource();

            var agent = new Mock<IAgent>();
            agent.Setup(instance => instance.Pong(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((id, value) =>
                {
                    pongId = id;
                    pongValue = value;
                    pongCts.Cancel();
                });

            var streamReader = new StreamReader(stream);
            var agentReceiver = new AgentReceiver(streamReader, agent.Object);

            // act
            agentReceiver.Start();

            // assert
            Assert.True(pongCts.Token.WaitHandle.WaitOne(1000));
            Assert.Equal(pongObject.Id, pongId);
            Assert.Equal(pongObject.Value, pongValue);
        }

        [Fact]
        public void CallsLogMethodWhenLogMessageIsWrittenToStream()
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            var logId = 0;
            var logText = "";
            var logCts = new CancellationTokenSource();

            var agent = new Mock<IAgent>();
            agent.Setup(instance => instance.Log(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((id, text) =>
                {
                    logId = id;
                    logText = text;
                    logCts.Cancel();
                });

            var streamReader = new StreamReader(stream);
            var agentReceiver = new AgentReceiver(streamReader, agent.Object);

            // act
            agentReceiver.Start();

            // assert
            Assert.True(logCts.Token.WaitHandle.WaitOne(1000));
            Assert.Equal(logObject.Id, logId);
            Assert.Equal(logObject.Text, logText);
        }

        [Fact]
        public void CallsStatusMethodWhenStatusMessageIsWrittenToStream()
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

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.WriteLine(JsonConvert.SerializeObject(message));
            streamWriter.Flush();
            stream.Position = 0;

            var statusId = 0;
            StatusInformation statusInformation = null;
            var statusCts = new CancellationTokenSource();

            var agent = new Mock<IAgent>();
            agent.Setup(instance => instance.Status(It.IsAny<int>(), It.IsAny<StatusInformation>()))
                .Callback<int, StatusInformation>((id, status) =>
                {
                    statusId = id;
                    statusInformation = status;
                    statusCts.Cancel();
                });

            var streamReader = new StreamReader(stream);
            var agentReceiver = new AgentReceiver(streamReader, agent.Object);

            // act
            agentReceiver.Start();

            // assert
            Assert.True(statusCts.Token.WaitHandle.WaitOne(1000));
            Assert.Equal(statusObject.Id, statusId);
            Assert.Equal(JsonConvert.SerializeObject(statusObject.StatusInformation), JsonConvert.SerializeObject(statusInformation));
        }
    }
}
