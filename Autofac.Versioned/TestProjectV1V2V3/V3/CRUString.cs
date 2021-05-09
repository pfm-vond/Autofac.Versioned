using System.Collections.Generic;
using System.Linq;

namespace TestProjectV1V2V3
{
    internal class CRUString : CName, RName, UName, CSurname, RSurname
    {
        private HashSet<string> content = new ();

        public void Create(string value)
        {
            content.Add(value);
        }

        public string[] Read()
        {
            return content.ToArray();
        }

        public void Update(string oldValue, string newValue)
        {
            content.Remove(oldValue);
            content.Add(newValue);
        }
    }
}