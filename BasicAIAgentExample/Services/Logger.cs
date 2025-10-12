using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicAIAgentExample.Services
{
   public static class Logger
    {
        public static void ConsoleLog(string logText, ConsoleColor consoleColor = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(logText);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
