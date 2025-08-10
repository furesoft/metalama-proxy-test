using System;
using System.Threading.Tasks;

namespace MetaLama_Test;

class MyServiceImpl : IMyService
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public string Echo(string message)
    {
        return message;
    }
}

sealed class Program
{
    static void Main(string[] args)
    {
        var service = new MyServiceImpl();
        var server = new RpcPipeServer<IMyService>(service);

        var serverTask = Task.Run(() => server.Start());

        Task.Delay(200).Wait();

        var transport = new NamedPipeTransport();
        var proxy = new Proxies.MyServiceProxy(transport);

        var sum = proxy.Add(3, 4);
        Console.WriteLine("Add(3, 4) = " + sum);

        var echo = proxy.Echo("Hallo Welt");
        Console.WriteLine($"Echo: {echo}");

        // Server beenden
        server.Stop();
        serverTask.Wait(500);
    }
}