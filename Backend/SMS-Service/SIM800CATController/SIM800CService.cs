using SIM800CATController.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM800CATController
{
    public class SIM800CService(ISIM800CDiscoveryService sIM800CDiscoveryService) : Contracts.ISIM800CService
    {
        private ISIM800CDiscoveryService _discoveryService = sIM800CDiscoveryService;
        private ATProtocolService? _service;
        private string? _portName;

        [MemberNotNull(nameof(_service), nameof(_portName))]
        public async Task EnsureConnected()
        {
            // Explicitly define _portName as non-nullable local variable.
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
            string discoveredPortName = await _discoveryService.Discover() ?? throw new SIM800CException("SIM800C module not found.");
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
            _portName = discoveredPortName; // Assign the non-null discoveredPortName to _portName

            // Ensure _service is not null or properly initialized
            if (_service == null)
            {
                _service = new ATProtocolService(_portName); // Using the non-null _portName
                await _service.ConnectAsync();
            }

            // Proceed with the rest of the method
            var atResult = await _service.ATAsync();
            if (!atResult)
            {
                throw new SIM800CException("SIM800C module connectivity not given.");
            }

            // At this point, if no exception is thrown, _service and _portName should be non-null.
        }

        public async Task<bool> SendSMS(string phoneNumber, string message)
        {
            await EnsureConnected();

            var result = await Task.Run(() => _service.SendSMS(phoneNumber, message));

            return result;
        }
    }
}
