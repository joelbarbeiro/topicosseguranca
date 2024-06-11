using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class LogControllers
    {
        public static void createLog(string command, string user, string target = null)
        {
            //string path = "log.txt"; //Caminho do ficheiro de log
            string path = @"../../log.txt";
            try
            {
                if (!File.Exists(path)) //Verificar se o ficheiro existe
                {
                    File.Create(path);
                }

                //Abrir o ficheiro para escrita
                using (StreamWriter logWriter = File.AppendText(path))
                {
                    logWriter.WriteLine(DateTime.Now + " -> " + command + " - " + user + " - " + target);
                    logWriter.Close();
                }
            }
            catch (Exception ex)
            {
                //Error Handler
                using (StreamWriter logWriter = File.AppendText("log.txt"))
                {
                    logWriter.WriteLine("Error: " + ex);
                }
            }
        }
    }
}
