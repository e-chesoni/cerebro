using System;

namespace mongoDb_CRUD_poc.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class MongoCollectionAttribute : Attribute
{
    public string Name
    {
        get;
    }

    public MongoCollectionAttribute(string name)
    {
        Name = name;
    }
}

