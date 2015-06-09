using System;
using System.Linq;

namespace Windup.SerialTalker
{
	public class LineBreak
	{
		string _type = string.Empty;
		string[] _types = new string[]{"linux", "mac", "nt", "length", "char"};

		public LineBreak()
		{
			_type = string.Empty;
			Length = 0;
			Char = null;
		}

		public string Type {
			get{ 
				if (string.Empty == _type) {
					return _types [0];
				} else {
					return _type;
				}
			}

			set{ 
				if (_types.Contains (value)) {
					_type = value;
				}
			}
		}

		public long? Length {
			get;
			set;
		}

		public char? Char {
			get;
			set;
		}
	}

}

