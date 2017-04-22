using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KillerrinStudiosToolkit
{
    public class APIProgress : Progress<APIProgressReport>
    {
        public APIProgress()
            :base()
        {
        }

        public APIProgress(Action<APIProgressReport> handler)
            :base(handler)
        {
        }
    }
    
    public struct APIProgressReport
    {
        public double Percentage { get; set; }
        public string StatusMessage { get; set; }
        public APIResponse CurrentAPIResonse { get; set; }
        public object Parameter { get; set; }

        public APIProgressReport(double percentage, string statusMessage, APIResponse apiResponse)
        {
            Percentage = percentage;
            StatusMessage = statusMessage;
            CurrentAPIResonse = apiResponse;
            Parameter = null;
        }
        public APIProgressReport(double percentage, string statusMessage, APIResponse apiResponse, object param)
        {
            Percentage = percentage;
            StatusMessage = statusMessage;
            CurrentAPIResonse = apiResponse;
            Parameter = param;
        }

        public override string ToString()
        {
            return string.Format("{0} | {1}, {2}", CurrentAPIResonse, Percentage, StatusMessage);
        }
    }
}
