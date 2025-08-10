namespace Proxy;

public interface IRpcTransport
{
    object? Invoke(string methodName, object?[] args, Type returnType);
}