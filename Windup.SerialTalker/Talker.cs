using System;
using System.Collections.Generic;

namespace Windup.SerialTalker
{
	public class Talker
	{
		//处理到达数据的回调函数，Talker的最核心的处理
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

		/// <summary>
		/// Creates the analyzer.
		/// </summary>
		/// <returns>The analyzer.</returns>
		/// <param name="sa">Sa.</param>
		/// <param name="lineBreak">Line break.</param>
		Analyzer CreateAnalyzer(SerialAgent sa, LineBreak lineBreak)
		{
			var analyzer = new Analyzer ();
			analyzer.SetAgent (sa);
			analyzer.SetLineBreak (lineBreak);
			analyzer.DataListReadyEvent += OnDataListReady;
			return analyzer;
		}

		/// <summary>
		/// Analyzer中的串口返回的字节列表到达时的数据处理函数.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="e">E.</param>
		void OnDataListReady(object o, DataListReadyEventArgs e)
		{
			byteListProc (e.Bytes);
		}
	}
}

