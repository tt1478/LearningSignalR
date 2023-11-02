// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Channels;

Console.WriteLine("Please specify the url of signalR Hub");

var url = Console.ReadLine();

Console.WriteLine("Please specify the access token");

var token = Console.ReadLine();


var hubConnection = new HubConnectionBuilder().WithUrl(url, options => {
    options.AccessTokenProvider = () => Task.FromResult(token);
}).Build();

hubConnection.On<string>("ReceiveMessage", message => Console.WriteLine($"SignalR Hub Message: {message}"));

try
{
    await hubConnection.StartAsync();
    while(true)
    {
        var message = string.Empty;
        var groupName = string.Empty;
        Console.WriteLine("Please specify the action:");
        Console.WriteLine("0 - broadcast to all");
        Console.WriteLine("1 - send to others");
        Console.WriteLine("2 - send to self");
        Console.WriteLine("3 - send to indivisual");
        Console.WriteLine("4 - send to a group");
        Console.WriteLine("5 - add user to a group");
        Console.WriteLine("6 - remove user from a group");
        Console.WriteLine("7 - trigger a server stream");
        Console.WriteLine("exit - Exit the program");
        var action = Console.ReadLine();
        switch(action)
        {
            case "0":
                Console.WriteLine("Please specify the message");
                message = Console.ReadLine();
                if(message?.Contains(";") ?? false)
                {
                    var channel = Channel.CreateBounded<string>(10);
                    await hubConnection.SendAsync("BroadcastStream", channel.Reader);
                    foreach(var item in message.Split(';'))
                    {
                        await channel.Writer.WriteAsync(item);
                    }
                    channel.Writer.Complete();
                }
                else
                {
                    await hubConnection.SendAsync("BroadcastMessage", message);
                }
                break;
            case "1":
                Console.WriteLine("Please specify the message");
                message = Console.ReadLine();
                await hubConnection.SendAsync("SendToOthers", message);
                break;
            case "2":
                Console.WriteLine("Please specify the message");
                message = Console.ReadLine();
                await hubConnection.SendAsync("SendToCallers", message);
                break;
            case "3":
                Console.WriteLine("Please specify the message");
                message = Console.ReadLine();
                Console.WriteLine("Please specify the connectionId");
                var connectionId = Console.ReadLine();
                await hubConnection.SendAsync("SendToIndivisual", connectionId, message);
                break;
            case "4":
                Console.WriteLine("Please specify the message");
                message = Console.ReadLine();
                Console.WriteLine("Please specify the group name");
                groupName = Console.ReadLine();
                await hubConnection.SendAsync("SendToGroup", groupName, message);
                break;
            case "5":
                Console.WriteLine("Please specify the group name");
                groupName = Console.ReadLine();
                await hubConnection.SendAsync("AddUserToGroup", groupName);
                break;
            case "6":
                Console.WriteLine("Please specify the group name");
                groupName = Console.ReadLine();
                await hubConnection.SendAsync("RemoveUserFromGroup", groupName);
                break;
            case "7":
                Console.WriteLine("Please specify the number of jobs");
                int numberOfJobs = Int32.Parse(Console.ReadLine() ?? "");  
                CancellationToken cancellationToken = new CancellationToken();
                var stream = hubConnection.StreamAsync<string>("TriggerStream", numberOfJobs, cancellationToken);
                await foreach(var reply in stream)
                {
                    Console.WriteLine(reply);
                }
                break;
            case "exit":
                break;
            default:
                Console.WriteLine("Invalid action specified");
                break;
        }
    }
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}


