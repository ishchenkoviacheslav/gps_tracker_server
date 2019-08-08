using System;
using System.Threading;

namespace MasterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MasterClient MasterClient = null;
            using (MasterClient = new MasterClient())
            {
                while (true)
                {
                    Thread.Sleep(10000);
                    Console.WriteLine("service master client rabbitmq working...");
                }
            }
        }
    }
}
