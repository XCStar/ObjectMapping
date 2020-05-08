using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectMapping.Attributes;

namespace ObjectMapping
{
    public class MapperExpressionCommon
    {
        public static Func<TIn, TOut> GetConvertFunc<TIn, TOut> (List<string> properties) where TIn : class where TOut : class
        {
            var inType = typeof (TIn);
            var outType = typeof (TOut);
            var outPropertyInfos = outType.GetProperties ();
            var outDic = outPropertyInfos.ToDictionary (t => t.Name, t => t);
            var inPropertyInfos = inType.GetProperties ();
            ParameterExpression parameter = Expression.Parameter (typeof (TIn), inType.Name);
            var memberBindings = new List<MemberBinding> ();
            foreach (PropertyInfo inProperty in inPropertyInfos)
            {
                if(properties.Contains(inProperty.Name))
                {
                    continue;
                }
                var dotNotAttribute = inProperty.GetCustomAttribute (typeof (DotNotMapperAttribute));
                if (dotNotAttribute != null)
                {
                    continue;
                }
                var mappAttribute = inProperty.GetCustomAttribute (typeof (MapperAttribute));
                var targetName = string.Empty;
                var propertyExprssion = Expression.Property (parameter, inProperty);
                if (mappAttribute != null)
                {
                    targetName = MapperAttribute.GetTargeName (inProperty);
                    if (!outDic.Keys.Contains (targetName))
                    {
                        throw new CustomAttributeFormatException ($"Not Found Mapping Name {targetName}");
                    }
                }
                else
                {
                    if (!outDic.Keys.Contains (inProperty.Name))
                    {
                        continue;
                    }
                    targetName = inProperty.Name;
                }

                
                var memberBinding = Expression.Bind (outDic[targetName], propertyExprssion);
                memberBindings.Add (memberBinding);
            }
            var memberInitExpression = Expression.MemberInit (Expression.New (typeof (TOut)), memberBindings);
            var expression= Expression.Lambda<Func<TIn, TOut>> (memberInitExpression, parameter);
            return expression.Compile();
        }
    }
}