namespace Sonar;

public interface IListenable
{
    void Start();
    void Stop();
    void Poll();
}