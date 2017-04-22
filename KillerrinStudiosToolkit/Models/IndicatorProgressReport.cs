using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KillerrinStudiosToolkit.Models
{
    public struct IndicatorProgressReport
    {
        public bool RingEnabled { get; }
        public double Percentage { get; }
        public string StatusMessage { get; }
        public bool WriteToDebugConsole { get; }

        public IndicatorProgressReport(bool ringEnabled, double percentage, string statusMessage, bool writeToDebugConsole)
        {
            RingEnabled = ringEnabled;
            Percentage = percentage;
            StatusMessage = statusMessage;
            WriteToDebugConsole = writeToDebugConsole;
        }

    }
}
