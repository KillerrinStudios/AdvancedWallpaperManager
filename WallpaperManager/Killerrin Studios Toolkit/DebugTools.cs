using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killerrin_Studios_Toolkit
{
    public static class DebugTools
    {
        public static bool DebugMode { get { return Debugger.IsAttached; } }

        public static string PrintOutException(this Exception ex, string headerMessage)
        {
            string str = headerMessage;
            try
            {
                Debug.WriteLine(headerMessage);
                str += "\nSource: " + ex.Source +
                        "\n Help Link: " + ex.HelpLink +
                        "\n HResult: " + ex.HResult +
                        "\n Message: " + ex.Message +
                        "\n StackTrace: " + ex.StackTrace;
                Debug.WriteLine(str);

                foreach (var key in ex.Data.Keys)
                {
                    Debug.WriteLine(key.ToString() + " | " + ex.Data[key].ToString());
                }

                str += "\n";

                if (ex.InnerException != null)
                    str += PrintOutException(ex.InnerException, "Entering Inner Exception");
            }
            catch (Exception) { str = headerMessage; }

            return str;
        }
    }
}
