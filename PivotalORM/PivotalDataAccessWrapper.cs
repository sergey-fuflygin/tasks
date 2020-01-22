using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PivotalORM.Interfaces;
using System.Data;
using CdcSoftware.Pivotal.Applications.Foundation.Server;
using CdcSoftware.Pivotal.Engine;

namespace PivotalORM
{
    internal class PivotalDataAccessWrapper : IPivotalDataAccess
    {
        private DataAccess _pivotalDataAccess;

        public PivotalDataAccessWrapper(DataAccess pivotalDataAccess)
        {
            _pivotalDataAccess = pivotalDataAccess;
        }

        public DataRow GetNewDataRow(string tableName, params string[] resultColumnNames)
        {
            return _pivotalDataAccess.GetNewDataRow(tableName, resultColumnNames);
        }

        public DataTable GetNewDataTable(string tableName, params string[] resultColumnNames)
        {
            return _pivotalDataAccess.GetNewDataTable(tableName, resultColumnNames);
        }

        public DataRow SaveDataRow(DataRow dataRow)
        {
            return _pivotalDataAccess.SaveDataRow(dataRow);
        }

        public DataTable SaveDataTable(DataTable dataTable)
        {
            return _pivotalDataAccess.SaveDataTable(dataTable);
        }

        public DataTable GetDataTable(string sqlText)
        {
            return _pivotalDataAccess.GetDataTable(sqlText);
        }

        public DataTable GetDataTable(string queryName, object[] parameters, params string[] columnNames)
        {
            return _pivotalDataAccess.GetDataTable(queryName, parameters, columnNames);
        }

        public DataRow GetDataRow(string tableName, Id recordId, params string[] resultColumnNames)
        {
            return _pivotalDataAccess.GetDataRow(tableName, recordId, resultColumnNames);
        }

        //public Id SqlFind(string tableName, string filterColumnName, object filterColumnValue)
        //{
        //    return _pivotalDataAccess.SqlFind(tableName, filterColumnName, filterColumnValue);
        //}

        public DataTable GetLinkedDataTable(string tableName, string foreignKeyColumnName, Id foreignKeyId, params string[] columnNames)
        {
            return _pivotalDataAccess.GetLinkedDataTable(tableName, foreignKeyColumnName, foreignKeyId, columnNames);
        }

        public DataTable GetDataTableBySql(string sql)
        {
            return _pivotalDataAccess.GetDataTable(sql);
        }

        public void Delete(string tableName, Id id)
        {
            _pivotalDataAccess.DeleteRecord(id, tableName);
        }
    }
}
