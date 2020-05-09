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
            this.memberAssignments = new Dictionary<string, Func<ParameterExpression, InvocationExpression>> ();
            propteryConverts = new Dictionary<string, Func<TIn, TOut, TOut>> ();
        }
        private Func<TIn, TOut> Convert;
        private Dictionary<string, Func<ParameterExpression, InvocationExpression>> memberAssignments;
        public Dictionary<string, Func<TIn, TOut, TOut>> propteryConverts;

        ///转换结果
        public TOut AutoMapper (TIn tIn)
        {

            if (Convert == null)
            {
                //多线程问题暂不考虑
                Convert = MapperExpressionCommon.GetConvertFunc<TIn, TOut> (memberAssignments,propteryConverts.Keys.ToHashSet());
            }
            var tout = Convert (tIn);
            foreach (var item in this.propteryConverts)
            {
                tout = item.Value (tIn, tout);
            }
            return tout;
        }
        /// <summary>
        /// 从转换对象处理
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="convertExpression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public Mapper<TIn, TOut> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Expression<Func<TIn, TProperty>> convertExpression)
        {

            this.Add<TProperty> (expression, (a) =>
            {

                return Expression.Invoke (convertExpression, a);
            });
            return this;

        }

        /// <summary>
        /// lambda数据映射
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public PropertyMapper<TIn, TOut, TProperty> PropertyMap<TProperty> (Expression<Func<TOut, TProperty>> expression)
        {
            var memberExpesssion= expression.Body as MemberExpression;
            return new PropertyMapper<TIn, TOut, TProperty>(memberExpesssion.Member.Name,this);

        }

        /// <summary>
        /// 自定义值
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="convertExpression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public Mapper<TIn, TOut> Property<TProperty> (Expression<Func<TOut, TProperty>> expression, Expression<Func<TProperty>> convertExpression)
        {
            this.Add<TProperty> (expression, a =>
            {
                return Expression.Invoke (convertExpression);
            });
            return this;
        }
        private void Add<TProperty> (Expression<Func<TOut, TProperty>> expression, Func<ParameterExpression, InvocationExpression> invokeFunc)
        {
            var memberExpesssion = expression.Body as MemberExpression;
            this.memberAssignments.Add (memberExpesssion.Member.Name, invokeFunc);
        }
    }
    public class PropertyMapper<TIn, TOut, TProterty> where TIn : class where TOut : class
    {
        private Mapper<TIn, TOut> mapper;
        private string propterty;
        public PropertyMapper (string propterty, Mapper<TIn, TOut> mapper)
        {
            this.mapper = mapper;
            this.propterty = propterty;
        }
        public Mapper<TIn, TOut> Map (Func<TIn, TProterty> convert)
        {
            this.Add((a,b)=>convert(a));
            return this.mapper;
        }
        private void Add(Func<TIn,TOut,TProterty> convert)
        {
            Func<TIn, TOut, TOut> func = (a, b) =>
            {
                var value = convert (a,b);
               var assignFunc = MapperExpressionCommon.AssignFunc<TOut,TProterty>(this.propterty);
                assignFunc(b,value);
                return b;
            };
            this.mapper.propteryConverts.Add (propterty, func);
        }
        /// <summary>
        /// 自定义对象
        /// </summary>
        /// <param name="convert"></param>
        /// <returns></returns>
        public Mapper<TIn, TOut> Map (Func<TProterty> convert)
        {
           this.Add((a,b)=>convert());
            return this.mapper;
        }
        
    }

}