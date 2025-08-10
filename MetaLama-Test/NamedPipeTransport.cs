using System;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using Proxy;

namespace MetaLama_Test;

public class NamedPipeTransport : IRpcTransport
{
    private const string PipeName = "RpcTransportPipe";

    public object? Invoke(string methodName, object?[] args, Type returnType)
    {
        var request = new RpcRequest { Method = methodName, Args = args };
        var json = JsonSerializer.Serialize(request);
        var bytes = Encoding.UTF8.GetBytes(json);

        using var pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
        pipe.Connect();
        pipe.Write(bytes, 0, bytes.Length);
        pipe.Flush();

        var buffer = new byte[4096];
        int read = pipe.Read(buffer, 0, buffer.Length);
        if (read > 0)
        {
            var respJson = Encoding.UTF8.GetString(buffer, 0, read);
            var response = JsonSerializer.Deserialize<RpcResponse>(respJson);
            if (response?.Result is JsonElement resultEl)
            {
                return JsonSerializer.Deserialize(resultEl.GetRawText(), returnType);
            }
        }
        return null;
    }

    private class RpcRequest
    {
        public string Method { get; set; } = string.Empty;
        public object?[] Args { get; set; } = Array.Empty<object?>();
    }

    private class RpcResponse
    {
        public object? Result { get; set; }
    }
}