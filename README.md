# MetaLama-Test

This project demonstrates the use of Metalama aspects to generate RPC proxy classes for interfaces in C#. It avoids the use of `DispatchProxy` and instead leverages a custom Metalama aspect to automatically generate proxy implementations that delegate method calls to a transport layer.

## Features
- **Metalama Aspect**: Uses a custom `RpcProxyAspect` to generate proxy classes for interfaces.
- **Transport Abstraction**: All RPC calls are routed through an `IRpcTransport` implementation.
- **No DispatchProxy**: The solution does not use .NET's `DispatchProxy`.
- **Automatic Proxy Generation**: Proxy classes are generated at compile time for any interface annotated with the aspect.

## Structure
- `MetaLama-Test/IMyService.cs`: Example service interface.
- `MetaLama-Test/Program.cs`: Example usage of the generated proxy.
- `MetaLama-Test/RpcTransport.cs`: Example transport implementation.
- `Proxy/IRpcTransport.cs`: Transport abstraction interface.
- `Proxy/RpcProxyAspect.cs`: Metalama aspect for proxy generation.

## How It Works
1. Annotate your service interface with `[RpcProxyAspect(typeof(MyTransport))]`.
2. The aspect generates a proxy class (e.g., `MyServiceProxy`) implementing the interface.
3. The proxy delegates all method calls to the provided transport.
4. Use the generated proxy in your application by passing an `IRpcTransport` instance.

## Example
```csharp
var transport = new RpcTransport();
var proxy = new Proxies.MyServiceProxy(transport);
var result = proxy.Add(1, 2);
```

## Requirements
- .NET 9.0 or later
- [Metalama](https://metalama.net/)

## Build & Run
1. Restore NuGet packages.
2. Build the solution.
3. Run the `MetaLama-Test` project.

## License
GPL-3.0 License
