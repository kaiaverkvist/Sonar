
using System.Net.Sockets;
using Sonar;
using Sonar.Builtin;
using Sonar.Configuration;

Console.WriteLine("Hello, World!");

var server = new ServerNetworkable(new ServerConfig{Port = 27014, Key = "A", MaxConnections = 10, PollRate = 140});
server.Router.Register<ClientConnected>((client, connected) =>
{
    Console.WriteLine($"{client}: Connected");
    client.Send(new NetworkError(SocketError.NoData));
});
server.Router.Register<ClientDisconnected>((client, disconnected) =>
{
    Console.WriteLine($"{client}: Disconnected: {disconnected.Reason}");
});
server.Router.Register<NetworkError>((client, error) =>
{
    Console.WriteLine($"{client}: Network Error: {error.Error}");
});

server.Start();

while (!Console.KeyAvailable)
{
    server.Poll();
}
