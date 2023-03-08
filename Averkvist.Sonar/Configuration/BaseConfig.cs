namespace Sonar.Configuration;

public abstract class BaseConfig
{
    public int PollRate { get; set; }
    
    public string Key { get; set; }
}