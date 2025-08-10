using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Proxy;

[AttributeUsage(AttributeTargets.Interface)]
public class RpcProxyAspect : TypeAspect
{
    public Type TransportType { get; }

    public RpcProxyAspect(Type transportType)
    {
        this.TransportType = transportType;
    }

    [Template]
    private IRpcTransport transportTemplate = null!;

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        base.BuildAspect( builder );

        var proxy = builder
            .With(builder.Target.GetNamespace()!)
            .WithChildNamespace("Proxies")
            .IntroduceClass(builder.Target.Name[1..] + "Proxy",OverrideStrategy.New,
            buildType:
            type =>
            {
                type.Accessibility = Accessibility.Public;
            } );

        proxy.IntroduceField(nameof(transportTemplate), IntroductionScope.Instance, OverrideStrategy.Default, field =>
        {
            field.Accessibility = Accessibility.Public;
            field.Name = "_transport";
        });

        proxy.IntroduceConstructor(nameof(CtorTemplate), OverrideStrategy.Default, constructor =>
        {
            constructor.AddParameter("transport", TransportType);
        });

        proxy.ImplementInterface(builder.Target);

        foreach (var method in builder.Target.Methods)
        {
            var proxyMethod = proxy.IntroduceMethod(nameof(MethodTemplate), IntroductionScope.Instance, OverrideStrategy.Default, m =>
            {
                m.ReturnType = method.ReturnType;
                m.Accessibility = Accessibility.Public;

                foreach (var methodParameter in method.Parameters)
                {
                    m.AddParameter(methodParameter.Name, methodParameter.Type);
                }
                m.Name = method.Name;
            });

        }

        builder.RemoveAttributes(builder.Target.Attributes.First().Type);
    }

    [Template(Accessibility = Accessibility.Public)]
    public dynamic MethodTemplate()
    {
        var transport = (IRpcTransport)meta.This._transport!;
        return transport.Invoke(meta.Target.Method.Name, meta.Target.Parameters.ToValueArray(), meta.Target.Method.ReturnType.ToType());
    }

    [Template(Accessibility = Accessibility.Public)]
    public void CtorTemplate()
    {
        meta.This._transport = meta.Target.Parameters.First().Value;
    }
}