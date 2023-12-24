using SIM800CATController.Contracts;

namespace SIM800CATController
{
    public class SIM800CDiscoveryService : Contracts.ISIM800CDiscoveryService
    {
        private ATProtocolService _service;

        /// <summary>
        /// Discovers the SIM800C module and returns the port name.
        /// </summary>
        /// <returns></returns>
        public async Task<string> Discover()
        {
            string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                _service = new ATProtocolService(portName);
                await _service.ConnectAsync();
                var result = await _service.ATAsync();

                if (result)
                {
                    return portName;
                }
            }
            return null;
        }
    }
}
