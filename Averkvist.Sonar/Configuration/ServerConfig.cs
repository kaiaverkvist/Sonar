namespace Sonar.Configuration;

public class ServerConfig : BaseConfig
{
    public uint Port { get; set; }
    public uint MaxConnections { get; set; }
}