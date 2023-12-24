using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace SIM800CATController
{
    internal class ATProtocolService : IDisposable
    {
        private string _portName;
        private System.IO.Ports.SerialPort _serialPort;
        private bool _isConnected;

        public ATProtocolService(string portName)
        {
            _portName = portName;
            _serialPort = new SerialPort(_portName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
        }

        public void SetPortName(string portName)
        {
            if (_isConnected)
            {
                throw new InvalidOperationException("Cannot change the port name while connected.");
            }
            _portName = portName;
            _serialPort.PortName = _portName;
        }

        /// <summary>
        /// Asynchronously connects to the serial port and checks the AT command.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ConnectAsync()
        {
            await Task.Run(() =>
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    _isConnected = true;
                }
                if (!AT())
                {
                    throw new InvalidOperationException("Unable to connect to the serial port.");
                }
                if (!CMGF())
                {
                    throw new SIM800CException("Unable to enable text mode for SMS.");
                }
            });
        }

        public async Task Disconnect()
        {
            await Task.Run(() =>
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _isConnected = false;
                }
            });
        }

        /// <summary>
        /// Sends an AT command asynchronously to check the connection with the module.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a boolean result indicating success.</returns>
        public async Task<bool> ATAsync()
        {
            return await Task.Run(() => AT());
        }

        /// <summary>
        /// Sends a command to the connected device and reads the response.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <returns>True if the device responded with 'OK'.</returns>
        private bool AT()
        {
            return SendCommand("AT");
        }
        
        /// <summary>
        /// Enables text mode for SMS.
        /// </summary>
        /// <returns></returns>
        private bool CMGF()
        {
            return SendCommand("AT+CMGF=1");
        }

        /// <summary>
        /// Synchronously sends a command and reads the response.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <returns>True if the response is 'OK'.</returns>
        private bool SendCommand(string command)
        {
            if (!_isConnected || !_serialPort.IsOpen)
            {
                throw new InvalidOperationException("Not connected to a serial port.");
            }
            _serialPort.WriteLine(command + "\r");
            string response = _serialPort.ReadLine();
            return response.Trim() == "OK";
        }

        public bool SendSMS(string phoneNumber, string message)
        {
            return SendCommand($"AT+CMGS=\"{phoneNumber}\"\r{message}\x1A");
        }

        public void Dispose()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}
