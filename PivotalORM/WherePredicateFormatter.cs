using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PivotalORM
{
    public class WherePredicateFormatter
    {
        public static string Equal(string fieldName, byte[] id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return $"{ EnsureNoSqlInjectionInFieldName(fieldName) } = {FormatSqlBinary(id)}";
        }
        public static string Equal(string fieldName, string stringValue)
        {
            if (stringValue == null)
                return $"{ EnsureNoSqlInjectionInFieldName(fieldName) } is null";

            return $"{ EnsureNoSqlInjectionInFieldName(fieldName) } = { FormateSqlString(stringValue) }";
        }

        public static string NotEqual(string fieldName, string stringValue)
        {
            if (stringValue == null)
                return $"{ EnsureNoSqlInjectionInFieldName(fieldName) } is not null";

            return $"{ EnsureNoSqlInjectionInFieldName(fieldName) } <> { FormateSqlString(stringValue) }";
        }

        public static string And(string condition1, string condition2)
        {
            return $"({condition1}) AND ({condition2})";
        }

        public static string And(params string[] conditions)
        {
            if (conditions == null || conditions.Length < 2)
                throw new ArgumentException(nameof(conditions));
            return string.Join(" AND ", conditions.Select(c => $"({c})"));
        }

        private static string FormateSqlString(string stringValue)
        {
            if (stringValue == null)
                return "null";

            return $"'{ stringValue.Replace("'", "''") }'";
        }

        private static string EnsureNoSqlInjectionInFieldName(string fieldName)
        {
            if (!Regex.IsMatch(fieldName, @"^[\w\.]+$"))
                throw new Exception($"String {fieldName} is not a valid field name");
            return fieldName;
        }

        private static string FormatSqlBinary(byte[] id)
        {
            var idHex = "0x" + BitConverter.ToString(id).Replace("-", "");
            return $"CAST({ idHex } AS BINARY(8))";
        }
    }
}
