using CdcSoftware.Pivotal.Engine;
using CdcSoftware.Pivotal.Applications.Foundation.Server;
using PivotalORM.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace PivotalORM
{
    public class Repository : IRepository
    {
        private IPivotalDataAccess _pivotalDataAccess;
        private Mapper _mapper;

        public Repository(IPivotalDataAccess pivotalDataAccess)
        {
            if (pivotalDataAccess == null)
            {
                throw new ArgumentNullException("pivotalDataAccess");
            }
            _pivotalDataAccess = pivotalDataAccess;

            _mapper = new Mapper();
        }

        public static IRepository Create(DataAccess pivotalDataAccess)
        {
            return new Repository(new PivotalDataAccessWrapper(pivotalDataAccess));
        }

        public T Get<T>(byte[] id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            var metadata = EntityMetadata.Create(typeof(T));
            var dataRow = _pivotalDataAccess.GetDataRow(metadata.TableName, Id.Create(id), metadata.Columns.Select(c => c.Name).ToArray());
            var entity = MapRowAndSecondariesToEntity(dataRow, typeof(T), metadata);
            return (T)entity;
        }

        public IEnumerable<T> Get<T>(string queryName, params object[] parameters)
        {
            var type = typeof(T);
            var metadata = EntityMetadata.Create(type);
            var dataTable = _pivotalDataAccess.GetDataTable(queryName, parameters, metadata.Columns.Select(c => c.Name).ToArray());
            foreach (DataRow row in dataTable.Rows)
            {
                yield return (T)MapRowAndSecondariesToEntity(row, type, metadata);
            }
        }

        public T GetSingleOrDefault<T>(string queryName, params object[] parameters)
        {
            var items = Get<T>(queryName, parameters);
            switch (items.Count())
            {
                case 0:
                    return default(T);
                case 1:
                    return items.Single();
                default:
                    throw new Exception($"Query {queryName} returns more than one element for parameter(s) {GetParamsString(parameters)}");
            }
        }

        public T GetSingleItem<T>(string queryName, params object[] parameters)
        {
            var item = GetSingleOrDefault<T>(queryName, parameters);
            if (item == null)
                throw new Exception($"Query {queryName} returns no elements for parameter(s) {GetParamsString(parameters)}");
            return item;
        }

        public void Insert(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var entityType = obj.GetType();
            var metadata = EntityMetadata.Create(entityType);
            var columnNames = metadata.Columns.Select(f => f.Name).ToArray();

            var row = _pivotalDataAccess.GetNewDataRow(metadata.TableName, columnNames);
            _mapper.Map(obj, row);
            var newRow = _pivotalDataAccess.SaveDataRow(row);

            _mapper.SetId(obj, newRow, metadata);
        }

        public void InsertRange(IEnumerable objects)
        {
            if (objects == null)
            {
                throw new ArgumentNullException("objects");
            }

            var types = objects.Cast<object>().Select(o => o.GetType()).Distinct().ToArray();
            if (types.Length == 0)
            {
                return;
            }
            if (types.Length > 1)
            {
                throw new ArgumentException("Collection contains objects of different types", "objects");
            }

            foreach (var obj in objects)
            {
                this.Insert(obj);
            }
        }

        public void Update(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var entityType = obj.GetType();
            var metadata = EntityMetadata.Create(entityType);
            var columnNames = metadata.Columns.Select(f => f.Name).ToArray();

            var id = metadata.PrimaryKey.Property.GetValue(obj);
            var row = _pivotalDataAccess.GetDataRow(metadata.TableName, Id.Create(id), columnNames);
            _mapper.Map(obj, row);
            _pivotalDataAccess.SaveDataRow(row);

            foreach (var secondary in metadata.Secondaries)
            {
                var secondaryItems = secondary.Property.GetValue(obj) as IEnumerable;
                if (secondaryItems == null)
                {
                    continue;
                }
                foreach (var secondaryItem in secondaryItems)
                {
                    Update(secondaryItem);
                }
            }
        }

        public void Delele<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var entityType = obj.GetType();
            var metadata = EntityMetadata.Create(entityType);
            var id = metadata.PrimaryKey.Property.GetValue(obj);
            _pivotalDataAccess.Delete(metadata.TableName, Id.Create(id));
        }   

        private object MapRowAndSecondariesToEntity(DataRow row, Type entityType, EntityMetadata metadata)
        {
            var primary = _mapper.Map(row, entityType);

            foreach (var secondary in metadata.Secondaries)
            {
                var secondaryDataTable = _pivotalDataAccess.GetLinkedDataTable(
                    secondary.ItemMetadata.TableName,
                    secondary.ItemMetadata.ForeignKey.Name,
                    Id.Create(metadata.PrimaryKey.Property.GetValue(primary)),
                    secondary.ItemMetadata.Columns.Select(c => c.Name).ToArray());

                var collectionType = secondary.Property.PropertyType;
                var collection = Activator.CreateInstance(collectionType);
                var addMethod = collectionType.GetMethod("Add");
                
                foreach (DataRow secondaryRow in secondaryDataTable.Rows)
                {
                    var secondaryItem =_mapper.Map(secondaryRow, secondary.ItemType);
                    addMethod.Invoke(collection, new object[] { secondaryItem });
                }

                secondary.Property.SetValue(primary, collection);
            }

            return primary;
        }

        public IEnumerable<T> GetAll<T>()
        {
            return GetBySqlInternal(typeof(T), null, null).Cast<T>();
        }

        public IEnumerable<T> GetBySql<T>(string wherePredicate)
        {
            return GetBySqlInternal(typeof(T), $" where {wherePredicate}", null).Cast<T>();
        }

        public IEnumerable<T> GetBySql<T>(string wherePredicate, int top)
        {
            return GetBySqlInternal(typeof(T), $" where {wherePredicate}", $" top {top}").Cast<T>();
        }

        private IEnumerable<object> GetBySqlInternal(Type type, string whereClause, string topClause)
        {
            var metadata = EntityMetadata.Create(type);
            var dataTable = _pivotalDataAccess.GetDataTableBySql(
                $"select { topClause } { string.Join(", ", metadata.Columns.Select(c => c.Name)) } from { metadata.TableName } { whereClause }");
            foreach (DataRow row in dataTable.Rows)
            {
                yield return MapRowAndSecondariesToEntity(row, type, metadata);
            }
        }

        private static string GetParamsString(object[] parameters)
        {
            return string.Join(", ", parameters.Select(p => p ?? "null"));
        }
    }
}
