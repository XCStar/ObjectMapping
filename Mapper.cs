using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using ObjectMapping.Attributes;

namespace ObjectMapping
{
    public class Mapper<TIn, TOut> where TIn : class where TOut : class
    {
        public Mapper ()
        {
            this.memberAssignments = new Dictionary<string, Func<ParameterExpression, ParameterExpression, InvocationExpression>> ();
        }
        private  Func<TIn, TOut> Convert;
        private Dictionary<string, Func<ParameterExpression, ParameterExpression, InvocationExpression>> memberAssignments;

        ///转换结果
        public TOut AutoMapper (TIn tIn)
        {

            if (Convert == null)
            {
                //多线程问题暂不考虑
                Convert = MapperExpressionCommon.GetConvertFunc<TIn, TOut> (memberAssignments);
            }
            var tout = Convert (tIn);
            return tout;
        }
        /// <summary>
        /// 从转换对象处理
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="convertExpression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public Mapper<TIn,TOut> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Expression<Func<TIn, TProperty>> convertExpression)
        {

            this.Add<TProperty> (expression, (a, b) =>
            {

                return Expression.Invoke (convertExpression, a);
            });
            return this;

        }
        /// <summary>
        /// 自定义处理
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="convertExpression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public Mapper< TIn,TOut> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Expression<Func<TIn, TOut, TProperty>> convertExpression)
        {
            this.Add<TProperty> (expression, (a, b) =>
            {

                return Expression.Invoke (convertExpression, a, b);
            });
            return this;
        }

        /// <summary>
        /// 自定义值
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="convertExpression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public Mapper<TIn,TOut> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Expression<Func<TProperty>> convertExpression)
        {
            this.Add<TProperty> (expression, (a, b) =>
            {
                return Expression.Invoke (convertExpression);
            });
            return this;
        }
        private void Add<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<ParameterExpression, ParameterExpression, InvocationExpression> invokeFunc)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            this.memberAssignments.Add (memberExpesssion.Member.Name, invokeFunc);
        }
    }
}