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

            // loop, ask for msg and send

            for (;true;)
            {
                System.Console.WriteLine("msg?: ");
                string msg2 = Console.ReadLine()!;

                if (!string.IsNullOrEmpty(msg2))
                {
                    service.SendSMS("4367762357798", msg2).Wait();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
