using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatControllSystem
{
    internal class Utils
    {
        public static void WriteLog(string Message)
        {
            try
            {
                string LogPath = @".\\Logs\\CatController";
                if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
                string LogFileName = Path.Combine(LogPath, $"{DateTime.Now:yyyy-MMM-dd HHmmss}.txt");
                File.AppendAllText(LogFileName, $"{DateTime.Now:yyyy-MMM-dd HH:mm:ss} => {Message} {Environment.NewLine}");
            }
            catch (Exception ex)
            {
            }
        }

        public static void MessageboxAlert(string Message)
        {
            MessageBox.Show(Message, "Alert");
        }
    }

}
