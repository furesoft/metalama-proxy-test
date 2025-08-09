using Proxy;

namespace MetaLama_Test;

[RpcProxyAspect(typeof(RpcTransport))]
public interface IMyService
{
    int Add(int a, int b);
    string Echo(string text);
}