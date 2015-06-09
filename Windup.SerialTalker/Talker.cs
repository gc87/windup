using System;
using System.Collections.Generic;

namespace Windup.SerialTalker
{
	public class Talker
	{
		public delegate void ByteListProc(List<byte> bytes);
		public ByteListProc byteListProc;
		public Talker ()
		{
		}

		public Talker(string portName, int baudRate, LineBreak lineBreak)
		{
			var sa = new SerialAgent (portName, baudRate);
			CreateAnalyzer (sa, lineBreak);
		}

		public Talker(SPSetting setting, LineBreak lineBreak)
		{
			var sa = AgentGenerator.CreateAgent (setting);
			CreateAnalyzer (sa, lineBreak);
		}

		Analyzer CreateAnalyzer(SerialAgent sa, LineBreak lineBreak)
		{
			var analyzer = new Analyzer ();
			analyzer.SetAgent (sa);
			analyzer.SetLineBreak (lineBreak);
			analyzer.DataListReadyEvent += OnDataListReady;
			return analyzer;
		}

		void OnDataListReady(object o, DataListReadyEventArgs e)
		{
			byteListProc (e.Bytes);
		}
	}
}

