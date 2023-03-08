namespace Sonar.Configuration;

public class ClientConfig : BaseConfig
{
    public string ServerAddress { get; set; }
    public uint Port { get; set; }
}