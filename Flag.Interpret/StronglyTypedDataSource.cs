using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Flag.Interpret
{
    using Data;
    public class StronglyTypedDataSource
    {
        public Element AdaptString(string s)
        {
            return new StringElement(s);
        }

        public Element AdaptList(IEnumerable<object> l)
        {
            return new ListElement(l.Select(Adapt));
        }

        public Element AdaptMap(IDictionary<string, object> m)
        {
            return new MapElement(m.ToDictionary(kvp => kvp.Key, kvp => Adapt(kvp.Value)));
        }

        public Element Adapt(object o)
        {
            if (o is string)
                return AdaptString((string)o);
            if (o is IEnumerable<object>)
                return AdaptList((IEnumerable<object>)o);
            if (IsInstanceOfGenericType(typeof(Dictionary<,>), o))
            {
                var d = (IDictionary)o;
                return AdaptMap(d.Cast<dynamic>().ToDictionary(de => (string)de.Key, de => (object)de.Value));
            }
            throw new Exception("Can only adapt strings, enumerables, and dictionaries");
        }

        static bool IsInstanceOfGenericType(Type genericType, object instance)
        {
            Type type = instance.GetType();
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}
