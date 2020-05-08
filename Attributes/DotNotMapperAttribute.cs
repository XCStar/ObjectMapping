using System;
namespace ObjectMapping.Attributes
{

    [AttributeUsageAttribute(AttributeTargets.Property,Inherited=true)]
    public class DotNotMapperAttribute:Attribute
    {
        
    }
}