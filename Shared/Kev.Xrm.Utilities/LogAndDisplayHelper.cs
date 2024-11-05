using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kev.Xrm.Utilities
{
    public class LogAndDisplayHelper
    {
        // Helper method to log messages to a file and display them on the console
        public static void LogAndDisplay(string message, string logFilePath)
        {
            // Display the message on the console
            Console.WriteLine(message);

            // Append the message to the log file with a timestamp
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
