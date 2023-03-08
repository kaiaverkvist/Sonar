# Sonar
A library to make basic game servers very simple. Includes a basic message serializer along with routing.

## Minimal Client
```cs
using Sonar;
using Sonar.Builtin;
using Sonar.Configuration;

var client = new ClientNetworkable(new ClientConfig{ServerAddress = "localhost", Port = 27014, Key = "A", PollRate = 140});
client.Router.Register<ClientConnected>((client, connected) =>
{
    Console.WriteLine($"{client}: Connected");
});
client.Router.Register<ClientDisconnected>((client, disconnected) =>
{
    Console.WriteLine($"{client}: Disconnected: {disconnected.Reason}");
});
client.Router.Register<NetworkError>((client, error) =>
{
    Console.WriteLine($"{client}: Network Error: {error.Error}");
});

client.Start();
client.Connect("localhost", 27014);

while (!Console.KeyAvailable)
{
    client.Poll();
}
```

## Minimal Server
```cs
using System.Net.Sockets;
using Sonar;
using Sonar.Builtin;
using Sonar.Configuration;

var server = new ServerNetworkable(new ServerConfig{Port = 27014, Key = "A", MaxConnections = 10, PollRate = 140});
server.Router.Register<ClientConnected>((client, connected) =>
{
    Console.WriteLine($"{client}: Connected");
    client.Send(new NetworkError(SocketError.NoData)); // This should be received by the server, upon connecting!
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
```