using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SmartWalk.Shared.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName) != null;
        }

        public static bool HasProperty(this object objectToCheck, string propertyName)
        {
            var type = objectToCheck.GetType();
            return type.GetProperties().Any(p => p.Name.Equals(propertyName));
        }

        /// <summary>
        /// Gets the field represented by the lambda expression.
        /// </summary>
        /// <typeparam name="TTarget">
        /// The type of the target.
        /// </typeparam>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.Reflection.FieldInfo"/>
        ///     .
        /// </returns>
        public static FieldInfo GetField<TTarget>([InstantHandle]Expression<Func<TTarget, object>> field)
        {
            return Reflection<TTarget>.GetField(field);
        }

        public static FieldInfo GetField<TTarget>(this TTarget obj, [InstantHandle]Expression<Func<TTarget, object>> field)
        {
            return GetField(field);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="TTarget">
        /// The type of the target.
        /// </typeparam>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo"/>
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The
        ///     <paramref name="method"/>
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The
        ///     <paramref name="method"/>
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<TTarget>([InstantHandle]Expression<Action<TTarget>> method)
        {
            return Reflection<TTarget>.GetMethod(method);
        }

        public static MethodInfo GetMethod<TTarget>(this TTarget obj, [InstantHandle]Expression<Action<TTarget>> method)
        {
            return GetMethod(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="TTarget">
        /// The type of the target.
        /// </typeparam>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo"/>
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The
        ///     <paramref name="method"/>
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The
        ///     <paramref name="method"/>
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<TTarget, T1>([InstantHandle]Expression<Action<TTarget, T1>> method)
        {
            return Reflection<TTarget>.GetMethod(method);
        }

        public static MethodInfo GetMethod<TTarget, T1>(this TTarget obj, [InstantHandle]Expression<Action<TTarget, T1>> method)
        {
            return GetMethod(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="TTarget">
        /// The type of the target.
        /// </typeparam>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo"/>
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The
        ///     <paramref name="method"/>
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The
        ///     <paramref name="method"/>
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<TTarget, T1, T2>([InstantHandle]Expression<Action<TTarget, T1, T2>> method)
        {
            return Reflection<TTarget>.GetMethod(method);
        }

        public static MethodInfo GetMethod<TTarget, T1, T2>(this TTarget obj, [InstantHandle]Expression<Action<TTarget, T1, T2>> method)
        {
            return GetMethod(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="TTarget">
        /// The type of the target.
        /// </typeparam>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo"/>
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The
        ///     <paramref name="method"/>
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The
        ///     <paramref name="method"/>
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<TTarget, T1, T2, T3>([InstantHandle]Expression<Action<TTarget, T1, T2, T3>> method)
        {
            return Reflection<TTarget>.GetMethod(method);
        }

        public static MethodInfo GetMethod<TTarget, T1, T2, T3>(
            this TTarget obj,
            [InstantHandle]Expression<Action<TTarget, T1, T2, T3>> method)
        {
            return GetMethod(method);
        }

        /// <summary>
        /// Get path represented by <see cref="PropertyInfo"/>'s.
        /// </summary>
        /// <param name="propertyInfos">
        /// <see cref="PropertyInfo"/>'s for concatenation.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath(params PropertyInfo[] propertyInfos)
        {
            var stringBuilder = new StringBuilder();
            foreach (var info in propertyInfos)
            {
                stringBuilder.AppendFormat("{0}.", info.Name);
            }

            stringBuilder.Length--;
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1>([InstantHandle]Expression<Func<T1, object>> property1)
        {
            return GetPath(GetProperty(property1));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2)
        {
            return GetPath(GetProperty(property1), GetProperty(property2));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3)
        {
            return GetPath(GetProperty(property1), GetProperty(property2), GetProperty(property3));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <typeparam name="T5">
        /// The type of the 5.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <param name="property5">
        /// The property5.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4, T5>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4,
            [InstantHandle]Expression<Func<T5, object>> property5)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4),
                GetProperty(property5));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <typeparam name="T5">
        /// The type of the 5.
        /// </typeparam>
        /// <typeparam name="T6">
        /// The type of the 6.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <param name="property5">
        /// The property5.
        /// </param>
        /// <param name="property6">
        /// The property6.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4, T5, T6>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4,
            [InstantHandle]Expression<Func<T5, object>> property5,
            [InstantHandle]Expression<Func<T6, object>> property6)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4),
                GetProperty(property5),
                GetProperty(property6));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <typeparam name="T5">
        /// The type of the 5.
        /// </typeparam>
        /// <typeparam name="T6">
        /// The type of the 6.
        /// </typeparam>
        /// <typeparam name="T7">
        /// The type of the 7.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <param name="property5">
        /// The property5.
        /// </param>
        /// <param name="property6">
        /// The property6.
        /// </param>
        /// <param name="property7">
        /// The property7.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4, T5, T6, T7>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4,
            [InstantHandle]Expression<Func<T5, object>> property5,
            [InstantHandle]Expression<Func<T6, object>> property6,
            [InstantHandle]Expression<Func<T7, object>> property7)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4),
                GetProperty(property5),
                GetProperty(property6),
                GetProperty(property7));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <typeparam name="T5">
        /// The type of the 5.
        /// </typeparam>
        /// <typeparam name="T6">
        /// The type of the 6.
        /// </typeparam>
        /// <typeparam name="T7">
        /// The type of the 7.
        /// </typeparam>
        /// <typeparam name="T8">
        /// The type of the 8.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <param name="property5">
        /// The property5.
        /// </param>
        /// <param name="property6">
        /// The property6.
        /// </param>
        /// <param name="property7">
        /// The property7.
        /// </param>
        /// <param name="property8">
        /// The property8.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4, T5, T6, T7, T8>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4,
            [InstantHandle]Expression<Func<T5, object>> property5,
            [InstantHandle]Expression<Func<T6, object>> property6,
            [InstantHandle]Expression<Func<T7, object>> property7,
            [InstantHandle]Expression<Func<T8, object>> property8)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4),
                GetProperty(property5),
                GetProperty(property6),
                GetProperty(property7),
                GetProperty(property8));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <typeparam name="T5">
        /// The type of the 5.
        /// </typeparam>
        /// <typeparam name="T6">
        /// The type of the 6.
        /// </typeparam>
        /// <typeparam name="T7">
        /// The type of the 7.
        /// </typeparam>
        /// <typeparam name="T8">
        /// The type of the 8.
        /// </typeparam>
        /// <typeparam name="T9">
        /// The type of the 9.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <param name="property5">
        /// The property5.
        /// </param>
        /// <param name="property6">
        /// The property6.
        /// </param>
        /// <param name="property7">
        /// The property7.
        /// </param>
        /// <param name="property8">
        /// The property8.
        /// </param>
        /// <param name="property9">
        /// The property9.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4,
            [InstantHandle]Expression<Func<T5, object>> property5,
            [InstantHandle]Expression<Func<T6, object>> property6,
            [InstantHandle]Expression<Func<T7, object>> property7,
            [InstantHandle]Expression<Func<T8, object>> property8,
            [InstantHandle]Expression<Func<T9, object>> property9)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4),
                GetProperty(property5),
                GetProperty(property6),
                GetProperty(property7),
                GetProperty(property8),
                GetProperty(property9));
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the 1.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the 2.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the 3.
        /// </typeparam>
        /// <typeparam name="T4">
        /// The type of the 4.
        /// </typeparam>
        /// <typeparam name="T5">
        /// The type of the 5.
        /// </typeparam>
        /// <typeparam name="T6">
        /// The type of the 6.
        /// </typeparam>
        /// <typeparam name="T7">
        /// The type of the 7.
        /// </typeparam>
        /// <typeparam name="T8">
        /// The type of the 8.
        /// </typeparam>
        /// <typeparam name="T9">
        /// The type of the 9.
        /// </typeparam>
        /// <typeparam name="T10">
        /// The type of the 10.
        /// </typeparam>
        /// <param name="property1">
        /// The property1.
        /// </param>
        /// <param name="property2">
        /// The property2.
        /// </param>
        /// <param name="property3">
        /// The property3.
        /// </param>
        /// <param name="property4">
        /// The property4.
        /// </param>
        /// <param name="property5">
        /// The property5.
        /// </param>
        /// <param name="property6">
        /// The property6.
        /// </param>
        /// <param name="property7">
        /// The property7.
        /// </param>
        /// <param name="property8">
        /// The property8.
        /// </param>
        /// <param name="property9">
        /// The property9.
        /// </param>
        /// <param name="property10">
        /// The property10.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.String"/>
        ///     .
        /// </returns>
        public static string GetPath<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            [InstantHandle]Expression<Func<T1, object>> property1,
            [InstantHandle]Expression<Func<T2, object>> property2,
            [InstantHandle]Expression<Func<T3, object>> property3,
            [InstantHandle]Expression<Func<T4, object>> property4,
            [InstantHandle]Expression<Func<T5, object>> property5,
            [InstantHandle]Expression<Func<T6, object>> property6,
            [InstantHandle]Expression<Func<T7, object>> property7,
            [InstantHandle]Expression<Func<T8, object>> property8,
            [InstantHandle]Expression<Func<T9, object>> property9,
            [InstantHandle]Expression<Func<T10, object>> property10)
        {
            return GetPath(
                GetProperty(property1),
                GetProperty(property2),
                GetProperty(property3),
                GetProperty(property4),
                GetProperty(property5),
                GetProperty(property6),
                GetProperty(property7),
                GetProperty(property8),
                GetProperty(property9),
                GetProperty(property10));
        }

        /// <summary>
        /// Gets the property represented by the lambda expression.
        /// </summary>
        /// <typeparam name="TTarget">
        /// The type of the target.
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// Returns an instance of
        ///     <see cref="System.Reflection.PropertyInfo"/>
        ///     .
        /// </returns>
        public static PropertyInfo GetProperty<TTarget>([InstantHandle]Expression<Func<TTarget, object>> property)
        {
            return Reflection<TTarget>.GetProperty(property);
        }

        public static PropertyInfo GetProperty<TTarget>(this TTarget obj, [InstantHandle]Expression<Func<TTarget, object>> property)
        {
            return GetProperty(property);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <returns>
        /// The name of the property.
        /// </returns>
        public static string GetPropertyName<TTarget>(this TTarget obj, [InstantHandle]Expression<Func<TTarget, object>> property)
        {
            var propertyInfo = GetProperty(property);
            return propertyInfo.Name;
        }

        /// The method is taken from http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx
        /// <summary>
        /// Gets a type of elements in Enumerable.
        /// </summary>
        public static Type GetElementType(Type seqType)
        {
            var ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string)) return null;
            if (seqType.IsArray) return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (var arg in seqType.GetGenericArguments())
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            var ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (var iface in ifaces)
                {
                    var ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null &&
                seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }
}
