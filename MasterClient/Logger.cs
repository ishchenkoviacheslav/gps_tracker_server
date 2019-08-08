using System;
using System.IO;

namespace MasterClient
{
    class Logger
    {
        static object obj = new object();

        public static void Info(string message)
        {
            RecordEntry("INFO", message);
        }
        public static void Error(string message)
        {
            RecordEntry("ERROR", message);
        }
        private static void RecordEntry(string fileEvent, string EventData)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(@"/home/" + "RabitMQ_MasterClientlog.txt", true))
                //using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "RabitMQ_MasterClientlog.txt", true))
                {
                    writer.WriteLine(String.Format($"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")} : [{fileEvent}] - {EventData}"));
                    writer.Flush();
                }
            }
        }
    }
}
