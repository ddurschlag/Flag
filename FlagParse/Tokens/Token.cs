using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Tokens
{
    [Serializable]
    public abstract class Token : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("s", ToString());
        }

        internal abstract void Accept(TokenVisitor v);
        internal abstract T Accept<T>(TokenVisitor<T> v);
    }
}
