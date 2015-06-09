using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    public class SPSetting
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        public SPSetting()
        {
            PortName = string.Empty;
            BaudRate = 9600;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
        }
    }
}
