using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PivotalORM
{
    public class EntityMetadata
    {
        public string TableName { get; private set; }
        public EntityColumn PrimaryKey { get; private set; }
        public IReadOnlyCollection<EntityColumn> Columns { get; private set; }
        public EntityColumn ForeignKey { get; private set; }
        public IReadOnlyCollection<Secondary> Secondaries { get; private set; }

        private EntityMetadata(string tableName, EntityColumn primaryKey, IReadOnlyCollection<EntityColumn> columns, EntityColumn foreignKey, IReadOnlyCollection<Secondary> secondaries)
        {
            TableName = tableName;
            PrimaryKey = primaryKey;
            Columns = columns;
            ForeignKey = foreignKey;
            Secondaries = secondaries;
        }

        public IEnumerable<EntityColumn> GetAllColumnsExceptPrimaryKey()
        {
            return Columns.Where(c => c != PrimaryKey);
        }

        public static EntityMetadata Create(Type entityType)
        {
            var tableAttribute = entityType.GetCustomAttributes().OfType<TableAttribute>().Single();
            var tableName = tableAttribute.Name ?? entityType.GetDefaultTableName();

            var properties = entityType.GetProperties();

            var columns = (from property in properties
                           let attribute = property.GetCustomAttribute<ColumnAttribute>()
                           where attribute != null
                           let columnName = attribute.Name ?? property.GetDefaultColumnName()
                           let asString = property.GetCustomAttribute<AsStringAttribute>() != null
                           select new EntityColumn(columnName, property, asString)).ToArray();

            var primaryKeyProperty = properties.Single(p => Attribute.IsDefined(p, typeof(PrimaryKeyAttribute)));
            var primaryKeyColumn = columns.Single(f => f.Property == primaryKeyProperty);

            var foreignKeyProperty = properties.SingleOrDefault(p => Attribute.IsDefined(p, typeof(ForeignKeyAttribute)));
            var foreignKeyColumn = columns.SingleOrDefault(f => f.Property == foreignKeyProperty);

            var secondaries = (from property in properties
                               let attribute = property.GetCustomAttribute<SecondaryAttribute>()
                               where attribute != null
                               select Secondary.Create(property)).ToArray();

            return new EntityMetadata(tableName, primaryKeyColumn, columns, foreignKeyColumn, secondaries);
        }
    }
}
