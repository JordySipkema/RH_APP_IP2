using System;
using System.IO.Ports;
using Mallaca;

namespace RH_APP.Classes
{
    class COM_Bike : IBike
    {
        private readonly SerialPort serial = null;

        public COM_Bike(String com_port) 
        {
            serial = new SerialPort
            {
                PortName = com_port,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 2000,
                WriteTimeout = 100
            };

            serial.Open();
            serial.WriteLine("CM");
        }

        public override Measurement RecieveData()
        {
            String rcv = string.Empty;
            try
            {
                serial.WriteLine("ST");
                rcv = serial.ReadLine();
                Measurement m = ProtocolToMeasurement(rcv);
                m.DATE = DateTime.Now;
                return m;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Serialconnection timed out");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Operation Invalid \n" + e.Message);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Data received = {0}", rcv);
                Console.WriteLine("Invalid data!");
            }
            return null;
        }
        public override void SendData(string command)
        {
            serial.WriteLine(command);
        }

        /// <summary>
        /// Destructor for COM_Bike
        /// Closes the active serial connection to release resources
        /// </summary>
        ~COM_Bike()
        {
            serial.Close();
        }

    }
}
