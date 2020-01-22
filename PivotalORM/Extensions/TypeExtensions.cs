using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotalORM
{
    public static class TypeExtensions
    {
        public static string GetDefaultTableName(this Type entityType)
        {
            return entityType.Name.Underscored();
        }
    }
}
