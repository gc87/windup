Windup
======

Serial port access library on c#. 

###Introduction

###Quickstart
>Get all serial port name.

```C#
string[] portNames = GetSerialPorts ();
````

>Test a serial port is busy.

```C#
var isBusy = Talker.TouchSerialPort ("COM3", 9600)
````

>Create a instance of Talker.

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

>Open talker if it's not opened.

```C#
if (!talker.IsOpen) {
	talker.Open (); 
}
````

>Write byte data to serialport.

```C#
talker.Write (new byte[] { 10, 12, 11, 17, 18 });
````

>Close talker.

```C#
var isSuccessed = talker.Close();
````
	
###License