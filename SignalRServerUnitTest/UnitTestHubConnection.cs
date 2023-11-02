using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServerUnitTest
{
    public static class UnitTestHubConnection
    {
        public static async Task<HubConnection> CreateHubConnectionBuilder()
        {
            var server = await UnitTestHostService.GetInstanse();
            var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7142/learningHub",o => 
            {
                o.HttpMessageHandlerFactory = _ => server.TestServer.CreateHandler();
                o.AccessTokenProvider = () => Task.FromResult(server.AccessToken);
            })
            .Build();
            return connection;
        }
    }
}
