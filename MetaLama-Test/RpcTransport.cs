using System;
using Proxy;

namespace MetaLama_Test;

public class RpcTransport : IRpcTransport
{
    public object? Invoke(string methodName, object?[] args)
    {
        // Eigene Logik, z.B. REST-Call
        Console.WriteLine($"RPC: {methodName}({string.Join(", ", args)})");
        return args[0]; // Dummy
    }
}