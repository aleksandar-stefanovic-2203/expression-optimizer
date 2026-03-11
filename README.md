# Expression Optimizer

A small C# library for building, representing, and optimizing mathematical expression trees.

## Overview

This project provides a framework for creating mathematical expression trees and optimizing them by eliminating redundant sub-expressions. The optimizer identifies duplicate nodes in the tree and ensures they reference the same object in memory, reducing redundancy and improving memory efficiency.

## Features

- **Expression Types**: Support for constants, variables, binary operations, and functions
- **Binary Operators**: Plus, Minus, Multiply, Divide
- **Functions**: Sin, Cos, Max
- **Expression Optimization**: Automatic detection and removal of duplicate sub-expressions
- **Commutative Operator Handling**: Smart ordering of operands for commutative operations (+ and *)

## Project Structure

```
ExpressionOptimizer/
├── lib/
│   └── Expressions.cs    # Core expression classes and optimizer
└── app/
    └── Program.cs        # Test cases and demonstration
```

## Expression Types

### Constants
```csharp
var five = new ConstantExpression(5);
```

### Variables
```csharp
var x = new VariableExpression("x");
```

### Binary Expressions
```csharp
var sum = new BinaryExpression(
    new ConstantExpression(2),
    new VariableExpression("x"),
    OperatorSign.Plus
); // Represents: 2 + x
```

### Functions
```csharp
var sinX = new Function(
    FunctionKind.Sin,
    new VariableExpression("x")
); // Represents: sin(x)
```

## Usage

### Basic Optimization

```csharp
using Expressions;

// Create an expression with duplicates: (x + x) + (x + x)
var expr = new BinaryExpression(
    new BinaryExpression(
        new VariableExpression("x"),
        new VariableExpression("x"),
        OperatorSign.Plus
    ),
    new BinaryExpression(
        new VariableExpression("x"),
        new VariableExpression("x"),
        OperatorSign.Plus
    ),
    OperatorSign.Plus
);

// Optimize the expression
var optimized = ExpressionOptimizer.Optimize(expr);

// The optimized expression will reuse identical sub-expressions
// reducing memory footprint
```

### Running the Tests

The project includes 10 comprehensive test cases demonstrating various optimization scenarios.

## Test Cases

1. **Simple constant** - Basic constant expression
2. **Simple variable** - Basic variable expression
3. **Duplicate constants** - Multiple instances of the same constant
4. **Commutative order swap** - Expressions like `(2 + x)` and `(x + 2)` that are equivalent
5. **Non-commutative order swap** - Expressions like `(2 - x)` and `(x - 2)` that are NOT equivalent
6. **Nested duplicates** - Complex nested structures with repeated patterns
7. **Function duplicates** - Repeated function calls
8. **Nested function duplicates** - Functions within functions
9. **Deep mixed structure** - Combinations of functions and operations
10. **Complex expression with shared subtree** - Real-world optimization scenario

## How It Works

The optimizer uses a string-based caching mechanism:

1. Converts each sub-expression to its string representation
2. Checks if an equivalent expression already exists in the cache
3. If found, reuses the existing expression object
4. If not found, clones the expression and adds it to the cache
5. Recursively processes all child expressions

### Complexity Analysis

- **n** = number of expressions in the expression tree
- **m** = average length of string representation per expression

**Time Complexity:** O(n × m)
- Each expression is visited once, and string operations (ToString, dictionary lookup) take O(m) time
- For typical balanced expression trees, this simplifies to approximately O(n log n)

**Space Complexity:** O(n × m)
- The cache dictionary stores at most n unique sub-expressions
- Each string key requires O(m) space
- The optimized expression tree requires O(n) space
- Recursion stack depth is O(h) where h is the tree height

### Commutative Operator Handling

For commutative operators (+ and *), the string representation automatically orders operands alphabetically, ensuring that expressions like `(2 + x)` and `(x + 2)` are recognized as duplicates:

```csharp
// Both produce the same string representation: "(2 + x)"
new BinaryExpression(new ConstantExpression(2), new VariableExpression("x"), OperatorSign.Plus)
new BinaryExpression(new VariableExpression("x"), new ConstantExpression(2), OperatorSign.Plus)
```

## Output Example

```
4. Commutative order swap (2 + x) and (x + 2):
Original expression: ((2 + x) + (2 + x))
Unique subexpressions (before): 5
Optimized expression: ((2 + x) + (2 + x))
Unique subexpressions (after): 3
Duplicates removed: 2
Memory saved: 40.00%
```

## Extensibility

The framework is designed to be easily extended:

- Add new operators to `OperatorSign` enum
- Add new functions to `FunctionKind` enum
- Implement new expression types by extending `IExpression`
- Customize optimization logic in `ExpressionOptimizer`

## License

This project is available for educational and commercial use.

## Requirements

- .NET 6.0 or higher
- C# 10.0 or higher