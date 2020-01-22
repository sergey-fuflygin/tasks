using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PivotalORM
{
    public static class PropertyInfoExtensions
    {
        public static string GetDefaultColumnName(this PropertyInfo property)
        {
            return property.Name.Underscored();
        }
    }
}
