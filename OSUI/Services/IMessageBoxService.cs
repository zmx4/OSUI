using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSUI.Services;

internal interface IMessageBoxService
{
    public void ShowMessage(string title, string message);
}