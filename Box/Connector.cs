using System.Collections.Generic;

namespace Box
{
    public class Connector
    {
        public static string[] ConnectStrings(string text)
        {
            List<string> items = new();
            bool inStr = false;
            string current = "";
            bool escaped = false;
            foreach (char ch in text)
            {
                if (ch == '\\' && !escaped)
                {
                    escaped = true;
                    continue;
                }
                if (ch == '"' && !escaped)
                {
                    inStr = !inStr;
                    items.Add(current);
                    current = "";
                    continue;
                }
                if (ch == ' ' && !inStr)
                {
                    items.Add(current);
                    current = "";
                    continue;
                }
                
                if (escaped)
                {
                    if (ch == 'n')
                    {
                        current += "\n";
                    }

                    escaped = false;
                    continue;
                }

                current += ch;
                escaped = false;
            }
            
            items.Add(current);

            items.RemoveAll(x => x.Trim().Length == 0);
            return items.ToArray();
        }
    }
}