using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Wings21D.Utils
{
    public class LogsGenerator
    {
        public static void LogError(string message,string dbName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory.ToString();
            string logDir = @"/logs/";
            string logDirPath = baseDirectory + logDir;
            string path = logDirPath + dbName + "_" + DateTime.Today.ToString("ddMMyyyy") + ".txt";

            try
            {   
                if (!File.Exists(@path))
                {
                    File.Create(@path).Close();
                }
               
                using(StreamWriter sw = File.AppendText(@path))
                {
                    sw.WriteLine("\r\nLog Entry: ");
                    sw.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    string err = "Error in: " + System.Web.HttpContext.Current.Request.Url.ToString() + ". \n\nError Message: "+message;
                    sw.WriteLine(err);
                    sw.WriteLine("=================================================");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
                throw;
            }
        }
        public static void LogMessage(string message, string dbName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory.ToString();
            string logDir = @"/logs/";
            string logDirPath = baseDirectory + logDir;
            string path = logDirPath + dbName + "_" + DateTime.Today.ToString("ddMMyyyy") + ".txt";
            try
            {
               // string path = "~/Logs/" + dbName + "_" + DateTime.Today.ToString("ddMMyyyy") + ".txt";
                if (!File.Exists(@path))
                {
                    File.Create(@path).Close();
                }
                using (StreamWriter sw = File.AppendText(@path))
                {
                    sw.WriteLine("\r\nLog Entry: ");
                    sw.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    // string err = "Error in: " + System.Web.HttpContext.Current.Request.Url.ToString() + ". \n\nError Message: " + message;

                    sw.WriteLine(message);
                    sw.WriteLine("=================================================");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}