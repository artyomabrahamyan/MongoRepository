using System;

namespace Common.Mongo.Attributes
{
    /// <summary>
    /// This attribute allows to specify of the name of the collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string name) => Name = name;

        public string Name { get; private set; }
    }
}
