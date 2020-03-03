
using System;

namespace ShipDock.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ResolvableAttribute : Attribute
    {

        public ResolvableAttribute(string name)
        {
            Alias = name;
        }
        
        public string Alias { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CallableAttribute : ResolvableAttribute
    {
        public CallableAttribute(string resolverName, string alias) : base(alias)
        {
            ResolverName = resolverName;
        }

        public string ResolverName { get; private set; }
    }
}