using System;
using System.IO.Ports;
using NumatoLabGpio;

namespace Numato_16chn
{
    class Program
    {
        public static SerialPort SerialPort;
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {

            var gpio = new GpioDriver("com7");

            gpio.Open();
            gpio.SetGpioState(0,true);

            gpio.SetGpioAsInput(0);
            gpio.SetGpioAsInput(1);
            gpio.SetGpioAsInput(2);

            while (true)
            {
                var a = gpio.GetInput(0).ToString().PadLeft(10,' ');
                var b = gpio.GetInput(1).ToString().PadLeft(10,' ');
                var c = gpio.GetInput(2).ToString().PadLeft(10,' ');
                Console.WriteLine($"A={a} / B={b} / C={c}");
                System.Threading.Thread.Sleep(10);
            }
            // ReSharper disable once FunctionNeverReturns
        }

    }
}
