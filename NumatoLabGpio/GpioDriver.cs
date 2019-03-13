using System;
using System.Collections;
using System.IO.Ports;

namespace NumatoLabGpio
{
    /// <summary>
    /// https://numato.com/docs/16-channel-usb-gpio-module-with-analog-inputs/
    /// </summary>

    public class GpioDriver
    {
        private const int Ionumber = 16;

        private string _comPort;
        private SerialPort _serialPort;
        private string _serialData;
        private bool _dataInBuffer;
        private bool _connected;

        private bool[] _gpioOutputDirection;

        public GpioDriver (
            string comPort
        )
        {
            _connected = false;
            _comPort = comPort;
        }

        public void Open()
        {
            OpenSerialPort();
            SendData("ver");
            var data = GetData();
            if (data == null) throw new Exception("Port not valid or no module attached");
            if (data.Length < 5)throw new Exception("Port not valid or no module attached");
            _connected = true;
            _gpioOutputDirection = new bool[Ionumber];
            for (int i = 0; i < Ionumber; i++)
                _gpioOutputDirection[i] = false;
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen) _serialPort.Close();
            _serialPort = null;
            _connected = false;
        }

        public void SetGpioState(int gpioNumber, bool state)
        {
            var portNumber = ValidateGpioPortNumber(gpioNumber);

            if (state)
            {
                SendData($"gpio set {portNumber}");
            }
            else
            {
                SendData($"gpio clear {portNumber}");
            }
        }


        public void SetGpioAsInput(int gpioNumber)
        {
            ValidateGpioPortNumber(gpioNumber);
            _gpioOutputDirection[gpioNumber] = true;
            SetDirection();
        }

        public void SetGpioAsOutput(int gpioNumber)
        {
            ValidateGpioPortNumber(gpioNumber);
            _gpioOutputDirection[gpioNumber] = false;
            SetDirection();
        }

        public bool GetInput(int gpioNumber)
        {
            var gpio = ValidateGpioPortNumber(gpioNumber);
            SendData($"gpio read {gpio}");
            var data = GetData();
            if ((data == null) || (data.Length < 1))
            {
                throw new Exception("Read from module failes");
            }

            return data[0] == '1';
        }

        // *************************************************************************************************

        private void SetDirection()
        {
            BitArray arr = new BitArray(_gpioOutputDirection);
            var data = new int[1];
            arr.CopyTo(data, 0);
            var st = data[0].ToString("X").PadLeft(4,'0');
            SendData($"gpio iodir {st}");
        }

        private string ValidateGpioPortNumber(int gpioNumber)
        {
            if (!_connected) throw new Exception("GPIO not open");
            if (gpioNumber < 0 ||gpioNumber >= Ionumber) throw new Exception("GPIO Number outside valid range");
            return gpioNumber.ToString("X");
        }

        private string GetData()
        {
            for (int i = 0; i < 10; i++)
            {
                if (_dataInBuffer) return _serialData;
                System.Threading.Thread.Sleep(1);
            }
            return null;
        }

        private void SendData(string data)
        {
            _dataInBuffer = false;
            _serialData = "";
            _serialPort.Write(data + "\r");
        }

        private void OpenSerialPort()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort = null;
            }

            _serialPort = new SerialPort 
                {BaudRate = 9600};
            _serialPort.PortName = _comPort;
            _serialPort.Open();
            _serialPort.DataReceived += SerialPortOnDataReceived;
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            _serialData = _serialPort.ReadExisting();
            _dataInBuffer = true;
        }
    }
}
