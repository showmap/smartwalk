using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartWalk.Shared.Resources;

namespace SmartWalk.Shared.Extensions
{
    /// <summary>
    ///     Provides strong-typed reflection of the
    ///     <typeparamref name="TTarget" />
    ///     type.
    /// </summary>
    /// <typeparam name="TTarget">Type to reflect.</typeparam>
    public static class Reflection<TTarget>
    {
        /// <summary>
        ///     Gets the field represented by the lambda expression.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.FieldInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Member is not a field.
        /// </exception>
        public static FieldInfo GetField([InstantHandle] Expression<Func<TTarget, object>> field)
        {
            var info = GetMemberInfo(field) as FieldInfo;
            if (info == null)
            {
                throw new ArgumentException(Localization.MemberIsNotAField);
            }

            return info;
        }

        /// <summary>
        ///     Gets the method represented by the lambda expression.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="method" />
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The
        ///     <paramref name="method" />
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod([InstantHandle] Expression<Action<TTarget>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        ///     Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="method" />
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The
        ///     <paramref name="method" />
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<T1>([InstantHandle] Expression<Action<TTarget, T1>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        ///     Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="method" />
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The
        ///     <paramref name="method" />
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<T1, T2>([InstantHandle] Expression<Action<TTarget, T1, T2>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        ///     Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="method" />
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The
        ///     <paramref name="method" />
        ///     is not a lambda expression or it does not represent a method invocation.
        /// </exception>
        public static MethodInfo GetMethod<T1, T2, T3>([InstantHandle] Expression<Action<TTarget, T1, T2, T3>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        ///     Get path represented by <see cref="PropertyInfo"/>'s.
        /// </summary>
        /// <param name="propertyInfos">
        ///     <see cref="PropertyInfo"/>'s for concatenation.
        /// </param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.String" />
        ///     containing concatenated property names.
        /// </returns>
        public static string GetPath(params PropertyInfo[] propertyInfos)
        {
            return ReflectionExtensions.GetPath(propertyInfos);
        }

        /// <summary>
        ///     Gets the property represented by the lambda expression.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.PropertyInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Member is not a property.
        /// </exception>
        public static PropertyInfo GetProperty([InstantHandle] Expression<Func<TTarget, object>> property)
        {
            var info = GetMemberInfo(property) as PropertyInfo;
            if (info == null)
            {
                throw new ArgumentException(Localization.MemberIsNotAProperty);
            }

            return info;
        }

        public static bool HasProperty(string propertyName)
        {
            return typeof(TTarget).GetProperties().Any(p => p.Name.EqualsIgnoreCase(propertyName));
        }

        /// <summary>
        ///     Gets the member info.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.MemberInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <c>member</c>
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Not a lambda expression
        /// </exception>
        private static MemberInfo GetMemberInfo(Expression member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            var lambda = member as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentException(Localization.NotALambdaExpression, "member");
            }

            MemberExpression memberExpr = null;

            // The Func<TTarget, object> we use returns an object, so first statement can be either 
            // a cast (if the field/property does not return an object) or the direct member access.
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                // The cast is an unary expression, where the operand is the 
                // actual member access expression.
                memberExpr = ((UnaryExpression) lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
            {
                throw new ArgumentException(Localization.NotAMemberAccess, "member");
            }

            return memberExpr.Member;
        }

        /// <summary>
        ///     Gets the method info.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="System.Reflection.MethodInfo" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <c>method</c>
        ///     is
        ///     <c>null</c>
        ///     .
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Not a lambda expression.
        /// </exception>
        /// <exception cref="ArgumentException">Not a method call.</exception>
        private static MethodInfo GetMethodInfo(Expression method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var lambda = method as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentException(Localization.NotALambdaExpression, "method");
            }

            if (lambda.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException(Localization.NotAMethodCall, "method");
            }

            return ((MethodCallExpression) lambda.Body).Method;
        }
    }
}