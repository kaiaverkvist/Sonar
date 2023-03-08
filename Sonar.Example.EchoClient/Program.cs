using Sonar;
using Sonar.Builtin;
using Sonar.Configuration;

Console.WriteLine("Hello, World!");

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