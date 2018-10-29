Windup
======

### Introduction
* Serial port access library on c#. 

### Quickstart
get all serial port name.
```C#
string[] portNames = GetSerialPorts ();
````

test a serial port is busy.
```C#
var isBusy = Talker.TouchSerialPort ("COM3", 9600)
````

create a instance of Talker.
```C#
var talker = new Talker () {
	PortName = "COM3",
	BaudRate = 9600,
	LineBreak = new LineBreak () { //set linebreak type
		Type = "nt",
	},
	Proc = list => { // data arrived process method(important)
		var str = string.Empty;
		foreach (var i in list) {
			var ch = (char)i;
			str += ch;
		}
		Console.WriteLine ("echo str: {0}", str);
	}
};
````

you can also use Parity, DataBits, StopBits to initlize talker.

linebreak type: "nt" -> '\r\n'; "linux" -> '\n'; "mac" -> '\r'; "length" -> you must initializing another property name is Length like this 
```C#
var lineBreak = new LineBreak () {
		Type = "length",
		Length = 100
	},
````

"char" -> you must initializing another property name is Char like this
```C#
var lineBreak = new LineBreak () {
		Type = "char",
		Char = 'e'
	},
````

open talker if it's not opened.
```C#
if (!talker.IsOpen) {
	talker.Open (); 
}
````

write byte data to serialport.
```C#
talker.Write (new byte[] { 10, 12, 11, 17, 18 });
````

close talker.
```C#
var isSuccessed = talker.Close();
````
	
### License
Copyright 2009–2015 gaoc and contributors
MIT License (enclosed)
