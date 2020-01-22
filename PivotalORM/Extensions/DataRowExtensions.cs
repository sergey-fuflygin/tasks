using System.Data;

namespace PivotalORM
{
    public static class DataRowExtensions
    {
        public static bool ColumnChanged(this DataRow dr, DataColumn dc)
        {
            return !dr[dc, DataRowVersion.Original].Equals(dr[dc, DataRowVersion.Current]);
        }       
    }
}
