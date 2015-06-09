using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Windup.SerialTalker
{
    class SerialList
    {
        public static string[] ReturnSerialList()
        {
            string[] allSerial = null;
            try
            {
                allSerial = SerialPort.GetPortNames();
            }
            catch /*(Exception e)*/
            {
                //throw new Exception(e.Message);
                return null;
            }
            return allSerial;
        }
    }
}
