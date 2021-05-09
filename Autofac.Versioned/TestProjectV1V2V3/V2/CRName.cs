using System.Collections.Generic;
using System.Linq;

namespace TestProjectV1V2V3
{
    internal class CRName : CName, RName
    {
        private HashSet<string> names = new ();

        public void Create(string name)
        {
            names.Add(name);
        }

        public string[] Read()
        {
            return names.ToArray();
        }
    }
}