using System;
using System.Text;
using Windup.SerialTalker;

namespace Transceiver
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var portName = "COM3";
			Console.Write ("> ");
			portName = Console.ReadLine (); 
			Console.WriteLine ("< {0}", portName);

			var talker = new Talker () {
				PortName = portName,
				BaudRate = 9600,
				LineBreak = new LineBreak () { 
					Type = "nt"
				},
				Proc = list => {
					var str = string.Empty;
					foreach (var i in list) {
						var ch = (char)i;
						str += ch;
					}
					Console.WriteLine ("< {0}", str);
				}
			};
			talker.Open ();

			while (true) {
				Console.Write ("> "); 
				string line = Console.ReadLine (); 
				if (line == ".exit") {
					if (talker.IsOpen) {
						talker.Close ();
					}
					break;
				}
				var array = Encoding.ASCII.GetBytes (line);
				var length = array.Length;
				var what = new byte[length + 2];
				what [length + 2 - 2] = 13;
				what [length + 2 - 1] = 10;
				array.CopyTo (what, 0);
				talker.Write (what);
			}
		}
	}
}
