using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    public class DataRxEventArgs: EventArgs
    {
        private byte _data;
        public byte Data { get { return _data; } }
        public DataRxEventArgs(byte data)
        {
            _data = data;
        }
    }
}
