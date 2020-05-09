using System.Runtime.CompilerServices;
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
        public static Func<TIn, TOut> GetConvertFunc<TIn, TOut> (Dictionary<string,Func<ParameterExpression,InvocationExpression>> properties,HashSet<string> complexPropterties) where TIn : class where TOut : class
        {
            //https://github.com/XCStar/ObjectMapping.git
            var inType = typeof (TIn);
            var outType = typeof (TOut);
            var outPropertyInfos = outType.GetProperties ();
            var outDic = outPropertyInfos.ToDictionary (t => t.Name, t => t);
            var inPropertyInfos = inType.GetProperties ();
            ParameterExpression parameter = Expression.Parameter (typeof (TIn), inType.Name);
            var memberBindings = new List<MemberBinding> ();
            foreach (PropertyInfo inProperty in inPropertyInfos)
            {
                if(complexPropterties.Contains(inProperty.Name))
                {
                    continue;
                }
                if(properties!=null&&properties.ContainsKey(inProperty.Name))
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
            foreach (var item in properties)
            {
                if(outDic.ContainsKey(item.Key))
                {
                    var memberBinding=Expression.Bind(outDic[item.Key],item.Value(parameter));
                    memberBindings.Add(memberBinding);
                }  
            }
            var memberInitExpression = Expression.MemberInit (Expression.New (typeof (TOut)), memberBindings);
            var expression= Expression.Lambda<Func<TIn, TOut>> (memberInitExpression, parameter);
            #if DEBUG
            // System.Console.WriteLine(expression);
            #endif
            return expression.Compile();
        }
        
        /// <summary>
        /// 给对象赋值
        /// </summary>
        /// <param name="proptertyValue"></param>
        /// <returns></returns>
        public static Action<TOut, TProterty> AssignFunc<TOut,TProterty> (string proptertyName)
        {
            var outType = typeof (TOut);
            var outParameterExpression = Expression.Parameter (outType, outType.Name);
            var proptertyType=typeof(TProterty);
            var proptertyParameterExpression=Expression.Parameter(proptertyType,proptertyType.Name);
            var proptertyExpression = Expression.Property (outParameterExpression, proptertyName);
            var assingExpression = Expression.Assign (proptertyExpression,proptertyParameterExpression);
            var expression = Expression.Lambda<Action<TOut, TProterty>> (assingExpression, outParameterExpression,proptertyParameterExpression);
            return expression.Compile ();
        }
    }
}