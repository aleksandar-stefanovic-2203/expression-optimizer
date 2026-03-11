/// <summary>
/// Contains a set of classes, interfaces, and utilities for building, representing, and optimizing mathematical expressions.
/// Supports constants, variables, binary operations, and functions. 
/// Includes an expression optimizer that removes duplicate sub-expressions to reduce redundancy in expression trees.
/// </summary>
namespace Expressions
{
    /// <summary>
    /// Base interface for all expression types.
    /// </summary>
    interface IExpression;

    /// <summary>
    /// Represents a constant numerical expression (e.g., 5).
    /// </summary>
    interface IConstantExpression : IExpression
    {
        /// <summary>The value of the constant.</summary>
        int Value { get; }
    }

    /// <summary>
    /// Represents a variable expression (e.g., x, y).
    /// </summary>
    interface IVariableExpression : IExpression
    {
        /// <summary>The name of the variable.</summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents a binary expression (e.g., a + b, x * 4).
    /// </summary>
    interface IBinaryExpression : IExpression
    {
        /// <summary>The left operand of the binary expression.</summary>
        IExpression Left { get; }

        /// <summary>The right operand of the binary expression.</summary>
        IExpression Right { get; }

        /// <summary>The operator of the binary expression.</summary>
        OperatorSign Sign { get; }
    }

    /// <summary>
    /// Represents a function expression (e.g., sin(x)).
    /// </summary>
    interface IFunction : IExpression
    {
        /// <summary>The kind of function (e.g., Sin, Cos, Max).</summary>
        FunctionKind Kind { get; }

        /// <summary>The argument of the function.</summary>
        IExpression Argument { get; }
    }

    /// <summary>
    /// Enumeration of supported function kinds.
    /// </summary>
    enum FunctionKind { Sin, Cos, Max }

    /// <summary>
    /// Enumeration of supported binary operators.
    /// </summary>
    enum OperatorSign { Plus, Minus, Multiply, Divide }

    /// <summary>
    /// Extension methods for <see cref="FunctionKind"/>.
    /// </summary>
    static class FunctionKindExtensions
    {
        /// <summary>
        /// Returns the string representation of the function name in lowercase.
        /// </summary>
        public static string AsName(this FunctionKind kind) => kind.ToString().ToLower();
    }

    /// <summary>
    /// Extension methods for <see cref="OperatorSign"/>.
    /// </summary>
    static class OperatorSignExtensions
    {
        /// <summary>
        /// Returns true if the operator is commutative (e.g., +, *).
        /// </summary>
        public static bool IsCommutative(this OperatorSign sign)
        {
            return sign == OperatorSign.Plus || sign == OperatorSign.Multiply;
        }

        /// <summary>
        /// Returns the string symbol of the operator (e.g., "+", "-", "*", "/").
        /// </summary>
        public static string AsSymbol(this OperatorSign sign) => sign switch
        {
            OperatorSign.Plus => "+",
            OperatorSign.Minus => "-",
            OperatorSign.Multiply => "*",
            OperatorSign.Divide => "/",
            _ => throw new Exception("Invalid operator sign")
        };
    }

    /// <summary>
    /// Represents a constant number in an expression tree.
    /// </summary>
    class ConstantExpression : IConstantExpression
    {
        /// <summary>The numeric value of this constant.</summary>
        public int Value { get; }

        /// <summary>Initializes a new constant expression with the given value (default 0).</summary>
        public ConstantExpression(int value = 0)
        {
            Value = value;
        }

        /// <summary>Returns the string representation of the constant.</summary>
        public override string ToString() => Value.ToString();
    }

    /// <summary>
    /// Represents a variable in an expression tree.
    /// </summary>
    class VariableExpression : IVariableExpression
    {
        /// <summary>The name of the variable.</summary>
        public string Name { get; }

        /// <summary>Initializes a new variable expression with the given name (default "x").</summary>
        public VariableExpression(string name = "x")
        {
            Name = name;
        }

        /// <summary>Returns the string representation of the variable.</summary>
        public override string ToString() => Name;
    }

    /// <summary>
    /// Represents a binary operation with left and right operands.
    /// </summary>
    class BinaryExpression : IBinaryExpression
    {
        /// <summary>The left operand of the binary expression.</summary>
        public IExpression Left { get; }

        /// <summary>The right operand of the binary expression.</summary>
        public IExpression Right { get; }

        /// <summary>The operator of the binary expression.</summary>
        public OperatorSign Sign { get; }

        /// <summary>Initializes a new binary expression with left, right operands and an operator.</summary>
        public BinaryExpression(IExpression leftExpression, IExpression rightExpression, OperatorSign sign)
        {
            Left = leftExpression ?? throw new ArgumentNullException(nameof(leftExpression));
            Right = rightExpression ?? throw new ArgumentNullException(nameof(rightExpression));
            Sign = sign;
        }

        /// <summary>
        /// Returns a string representation of the binary expression, ordering operands for commutative operators.
        /// </summary>
        public override string ToString()
        {
            string left = Left.ToString()!;
            string right = Right.ToString()!;

            if (Sign.IsCommutative())
            {
                if (string.Compare(left, right) > 0)
                {
                    var temp = left;
                    left = right;
                    right = temp;
                }
            }

            return $"({left} {Sign.AsSymbol()} {right})";
        }
    }

    /// <summary>
    /// Represents a function call (sin, cos, max, etc.) with a single argument.
    /// </summary>
    class Function : IFunction
    {
        /// <summary>The kind of function (e.g., Sin, Cos, Max).</summary>
        public FunctionKind Kind { get; }

        /// <summary>The argument of the function.</summary>
        public IExpression Argument { get; }

        /// <summary>Initializes a new function expression with the given kind and argument.</summary>
        public Function(FunctionKind kind, IExpression argument)
        {
            Kind = kind;
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        /// <summary>Returns the string representation of the function (e.g., "sin(x)").</summary>
        public override string ToString() => $"{Kind.AsName()}({Argument})";
    }

    /// <summary>
    /// Provides methods to optimize expression trees by removing duplicate sub-expressions.
    /// </summary>
    class ExpressionOptimizer
    {
        /// <summary>
        /// Optimizes the expression tree by removing duplicate sub-expressions.
        /// </summary>
        /// <param name="expression">The root expression to optimize.</param>
        /// <returns>An optimized expression tree with duplicates removed.</returns>
        public static IExpression? Optimize(IExpression? expression)
        {
            if (expression == null) return null;

            Dictionary<string, IExpression> cache = new();
            return RemoveDuplicates(expression, cache);
        }

        /// <summary>Recursively removes duplicate sub-expressions using a cache.</summary>
        protected static IExpression RemoveDuplicates(IExpression expression, Dictionary<string, IExpression> cache)
        {
            string key = expression.ToString()!;
            if (cache.TryGetValue(key, out IExpression? existingExpression)) return existingExpression;

            IExpression newExpression = CloneExpression(expression, cache);
            cache[key] = newExpression;
            return newExpression;
        }

        /// <summary>Clones an expression, recursively removing duplicates from children.</summary>
        protected static IExpression CloneExpression(IExpression expression, Dictionary<string, IExpression> cache) => expression switch
        {
            ConstantExpression c => new ConstantExpression(c.Value),
            VariableExpression v => new VariableExpression(v.Name),
            BinaryExpression b => new BinaryExpression(RemoveDuplicates(b.Left, cache), RemoveDuplicates(b.Right, cache), b.Sign),
            Function f => new Function(f.Kind, RemoveDuplicates(f.Argument, cache)),
            _ => throw new Exception("Invalid expression type")
        };
    }
}
