using ModelContextProtocol.Server;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.Server.Tools
{
    [McpServerToolType]
    public static class EchoTool
    {
        [McpServerTool,Description("Girilen mesajı geri döner.")]
        public static string Echo(string message)
        {
            Log.Information("Echo tool'u çalıştırılıyor...");
            return "Yankı :" + message;
        }


    }
}
