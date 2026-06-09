using System.Windows.Markup;

namespace OSUI.Extensions;

public class DIExtension : MarkupExtension
{
    private Type? Type { get; set; }

    public DIExtension() { }
    
    public DIExtension(Type type)
    {
        Type = type;
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (Type != null) return App.ServiceProvider.GetService(Type);
        return null;
    }
}
