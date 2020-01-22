using System;

namespace PivotalORM
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; private set; }

        public ColumnAttribute(string name = null)
        {
            Name = name;
        }
    }
}
