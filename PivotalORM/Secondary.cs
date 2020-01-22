using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PivotalORM
{
    public class Secondary
    {
        public PropertyInfo Property { get; private set;  }
        public Type ItemType { get; private set;  }
        public EntityMetadata ItemMetadata { get; private set;  }

        private Secondary(PropertyInfo property, Type itemType, EntityMetadata itemMetadata)
        {
            Property = property;
            ItemType = itemType;
            ItemMetadata = itemMetadata;
        }

        public static Secondary Create(PropertyInfo property)
        {
            var genericArgs = property.PropertyType.GetGenericArguments();
            if (genericArgs.Length != 1)
            {
                throw new Exception(string.Format(
                    "Unable to determine secondary collection type for property {0} of type {1}",
                    property.Name,
                    property.DeclaringType.FullName));
            }
            var itemType = genericArgs[0];
            return new Secondary(property, itemType, EntityMetadata.Create(itemType));
        }
    }
}
