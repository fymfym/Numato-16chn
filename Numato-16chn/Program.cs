using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumatoLabGpio;

namespace Numato_16chn
{
    class Program
    {
        private static System.IO.Ports.SerialPort _p ;
        static void Main(string[] args)
        {

            var gpio = new GpioDriver("com7");


            gpio.Open();
            gpio.SetGpioState(0,true);

            gpio.SetGpioAsInput(0);
            gpio.SetGpioAsInput(1);
            gpio.SetGpioAsInput(2);
            //_p = new SerialPort("COM7");

            //_p.DataReceived += POnDataReceived;
            //_p.BaudRate = 9600;
            //_p.Open();
            //_p.Write("ver\r");
            //System.Threading.Thread.Sleep(1000);
            //_p.Write("gpio set 0\r");
            while (true)
            {
                var a = gpio.GetInput(0).ToString().PadLeft(10,' ');
                var b = gpio.GetInput(1).ToString().PadLeft(10,' ');
                var c = gpio.GetInput(2).ToString().PadLeft(10,' ');
                Console.WriteLine($"A={a} / B={b} / C={c}");
                System.Threading.Thread.Sleep(10);
            }

            gpio.Close();
        }

        private static void POnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            Console.WriteLine(_p.ReadLine());
        }
    }
}
