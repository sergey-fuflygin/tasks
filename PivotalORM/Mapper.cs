using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PivotalORM
{
    public class Mapper
    {
        // row --> entity
        public object Map(DataRow dataRow, Type entityType)
        {
            var pivotalObject = Activator.CreateInstance(entityType);
            var metadata = EntityMetadata.Create(entityType);
            foreach (var columnInfo in metadata.Columns)
            {
                try
                {
                    SetFieldValue(pivotalObject, dataRow, columnInfo);
                }
                catch (Exception ex)
                {
                    throw new MappingException(string.Format("Unable to map database column {0} to the property {1} of type {2}", columnInfo.Name, columnInfo.Property.Name, entityType.FullName), ex);
                }
            }
            return pivotalObject;
        }

        // entity --> row
        public void Map(object obj, DataRow row)
        {
            var metadata = EntityMetadata.Create(obj.GetType());

            foreach (var column in metadata.GetAllColumnsExceptPrimaryKey())
            {
                try
                {
                    var propertyValue = column.Property.GetValue(obj);
                    var dbValue = ConvertPropertyValueToDatabaseValue(propertyValue, column);
                    row[column.Name] = dbValue;
                }
                catch (Exception ex)
                {
                    throw new MappingException(string.Format(
                        "Unable to map property {0} of type {1} to the dabatase filed {2}", 
                        column.Property.Name,
                        column.Property.DeclaringType.FullName,
                        column.Name), ex);
                }
            }
        }

        private static object ConvertPropertyValueToDatabaseValue(object propertyValue, EntityColumn columnInfo)
        {
            if (propertyValue == null)
            {
                return DBNull.Value;
            }

            if (columnInfo.AsString)
            {
                //if (!(propertyValue is bool))
                //{
                //    throw new MappingException(string.Format("Unable to convert value {0} to boolean", intValue));

                //    throw new ArgumentException("Unable to apply AsString");
                //}

                return propertyValue.ToString();
                //return (double)rawValue * 100;
            }

            var type = propertyValue.GetType();
            if (type.IsEnum)
            {
                var attribute = type.GetField(Enum.GetName(type, propertyValue))
                                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                                    .SingleOrDefault() as DisplayAttribute;

                return (attribute != null && attribute.Name != "")
                    ? attribute.Name
                    : propertyValue.ToString();
            }

            if (!(propertyValue is string))
            {
                return propertyValue;
            }

            var stringValue = propertyValue as string;
            return stringValue != ""
                ? stringValue.ToANSI()
                : null;
        }

        private static void SetFieldValue(Object pivotalObject, DataRow dataRow, EntityColumn columnInfo)
        {
            var dbValue = dataRow[columnInfo.Name];
            var convertedValue = ConvertDatabaseValueToPropertyValue(dbValue, columnInfo);
            columnInfo.Property.SetValue(pivotalObject, convertedValue);
        }

        private static object ConvertDatabaseValueToPropertyValue(object dbValue, EntityColumn fieldInfo)
        {
            if (dbValue is DBNull || dbValue == null)
            {
                return null;
            }

            var targetType = fieldInfo.Property.PropertyType;
            if (targetType.IsAssignableFrom(dbValue.GetType()))
            {
                return dbValue;
            }

            var typeToUse = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (typeToUse == typeof(bool))
            {
                var intValue = Convert.ToInt32(dbValue);
                if (intValue < 0 || intValue > 1)
                {
                    throw new MappingException(string.Format("Unable to convert value {0} to boolean", intValue));
                }

                return intValue == 1;
            }

            if (typeToUse.IsEnum) // Pivotal Combo
            {
                var strValue = Convert.ToString(dbValue);

                try
                {
                    return GetEnumValueFromName(typeToUse, strValue);
                }
                catch (Exception ex)
                {
                    throw new MappingException(string.Format("Unable to convert value {0} to enum {1}", strValue, typeToUse.FullName), ex);

                }
            }

            return Convert.ChangeType(dbValue, typeToUse);
        }

        internal void SetId(object obj, DataRow row, EntityMetadata metadata)
        {
            metadata.PrimaryKey.Property.SetValue(obj, row[metadata.PrimaryKey.Name]);
        }

        private static object GetEnumValueFromName(Type enumType, string displayName)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", enumType.FullName));
            }

            var enumField = (from field in enumType.GetFields()
                             let attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute
                             where (attribute != null && attribute.Name == displayName) || field.Name == displayName
                             select field).SingleOrDefault();
            if (enumField != null)
            {
                return enumField.GetValue(null);
            }

            throw new MappingException(string.Format("Unable to convert value {0} to enum {1}", displayName, enumType.FullName));
        }

        public static T GetEnumValueFromName<T>(string displayName)
        {
            return (T)GetEnumValueFromName(typeof(T), displayName);

            //return GetEnumValueFromName(typeof(T), displayName);
        }
    }
}
