using System;
using Windup.SerialTalker;

namespace Windup.Main
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//test SerialPort
			if (Talker.TouchSerialPort ("COM3", 9600)) {
				//create a instance of Talker.
				var talker = new Talker () {
					PortName = "COM3",
					BaudRate = 9600,
					LineBreak = new LineBreak () { //set linebreak type
						Type = "nt",
					},
					Proc = list => {
						var str = string.Empty;
						foreach (var i in list) {
							var ch = (char)i;
							str += ch;
						}
						Console.WriteLine ("echo str: {0}", str);
					}
				};

				//open talker if it's not opened.
				if (!talker.IsOpen) {
					talker.Open (); 
				}

				//write data to serialport.
				for (int i = 0; i < 1000; i++) {
					talker.Write (new byte[] { 10, 12, 11, 17, 18 });
				}
				Console.Read ();
			}
		}
	}
}
