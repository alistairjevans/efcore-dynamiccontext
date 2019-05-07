using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicContext
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsesDbModelAttribute : Attribute
    {
        public Type[] ModelTypes { get; }

        public UsesDbModelAttribute(params Type[] modelTypes)
        {
            ModelTypes = modelTypes;
        }
    }
}
