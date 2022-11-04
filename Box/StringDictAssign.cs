using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodi.IO
{
    public class StringDictAssign
    {
        public readonly string Data;
        public StringDictAssign(string data)
        {
            Data = data;
        }
        public Dictionary<string, string> Evaluate(string keyValSep, string entrySep, bool doTrim = true, string skipLineStr = null)
        {
            Dictionary<string, string> result = new();
            foreach (string item in Data.Split(entrySep))
            {
                if (item.Trim().Length == 0) continue;
                if (skipLineStr != null && item.StartsWith(skipLineStr)) continue;

                string[] splt = item.Split(keyValSep, 2);
                string key = splt[0];
                string val = splt.Length > 1 ? splt[1] : "";
                if (doTrim)
                {
                    key = key.Trim();
                    val = val.Trim();
                }
                result[key] = val;
            }
            return result;
        }
    }
}
