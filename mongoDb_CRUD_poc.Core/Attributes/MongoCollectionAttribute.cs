using System;

namespace mongoDb_CRUD_poc.Core.Attributes;
/// <summary>
/// Light weight class to allow MongoDB seeder to interpret collection names
/// </summary>
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

