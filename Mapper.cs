using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using ObjectMapping.Attributes;

namespace ObjectMapping
{
    public class Mapper<TOut, TIn> where TIn : class where TOut : class
    {
        public Mapper ()
        {
            this.proptertyMapping = new Dictionary<string, Func<TIn, TOut, TOut>> ();
        }
        private static Func<TIn, TOut> Convert;
        private Dictionary<string, Func<TIn, TOut, TOut>> proptertyMapping;

        ///转换结果
        public TOut AutoMapper (TIn tIn)
        {

            if (Convert == null)
            {
                //多线程问题暂不考虑
                Convert = MapperExpressionCommon.GetConvertFunc<TIn, TOut> (proptertyMapping.Keys.ToList ());
            }
            var tout = Convert (tIn);

            foreach (var item in proptertyMapping)
            {
                tout = item.Value.Invoke (tIn, tout);
            }
            return tout;
        }
        //从目标属性转换
        public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TIn, TProperty> convert)
        {
            this.Add(expression, (tin, tout) =>
            {
                return convert(tin);
            });
            return this;
       
        }
        ///自定义转换
        public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TIn, TOut, TProperty> convert)
        {
          this.Add(expression,(tin,tout)=>{
             return convert (tin,tout);
          });
            return this;
        }
        // public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TOut, TProperty> convert)
        // {
        //     this.Add(expression, (tin, tout) =>
        //     {
        //         return convert(tout);
        //     });
        //     return this;
        // }
        private void Add<TProperty>(Expression<Func<TOut, TProperty>> expression, Func<TIn,TOut,TProperty> func)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            var valuefun = GetPropertyFunc<TProperty>(memberExpesssion.Member.Name);
            proptertyMapping.Add(memberExpesssion.Member.Name, (a, b) =>
           {
               var value=func(a,b);
               valuefun(b, value);
               return b;
           });
        }
        ///自定义值
        public Mapper<TOut, TIn> Property<TProperty>(Expression<Func<TOut, TProperty>> expression, Func<TProperty> convert)
        {
            this.Add(expression, (tin, tout) =>
            {
                return convert();
            });
            return this;
        }
        ///获取赋值委托
        private Action<TOut, TProperty> GetPropertyFunc<TProperty> (string name)
        {
            var parameterExpression1 = Expression.Parameter (typeof (TOut), "a");
            var parameterExpression2 = Expression.Parameter (typeof (TProperty), "b");
            var propertyExpression = Expression.Property (parameterExpression1, name);
            var assignExpression = Expression.Assign (propertyExpression, parameterExpression2);
            var expression = Expression.Lambda<Action<TOut, TProperty>> (assignExpression, parameterExpression1, parameterExpression2);
            return expression.Compile ();
        }
    }
}