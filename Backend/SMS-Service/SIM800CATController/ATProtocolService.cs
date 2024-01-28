using System;
using System.IO.Ports;
using System.Text;
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
                ReadTimeout = 10000,
                WriteTimeout = 10000
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

        public async Task ConnectAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (!_serialPort.IsOpen)
                    {
                        _serialPort.Open();
                        _isConnected = true;
                    }
                    ATECHO(false);
                    if (!AT() || !CMGF())
                    {
                        throw new InvalidOperationException("Failed to initialize the device.");
                    }
                });
            }
            catch (Exception)
            {
                await Disconnect(); // Ensure disconnection on failure
                throw; // Re-throw the exception to the caller
            }
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

        public async Task<bool> ATAsync()
        {
            try
            {
                return await Task.Run(() => AT());
            }
            catch (Exception)
            {
                await Disconnect(); // Ensure disconnection on failure
                throw; // Consider whether to return false or re-throw based on your error handling policy
            }
        }

        private bool AT()
        {
            try
            {
                return SendCommand("AT") == "OK";
            }
            catch (Exception)
            {
                Disconnect().Wait(); // Synchronously wait for disconnection to ensure state
                throw; // Propagate the exception up
            }
        }

        private bool ATECHO(bool enable)
        {
            try
            {
                return SendCommand($"ATE{(enable ? "1" : "0")}") == "OK";
            }
            catch (Exception)
            {
                Disconnect().Wait();
                throw;
            }
        }

        private bool CMGF()
        {
            try
            {
                return SendCommand("AT+CMGF=1") == "OK";
            }
            catch (Exception)
            {
                Disconnect().Wait();
                throw;
            }
        }

        private string SendCommand(string command, bool newLineCommit = true, bool noResponsePossible = false)
        {
            if (!_isConnected || !_serialPort.IsOpen)
            {
                throw new InvalidOperationException("Not connected to a serial port.");
            }

            try
            {
                _serialPort.WriteLine(command + (newLineCommit ? "\r" : ""));

                StringBuilder responseBuilder = new StringBuilder();
                char[] buffer = new char[1]; // Buffer to hold each character

                try
                {
                    // Loop until the end conditions are met ("OK" or "ERROR")
                    while (true)
                    {
                        // Read a single character
                        int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0 && buffer[0] != '\r' && buffer[0] != '\n')
                        {
                            responseBuilder.Append(buffer, 0, bytesRead);
                        }

                        // Convert the current buffer into a string to check for end conditions
                        string response = responseBuilder.ToString();
                        if (response.Contains("OK") || response.Contains("ERROR") || response.Contains('>'))
                        {
                            break; // Break the loop if the end condition is met
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!noResponsePossible)
                    {
                        throw;
                    }
                    else
                    {
                        ATECHO(false);
                    }
                }

                return responseBuilder.ToString(); // Return the accumulated response
            }
            catch (Exception)
            {
                Disconnect().Wait();
                throw;
            }
        }

        public bool SendSMS(string phoneNumber, string message)
        {
            try
            {
                // Send the initial command to start SMS prompt
                string initialResponse = SendCommand($"AT+CMGS=\"{phoneNumber}\"");

                // Check for the prompt to input the message text
                if (!initialResponse.EndsWith('>') && false)
                {
                    return false; // If no prompt, something went wrong.
                }

                // Send the message text followed by the Ctrl-Z character, and read the final response
                string finalResponse = SendCommand(message + "\x1A", false, true);

                // Check if the final response indicates success
                return finalResponse.Contains("+CMGS:") && finalResponse.Contains("OK");
            }
            catch (Exception)
            {
                Disconnect().Wait();
                throw;
            }
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
