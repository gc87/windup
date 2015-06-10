using System;
using Windup.SerialTalker;

namespace Windup.Main
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var talker = new Talker () {
				PortName = "COM3",
				BaudRate = 9600,
				LineBreak = new LineBreak(){
					Type = "linux"
				},
				Proc = list => {
					foreach (var i in list) {
						Console.WriteLine (i);
					}
				}
			};

			for (int i = 0; i < 1000; i++) {
				talker.Write (new byte[] { 10, 12, 11, 17, 18 });
			}

			Console.Read ();
		}
	}
}
