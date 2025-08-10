using System;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MetaLama_Test;

public class RpcPipeServer<TService>
{
    private readonly TService _service;
    private readonly string _pipeName;
    private bool _running;

    public RpcPipeServer(TService service, string pipeName = "RpcTransportPipe")
    {
        _service = service;
        _pipeName = pipeName;
    }

    public void Start()
    {
        _running = true;
        while (_running)
        {
            using var pipe = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            pipe.WaitForConnection();

            var buffer = new byte[4096];
            int read = pipe.Read(buffer, 0, buffer.Length);
            if (read > 0)
            {
                var reqJson = Encoding.UTF8.GetString(buffer, 0, read);
                var request = JsonSerializer.Deserialize<RpcRequest>(reqJson);
                object? result = null;
                if (request != null)
                {
                    var method = typeof(TService).GetMethod(request.Method);
                    if (method != null)
                    {
                        var parameters = method.GetParameters();
                        var typedArgs = new object?[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (request.Args != null && i < request.Args.Length && request.Args[i] != null)
                            {
                                if (request.Args[i] is JsonElement je)
                                {
                                    typedArgs[i] = JsonSerializer.Deserialize(je.GetRawText(), parameters[i].ParameterType);
                                }
                            }
                        }
                        result = method.Invoke(_service, typedArgs);
                    }
                }
                var response = new RpcResponse { Result = result };
                var respJson = JsonSerializer.Serialize(response);
                var respBytes = Encoding.UTF8.GetBytes(respJson);
                pipe.Write(respBytes, 0, respBytes.Length);
                pipe.Flush();
            }
        }
    }

    public void Stop() => _running = false;

    private class RpcRequest
    {
        public string Method { get; set; } = string.Empty;
        public object?[] Args { get; set; } = [];
    }

    private class RpcResponse
    {
        public object? Result { get; set; }
    }
}
