using CdcSoftware.Pivotal.Engine;
using System.Data;

namespace PivotalORM.Interfaces
{
    public interface IPivotalDataAccess
    {
        DataRow GetNewDataRow(string tableName, params string[] resultColumnNames);
        DataTable GetNewDataTable(string tableName, params string[] resultColumnNames);
        DataRow SaveDataRow(DataRow dataRow);
        DataTable SaveDataTable(DataTable dataTable);
        DataTable GetDataTable(string sqlText);
        DataTable GetDataTable(string queryName, object[] parameters, params string[] columnNames);
        DataRow GetDataRow(string tableName, Id recordId, params string[] resultColumnNames);
        //Id SqlFind(string tableName, string filterColumnName, object filterColumnValue);
        DataTable GetLinkedDataTable(string tableName, string foreignKeyColumnName, Id foreignKeyId, params string[] columnNames);
        DataTable GetDataTableBySql(string sql);
        void Delete(string tableName, Id id);
    }
}