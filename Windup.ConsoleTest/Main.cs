using System;
using System.Diagnostics;
using System.Threading;
using Windup.SerialTalker;
using Windup.ExecuteEngine;
using System.Collections;
using System.Collections.Generic;

namespace Windup.ConsoleTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            ScriptingEngine se = new ScriptingEngine(path + "\\scripts\\script00.py");
            */
            ScriptingEngineFactory factory = new ScriptingEngineFactory();
            List<ScriptingEngine> list = factory.GetScriptingEngineList();
            foreach (var en in list) {
                Console.WriteLine("name: " + en.GetVariable("__caption__") + "--explain: " + en.GetVariable("__explain__"));
            }

            Type t = Type.GetType("Mono.Runtime");
            var r = t != null ? "Mono" : ".NET";
            Console.WriteLine("WindupConsoleTest v1.0.0");
            Console.WriteLine("OS: " + Environment.OSVersion.Platform);
            Console.WriteLine("Runtime: " + r + Environment.Version.Major);
            /*
            var list = SerialList.ReturnSerialList ();
            foreach (var port in list) {
                Console.WriteLine (port + "\t");
            }
*/

            if (SerialAgent.TouchAgentPort("COM3", 9600)) {
                /*
                SerialAgent s;
                AnalyzerNT a;
                MyCodecFactory f;
                Codec codec;

                s = new SerialAgent("COM3", 9600);
                s.AgentOpen();
                a = new AnalyzerNT(s);
                f = new MyCodecFactory();
                codec = new Codec(f, a);

                while (true) {
                    Console.Write("> ");
                    var cmd = Console.ReadLine();
                    if ("EXIT" == cmd.ToUpper()) {
                        Console.WriteLine("< See you later.");
                        Thread.Sleep(700);
                        break;
                    } else {
                        codec.WriteData(cmd);
                        Thread.Sleep(200);
                    }
                    if (0 < codec.queue.Count) {
                        var d = codec.queue.Dequeue();
                        foreach (var kv in d) {
                            Console.WriteLine("< " + kv.Value);
                        }
                    }
                }
                s.AgentClose();
                */
            } else {
                Console.WriteLine("Serial ports may be occupied.");
                Console.WriteLine("Press Enter exit.");
                Console.Read();
            }
        }
    }
}
