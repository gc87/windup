using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Windup.ExecuteEngine
{
    public class ScriptingEngineFactory
    {
        List<string> list = new List<string>();
        public ScriptingEngineFactory()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            path += "scripts";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (FileInfo i in directoryInfo.GetFiles()) {
                list.Add(i.Name);
            }
        }

        public List<ScriptingEngine> GetScriptingEngineList()
        {
            List<ScriptingEngine> engList = new List<ScriptingEngine>();
            foreach (string fName in list) {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "scripts\\";
                path += fName;
                ScriptingEngine eng = new ScriptingEngine(path);
                engList.Add(eng);
            }
            return engList;
        }
    }
}
