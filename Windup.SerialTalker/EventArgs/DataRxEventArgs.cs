using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    public class DataRxEventArgs: EventArgs
    {
		public DataRxEventArgs ()
		{
		}

		public byte Data {
			get;
			set;
		}
    }
}
