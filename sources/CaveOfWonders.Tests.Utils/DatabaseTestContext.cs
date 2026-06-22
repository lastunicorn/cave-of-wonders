using System.Dynamic;

namespace DustInTheWind.CaveOfWonders.Tests.Utils;

public class DatabaseTestContext : DynamicObject
{
    private readonly Dictionary<string, object> values = [];

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return values.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        bool keyExists = values.ContainsKey(binder.Name);

        if (keyExists)
            values[binder.Name] = value;
        else
            values.Add(binder.Name, value);

        return true;
    }
}
