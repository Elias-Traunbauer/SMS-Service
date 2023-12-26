using SIM800CATController;
using SIM800CATController.Contracts;

namespace SIMTEST
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ISIM800CDiscoveryService discoveryService = new SIM800CDiscoveryService();
            using ISIM800CService service = new SIM800CService(discoveryService);

            string? portName = discoveryService.Discover().Result;

            if (portName != null)
            {
                System.Console.WriteLine($"Found SIM800C module on port {portName}.");
            }
            else
            {
                System.Console.WriteLine("SIM800C module not found.");
                return;
            }
            _ = service.SendSMS("1", "").Result;
            Console.Write("msg?: ");
            string msg = Console.ReadLine()!;
            bool result = service.SendSMS("4367762357798", msg).Result;

            if (result)
            {
                System.Console.WriteLine("SMS sent successfully.");
            }
            else
            {
                System.Console.WriteLine("SMS could not be sent.");
            }

            bool resultz = service.SendSMS("4367762357798", msg + 2).Result;

            bool resultzz = service.SendSMS("4367762357798", msg + 3).Result;
        }
    }
}
