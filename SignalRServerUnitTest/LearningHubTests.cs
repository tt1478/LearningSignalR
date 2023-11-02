using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SignalRServer.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRServerUnitTest
{
    public class LearningHubTests
    {
        [Fact]
        public async Task BroadcastMessage_IfMessageIsValid()
        {
            //Arrange
            var connection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var result = string.Empty;
            //Act
            connection.On<string>("ReceiveMessage", msg =>
            {
                result = msg;
            });
            await connection.StartAsync();
            await connection.InvokeAsync("BroadcastMessage", message);
            Task.Delay(500).Wait();
            //Assert
            Assert.Equal(result, message);
        }
        [Fact]
        public async Task BroadcastMessage_IfMessageNotValid()
        {
            //Arrange
            var connection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = string.Empty;
            var result = string.Empty;
            //Act
            connection.On<string>("ReceiveMessage", msg =>
            {
                result = msg;
            });
            await connection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await connection.InvokeAsync("BroadcastMessage", message));
        }
        [Fact]
        public async Task SendToOthers_IfMessageIsValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var secondConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var firstResult = string.Empty;
            var secondResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            secondConnection.On<string>("ReceiveMessage", msg =>
            {
                secondResult = msg;
            });
            await firstConnection.StartAsync();
            await secondConnection.StartAsync();
            await firstConnection.InvokeAsync("SendToOthers", message);
            Task.Delay(500).Wait();
            //Assert
            Assert.Equal(string.Empty, firstResult);
            Assert.Equal(secondResult, message);
        }
        [Fact]
        public async Task SendToOthers_IfMessageNotValid()
        {
            //Arrange
            var connection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = string.Empty;
            var res = string.Empty;
            //Act
            connection.On<string>("ReceiveMessage", msg =>
            {
                res = msg;
            });
            await connection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await connection.InvokeAsync("SendToOthers", message));
        }
        [Fact]
        public async Task SendToCallers_IfMessageIsValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var secondConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var firstResult = string.Empty;
            var secondResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            secondConnection.On<string>("ReceiveMessage", msg =>
            {
                secondResult = msg;
            });
            await firstConnection.StartAsync();
            await secondConnection.StartAsync();
            await firstConnection.InvokeAsync("SendToCallers", message);
            Task.Delay(500).Wait();
            //Assert
            Assert.Equal(message, firstResult);
            Assert.Equal(string.Empty, secondResult);
        }
        [Fact]
        public async Task SendToCallers_IfMessageNotValid()
        {
            //Arrange
            var connection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = string.Empty;
            var res = string.Empty;
            //Act
            connection.On<string>("ReceiveMessage", msg =>
            {
                res = msg;
            });
            await connection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await connection.InvokeAsync("SendToCallers", message));
        }
        [Fact]
        public async Task SendToIndivisual_IfMessageIsValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var secondConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var firstResult = string.Empty;
            var secondResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            secondConnection.On<string>("ReceiveMessage", msg =>
            {
                secondResult = msg;
            });
            await firstConnection.StartAsync();
            await secondConnection.StartAsync();
            await firstConnection.InvokeAsync("SendToIndivisual", secondConnection.ConnectionId, message);
            Task.Delay(500).Wait();
            //Assert
            Assert.Equal(string.Empty, firstResult);
            Assert.Equal(message, secondResult);
        }
        [Fact]
        public async Task SendToIndivisual_IfMessageNotValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var secondConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = string.Empty;
            var res = string.Empty;
            //Act
            secondConnection.On<string>("ReceiveMessage", msg =>
            {
                res = msg;
            });
            await firstConnection.StartAsync();
            await secondConnection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await firstConnection.InvokeAsync("SendToIndivisual", secondConnection.ConnectionId, message));
        }
        [Fact]
        public async Task SendToIndivisual_IfConnectionIdNotValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var secondConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var message = string.Empty;
            var res = string.Empty;
            //Act
            secondConnection.On<string>("ReceiveMessage", msg =>
            {
                res = msg;
            });
            await firstConnection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await firstConnection.InvokeAsync("SendToIndivisual", secondConnection.ConnectionId, message));
        }
        [Fact]
        public async Task SendToGroup_IfGroupNameAndMessageAreValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = "firstGroup";
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            await firstConnection.InvokeAsync("AddUserToGroup", groupName);
            Task.Delay(500).Wait();
            await firstConnection.InvokeAsync("SendToGroup", groupName, message);
            Task.Delay(500).Wait();
            //Assert
            Assert.Contains(message, firstResult);
        }
        [Fact]
        public async Task SendToGroup_IfGroupNameNotValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = string.Empty;
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await firstConnection.InvokeAsync("SendToGroup", groupName, message));
        }
        [Fact]
        public async Task SendToGroup_IfMessageNotValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = "firstGroup";
            var message = string.Empty;
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await firstConnection.InvokeAsync("SendToGroup", groupName, message));
        }
        [Fact]
        public async Task AddUserToGroup_IfGroupNameIsValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = "firstGroup";
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            await firstConnection.InvokeAsync("AddUserToGroup", groupName);
            Task.Delay(500).Wait();
            //Assert
            Assert.Contains(groupName, firstResult);
        }
        [Fact]
        public async Task AddUserToGroup_IfGroupNameNotValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = string.Empty;
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await firstConnection.InvokeAsync("AddUserToGroup", groupName));
        }
        [Fact]
        public async Task RemoveUserFromGroup_IfGroupNameIsValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = "firstGroup";
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            await firstConnection.InvokeAsync("RemoveUserFromGroup", groupName);
            Task.Delay(500).Wait();
            //Assert
            Assert.Contains(groupName, firstResult);
        }
        [Fact]
        public async Task RemoveUserFromGroup_IfGroupNameNotValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var groupName = string.Empty;
            var firstResult = string.Empty;
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult = msg;
            });
            await firstConnection.StartAsync();
            //Assert
            await Assert.ThrowsAsync<HubException>(async () => await firstConnection.InvokeAsync("RemoveUserFromGroup", groupName));
        }
        [Fact]
        public async Task BroadcastStream_IfMessageIsValid()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var channel = Channel.CreateBounded<string>(10);
            var message = "message 1;message 2;message 3";
            var firstResult = new List<string>();
            //Act
            firstConnection.On<string>("ReceiveMessage", msg =>
            {
                firstResult.Add(msg);
            });
            await firstConnection.StartAsync();
            await firstConnection.SendAsync("BroadcastStream", channel.Reader);
            foreach (var item in message.Split(';'))
            {
                await channel.Writer.WriteAsync(item);
            }
            channel.Writer.Complete();
            Task.Delay(500).Wait();
            //Assert
            Assert.Equal(3, firstResult.Count);
            Assert.Contains("Server received message 1", firstResult);
            Assert.Contains("Server received message 2", firstResult);
            Assert.Contains("Server received message 3", firstResult);
        }
        [Fact]
        public async Task TriggerStream()
        {
            //Arrange
            var firstConnection = await UnitTestHubConnection.CreateHubConnectionBuilder();
            var numberOfJobs = 10;
            var results = new List<string>();
            var cancellationToken = new CancellationTokenSource();
            //Act
            await firstConnection.StartAsync();
            var stream = firstConnection.StreamAsync<string>("TriggerStream", numberOfJobs, cancellationToken.Token);
            await foreach (var item in stream)
            {
                results.Add(item);
            }
            Task.Delay(500).Wait();
            //Assert
            Assert.Equal(11, results.Count);
            Assert.Equal("Job 0 executed successfully", results[0]);
            Assert.Equal("Job 10 executed successfully", results[10]);
        }
    }
}
