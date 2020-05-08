using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
namespace ObjectMapping.Attributes
{

    [AttributeUsageAttribute(AttributeTargets.Property,Inherited=true)]
    public class MapperAttribute:Attribute
    {
        public string TargetName{get;set;}
     
        public MapperAttribute(string targetName)
        {
            this.TargetName=targetName;
        
        }
        public static string GetTargeName(PropertyInfo property)
        {
            var attr=property.GetCustomAttributes<MapperAttribute>(true).FirstOrDefault();
            if(attr==null)
            {
                return default(string);
            }
            return attr.TargetName;
        }
    }
}