using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmokeQuit.Common.Shared.LocDpx
{
    public  class Utilities
    {
        private static string loggerFilePath = Directory.GetCurrentDirectory() + @"\DataLog.txt";

        public static string ConvertObjectToJSONString<T> (T entity)
        {
            string jsonString = JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            return jsonString;
        }

        public static void WriteLoggerFile(string content)
        {
            try
            {
                var path = Directory.GetCurrentDirectory();

                using (var file = File.Open(loggerFilePath, FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteAsync(content);
                    writer.FlushAsync();
                }
            }
            catch (Exception ex)
            {
             
            }
        }

    }
}
