using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Esquel.Library.Infrastructure.Configuration
{
    public interface IApplicationSettings
    {
        string LoggerName { get; }

        string Database { get; }

        int Interval { get; }

    }
}
