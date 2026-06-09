using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OSUI.Messages;

public class DialogMessage(string message,object? parameter = null): ValueChangedMessage<(string, object?)>((message, parameter))
{
    
}