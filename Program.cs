using System;
using System.IO;
using System.Threading.Tasks;

namespace CustomLogBuffer
{
    class Program
    {
        static void Main(string[] args)
        {
            DoJournaling();
            Console.WriteLine("Enter any letter to exit...");
            Console.ReadKey();
        }

        static void DoJournaling()
        {
            var message = "";
            var logBuffer = new LogBuffer(5);
            while (message != @"\e")
            {
                Console.WriteLine(@"Enter '\e' to exit");
                Console.Write("Enter message: ");
                message = Console.ReadLine();
                if (message != "")
                {
                    var item = message;
                    Task.Run(() => logBuffer.Add(item));
                }
            }
            logBuffer.CompleteJournaling();
        }
    }
}