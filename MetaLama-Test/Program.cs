using System;

namespace MetaLama_Test;

sealed class Program
{
    static void Main(string[] args)
    {
        var transport = new RpcTransport();
        var proxy = new Proxies.MyServiceProxy(transport);

        var sum = proxy.Add(3, 4);
        Console.WriteLine("Add(3, 4) = " + sum);

        var echo = proxy.Echo("Hallo Welt");
        Console.WriteLine($"Echo: {echo}");

    }
}