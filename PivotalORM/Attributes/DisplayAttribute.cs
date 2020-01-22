using System;

namespace PivotalORM
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayAttribute : Attribute
    {
        public string Name { get; private set; }

        public DisplayAttribute(string name = "")
        {
            Name = name;
        }
    }
}
