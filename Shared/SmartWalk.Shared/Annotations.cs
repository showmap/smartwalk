using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SmartWalk.Shared
{
    /// <summary>
    ///     Indicates that marked method builds string by format pattern and (optional) arguments.
    ///     Parameter, which contains format string, should be given in constructor.
    ///     The format string should be in <see cref="string.Format(IFormatProvider,string,object[])" /> -like form.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class StringFormatMethodAttribute : Attribute
    {
        private readonly string _myFormatParameterName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringFormatMethodAttribute" /> class.
        /// </summary>
        /// <param name="formatParameterName">Specifies which parameter of an annotated method should be treated as format-string.</param>
        public StringFormatMethodAttribute(string formatParameterName)
        {
            _myFormatParameterName = formatParameterName;
        }

        /// <summary>
        ///     Gets format parameter name.
        /// </summary>
        public string FormatParameterName
        {
            [DebuggerStepThrough] get { return _myFormatParameterName; }
        }
    }

    /// <summary>
    /// Indicates that the function argument should be string literal and match one  of the parameters of the caller function.
    /// For example, <see cref="ArgumentNullException" /> has such parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InvokerParameterNameAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that the marked method unconditionally terminates control flow execution.
    /// For example, it could unconditionally throw exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TerminatesProgramAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that the value of marked element could be <c>null</c> sometimes, so the check for <c>null</c> is necessary before its usage.
    /// See <seealso cref="NotNullExpectedAttribute" />.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
        AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CanBeNullAttribute : Attribute
    {
    }

    /// <summary>
    ///     Indicates that the value of marked element could never be <c>null</c>.
    ///     See <seealso cref="NotNullExpectedAttribute" />.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
        AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NotNullAttribute : Attribute
    {
    }

    /// <summary>
    ///     Unlike <see cref="NotNullAttribute" /> the value marked with attribute is expected to be not <c>null</c>
    ///     but not validated for <see langword="null" />.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
        AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NotNullExpectedAttribute : Attribute
    {
    }

    /// <summary>
    /// When .NET 4.5 appears, replace this with <code>[MethodImpl(MethodImplOptions.AggressiveInlining)]</code>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class InlineAttribute : Attribute
    {
    }

    /// <summary>
    /// Mark code for review.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class ReviewThisCodeAttribute : Attribute
    {
        /// <summary>
        /// Reason for review.
        /// </summary>
        public readonly string Reason;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewThisCodeAttribute"/> class with specified reason.
        /// </summary>
        public ReviewThisCodeAttribute(string reason)
        {
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewThisCodeAttribute"/> class with no reason.
        /// </summary>
        public ReviewThisCodeAttribute()
        {
        }
    }

    /// <summary>
    ///     Indicates that the value of marked type (or its derivatives) cannot be compared using '==' or '!=' operators.
    ///     There is only exception to compare with <c>null</c>, it is permitted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false
        , Inherited = true)]
    public sealed class CannotApplyEqualityOperatorAttribute : Attribute
    {
    }

    /// <summary>
    ///     When applied to target attribute, specifies a requirement for any type which is marked with
    ///     target attribute to implement or inherit specific type or types.
    /// </summary>
    /// <example>
    ///     <code>
    /// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
    /// public class ComponentAttribute : Attribute 
    /// {}
    /// 
    /// [Component] // ComponentAttribute requires implementing IComponent interface
    /// public class MyComponent : IComponent
    /// {}
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [BaseTypeRequired(typeof (Attribute))]
    public sealed class BaseTypeRequiredAttribute : Attribute
    {
        private readonly Type[] _myBaseTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeRequiredAttribute"/> class.
        /// </summary>
        /// <param name="baseTypes">Specifies which types are required.</param>
        public BaseTypeRequiredAttribute(params Type[] baseTypes)
        {
            _myBaseTypes = baseTypes;
        }

        /// <summary>
        /// Gets enumerations of specified base types.
        /// </summary>
        public IEnumerable<Type> BaseTypes
        {
            [DebuggerStepThrough] get { return _myBaseTypes; }
        }
    }

    /// <summary>
    ///     Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
    ///     so this symbol will not be marked as unused (as well as by other usage inspections).
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class UsedImplicitlyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
        /// </summary>
        public UsedImplicitlyAttribute()
            : this(ImplicitUseFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class with specified flags.
        /// </summary>
        /// <param name="flags">Value of type <see cref="ImplicitUseFlags" /> indicating usage kind.</param>
        public UsedImplicitlyAttribute(ImplicitUseFlags flags)
        {
            Flags = flags;
        }

        /// <summary>
        ///     Gets value indicating what is meant to be used.
        /// </summary>
        [UsedImplicitly]
        public ImplicitUseFlags Flags { get; private set; }
    }

    /// <summary>
    /// Should be used on attributes and causes ReSharper to not mark symbols marked with such attributes as unused (as well as by other usage inspections).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MeansImplicitUseAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
        /// </summary>
        [UsedImplicitly]
        public MeansImplicitUseAttribute()
            : this(ImplicitUseFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class with specified flags.
        /// </summary>
        /// <param name="flags">Value of type <see cref="ImplicitUseFlags" /> indicating usage kind.</param>
        [UsedImplicitly]
        public MeansImplicitUseAttribute(ImplicitUseFlags flags)
        {
            Flags = flags;
        }

        /// <summary>
        /// Gets value indicating what is meant to be used.
        /// </summary>
        [UsedImplicitly]
        public ImplicitUseFlags Flags { get; private set; }
    }

    /// <summary>
    /// Describes dependency between method input and output.
    /// </summary>
    /// <syntax>
    /// <p>Function Definition Table syntax:</p>
    /// <list>
    /// <item>FDT      ::= FDTRow [;FDTRow]*</item>
    /// <item>FDTRow   ::= Input =&gt; Output | Output &lt;= Input</item>
    /// <item>Input    ::= ParameterName: Value [, Input]*</item>
    /// <item>Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
    /// <item>Value    ::= true | false | null | notnull | canbenull</item>
    /// </list>
    /// If method has single input parameter, it's name could be omitted. <br/>
    /// Using <c>halt</c> (or <c>void</c>/<c>nothing</c>, which is the same) for method output means that the methos doesn't return normally. <br/>
    /// <c>canbenull</c> annotation is only applicable for output parameters. <br/>
    /// You can use multiple <c>[ContractAnnotation]</c> for each FDT row, or use single attribute with rows separated by semicolon. <br/>
    /// </syntax>
    /// <examples>
    /// <list>
    /// <item><code>
    /// [ContractAnnotation("=> halt")]
    /// public void TerminationMethod()
    /// </code></item>
    /// <item><code>
    /// [ContractAnnotation("halt &lt;= condition: false")]
    /// public void Assert(bool condition, string text) // Regular Assertion method
    /// </code></item>
    /// <item><code>
    /// [ContractAnnotation("s:null => true")]
    /// public bool IsNullOrEmpty(string s) // String.IsNullOrEmpty
    /// </code></item>
    /// <item><code>
    /// // A method that returns null if the parameter is null, and not null if the parameter is not null
    /// [ContractAnnotation("null => null; notnull => notnull")]
    /// public object Transform(object data) 
    /// </code></item>
    /// <item><code>
    /// [ContractAnnotation("s:null=>false; =>true,result:notnull; =>false, result:null")]
    /// public bool TryParse(string s, out Person result)
    /// </code></item>
    /// </list>
    /// </examples>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ContractAnnotationAttribute : Attribute
    {
        public ContractAnnotationAttribute([NotNull] string fdt)
            : this(fdt, false)
        {
        }

        public ContractAnnotationAttribute([NotNull] string fdt, bool forceFullStates)
        {
            FDT = fdt;
            ForceFullStates = forceFullStates;
        }

        // ReSharper disable InconsistentNaming
        public string FDT { get; private set; }
        // ReSharper restore InconsistentNaming
        public bool ForceFullStates { get; private set; }
    }

    /// <summary>
    /// Tells code analysis engine if the parameter is completely handled when the invoked method is on stack. 
    /// If the parameter is a delegate, indicates that delegate is executed while the method is executed.
    /// If the parameter is an enumerable, indicates that it is enumerated while the method is executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
    public sealed class InstantHandleAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that a method does not make any observable state changes.
    /// The same as <see cref="System.Diagnostics.Contracts.PureAttribute"/>
    /// </summary>
    /// <example>
    /// <code>
    /// [Pure]
    /// private int Multiply(int x, int y)
    /// {
    ///   return x*y;
    /// }
    ///
    /// public void Foo()
    /// {
    ///   const int a=2, b=2;
    ///   Multiply(a, b); // Waring: Return value of pure method is not used
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class PureAttribute : Attribute
    {
    }

    /// <summary>
    /// Specify what is considered used implicitly when marked with <see cref="MeansImplicitUseAttribute" /> or <see cref="UsedImplicitlyAttribute" />.
    /// </summary>
    [Flags]
    public enum ImplicitUseFlags
    {
        /// <summary>
        /// Only entity marked with attribute considered used.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Entity marked with attribute and all its members considered used.
        /// </summary>
        IncludeMembers = 1
    }
}
