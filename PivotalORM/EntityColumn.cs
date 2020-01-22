using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PivotalORM
{
    public class EntityColumn
    {
        public string Name { get; private set; }
        public PropertyInfo Property { get; private set; }
        public bool AsString { get; private set; }

        public EntityColumn(string name, PropertyInfo property, bool asString)
        {
            Name = name;
            Property = property;
            AsString = asString;
        }
    }
}
