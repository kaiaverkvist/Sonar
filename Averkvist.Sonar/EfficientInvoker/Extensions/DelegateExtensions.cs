namespace Sonar.EfficientInvoker.Extensions;

public static class DelegateExtensions
{
    public static EfficientInvoker GetInvoker(this Delegate del)
    {
        return EfficientInvoker.ForDelegate(del);
    }
}