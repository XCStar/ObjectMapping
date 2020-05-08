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
        public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TIn, TProperty> convert)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            var valuefun = GetPropertyFunc<TProperty> (memberExpesssion.Member.Name);
            proptertyMapping.Add (memberExpesssion.Member.Name, (a, b) =>
            {
                var value = convert (a);
                valuefun (b, value);
                return b;
            });
            return this;
        }
        public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TIn, TOut, TProperty> convert)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            var valuefun = GetPropertyFunc<TProperty> (memberExpesssion.Member.Name);
            proptertyMapping.Add (memberExpesssion.Member.Name, (a, b) =>
            {
                var value = convert (a, b);

                valuefun (b, value);
                return b;
            });
            return this;
        }
        public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TOut, TProperty> convert)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            var valuefun = GetPropertyFunc<TProperty> (memberExpesssion.Member.Name);
            proptertyMapping.Add (memberExpesssion.Member.Name, (a, b) =>
            {
                var value = convert (b);

                valuefun (b, value);
                return b;
            });
            return this;
        }
        public Mapper<TOut, TIn> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<TProperty> convert)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            var valuefun = GetPropertyFunc<TProperty> (memberExpesssion.Member.Name);
            proptertyMapping.Add (memberExpesssion.Member.Name, (a, b) =>
            {
                var value = convert ();
                valuefun (b, value);
                return b;
            });
            return this;
        }
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