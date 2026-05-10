using System;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace OSUI.Extensions;

public class DIExtension : MarkupExtension
{
    public Type Type { get; set; }

    public DIExtension() { }
    
    public DIExtension(Type type)
    {
        Type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return App.ServiceProvider.GetService(Type);
    }
}
