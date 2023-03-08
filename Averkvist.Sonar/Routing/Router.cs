using System.Security;
using Sonar.EfficientInvoker.Extensions;

namespace Sonar.Routing;

/// <summary>
/// Router translates network messages into calls to registered handlers. 
/// </summary>
public class Router
{
    private Dictionary<string, Type> _typeLookup = new();
    private Dictionary<Type, HashSet<Delegate>> _handlers = new();
    public uint HandlerCount => (uint)_handlers.Count;
    
    public Router()
    {
    }

    public void Register<T>(Action<Client, T> handler)
    {
        var t = typeof(T);

        if (!_handlers.ContainsKey(t))
            _handlers[t] = new HashSet<Delegate>();
        
        if(!_typeLookup.ContainsKey(t.ToString()))
            _typeLookup.Add(t.ToString(), t);
        
        _handlers[t].Add(handler);
    }

    public int Trigger(Client client, object? message)
    {
        if (!_typeLookup.ContainsValue(message.GetType()))
            throw new SecurityException($"Invalid trigger type. Register first. Classname: {message.GetType()}");
        
        int callCount = 0;

        var t = message.GetType();
        foreach (var handlerPair in _handlers.Where(h => h.Key == t))
        {
            if(message == null)
                continue;

            var actions = handlerPair.Value;
            foreach (var @delegate in actions)
            {
                var invoker = @delegate.GetInvoker();
                invoker.Invoke(@delegate, client, message);
                
                callCount++;
            }
        }
        
        return callCount;
    }

    public int ManualTrigger<T>(Client client, T message)
    {
        int callCount = 0;
        
        foreach (var handlerPair in _handlers.Where(h => h.Key == typeof(T)))
        {
            if(message == null)
                continue;

            var actions = handlerPair.Value;
            foreach (var @delegate in actions)
            {
                var invoker = @delegate.GetInvoker();
                invoker.Invoke(@delegate, client, message);
                callCount++;
            }
        }
        
        return callCount;
    }
    
    public bool IsValidClassName(string className)
    {
        return _typeLookup.ContainsKey(className);
    }
    
    public Dictionary<Type, HashSet<Delegate>> GetHandlerDictionary()
    {
        return _handlers;
    }

    public Type? PayloadTypeLookup(string className)
    {
        var messageType = Extensions.TypeExtensions.GetTypeFromAssemblies(className);
        if(_handlers.ContainsKey(messageType))
            return _handlers.FirstOrDefault(e => e.Key == messageType).Key;
        
        return null;
    }
}