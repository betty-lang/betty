namespace Betty.Tests.InterpreterTests
{
    public class UserDefinedFunctionTests : InterpreterTestBase
    {
        [Fact]
        public void SimpleFunction_ReturnsConstantValue()
        {
            var code = @"
                func simple() { return 42; }
                func main() { return simple(); }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(42, result.AsNumber());
        }

        [Fact]
        public void FunctionWithParameters_CalculatesSum()
        {
            var code = @"
                func sum(a, b) { return a + b; }
                func main() { return sum(5, 7); }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(12, result.AsNumber());
        }

        [Fact]
        public void RecursiveFunction_CalculatesFactorial()
        {
            var code = @"
                func fact(n) {
                    if (n <= 1) { return 1; }
                    return n * fact(n - 1);
                }
                func main() { return fact(5); }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(120, result.AsNumber());
        }

        [Fact]
        public void NestedFunctionCalls_WorkCorrectly()
        {
            var code = @"
                func inner(a) { return a * a; }
                func outer(b) { return inner(b) + inner(b + 1); }
                func main() { return outer(3); }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(9 + 16, result.AsNumber()); // 3*3 + 4*4
        }

        [Fact]
        public void FunctionWithLoop_IteratesCorrectly()
        {
            var code = @"
                func sumToN(n) {
                    result = 0;
                    i = 1;
                    while (i <= n) {
                        result = result + i;
                        i = i + 1;
                    }
                    return result;
                }
                func main() { return sumToN(5); }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(15, result.AsNumber()); // Sum of 1 to 5
        }

        [Fact]
        public void LocalFunction_CanBeCalled()
        {
            var code = @"
        func main() {
            greet = func() { return ""Hello""; };
            return greet();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("Hello", result.AsString());
        }

        [Fact]
        public void LocalFunction_CanTakeArguments()
        {
            var code = @"
        func main() {
            add = func(a, b) { return a + b; };
            return add(2, 3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(5, result.AsNumber());
        }

        [Fact]
        public void LocalFunction_CanUseClosure()
        {
            var code = @"
        func main() {
            x = 10;
            multiply = func(y) { return x * y; };
            return multiply(3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(30, result.AsNumber());
        }

        [Fact]
        public void LocalFunction_CanBeRecursive()
        {
            var code = @"
        func main() {
            fact = func(n) {
                if (n <= 1) { return 1; }
                return n * fact(n - 1);
            };
            return fact(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(120, result.AsNumber());
        }

        [Fact]
        public void LocalFunction_CanBeNested()
        {
            var code = @"
        func main() {
            outer = func() {
                inner = func() { return 42; };
                return inner();
            };
            return outer();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(42, result.AsNumber());
        }

        [Fact]
        public void LocalFunction_CanBePassedAsArgument()
        {
            var code = @"
        func main() {
            apply = func(fn, value) { return fn(value); };
            square = func(x) { return x * x; };
            return apply(square, 4);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(16, result.AsNumber());
        }

        [Fact]
        public void LocalFunction_ReferencesAreEqualWhenPointingToSameFunction()
        {
            var code = @"
        func main() {
            greet1 = func() { return ""Hello""; };
            greet2 = greet1; # Assigning function reference
            return greet1 == greet2; # Comparing references
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.True(result.AsBoolean()); // Should be true since both reference the same function
        }

        [Fact]
        public void LocalFunction_ReferencesAreNotEqualWhenPointingToDifferentFunctions()
        {
            var code = @"
        func main() {
            greet1 = func() { return ""Hello""; };
            greet2 = func() { return ""Hello""; }; # Different function
            return greet1 == greet2; # Comparing references
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.False(result.AsBoolean()); // Should be false since they are different function references
        }

        [Fact]
        public void LambdaFunction_DirectCallReturnsExpectedValue()
        {
            var code = @"
        func main() {
            return (func() { return 42; })(); # Directly calling a lambda
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(42, result.AsNumber()); // Should return 42
        }

        [Fact]
        public void FunctionExpressions_CanBeStoredInLists_AndCalled()
        {
            var code = @"
        func main() {
            funcs = [
                func(x) { return x + 1; },
                func(x) { return x * 2; }
            ];
            
            result1 = funcs[0](3); # Should return 3 + 1 = 4
            result2 = funcs[1](3); # Should return 3 * 2 = 6
            
            return result1 + result2; # 4 + 6 = 10
        }
    ";

            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();

            Assert.Equal(10, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanBeCalled()
        {
            var code = @"
        func main() {
            func greet() { return ""Hello""; }
            return greet();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("Hello", result.AsString());
        }

        [Fact]
        public void LocalNamedFunction_CanTakeArguments()
        {
            var code = @"
        func main() {
            func add(a, b) { return a + b; }
            return add(2, 3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(5, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanUseClosure()
        {
            var code = @"
        func main() {
            x = 10;
            func multiply(y) { return x * y; }
            return multiply(3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(30, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanBeRecursive()
        {
            var code = @"
        func main() {
            func fact(n) {
                if (n <= 1) { return 1; }
                return n * fact(n - 1);
            }
            return fact(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(120, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanBeNested()
        {
            var code = @"
        func main() {
            func outer() {
                func inner() { return 42; }
                return inner();
            }
            return outer();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(42, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanShadowOuterFunction()
        {
            var code = @"
        func main() {
            func getValue() { return 10; }
            x = getValue();
            {
                func getValue() { return 20; }
                x = x + getValue();
            }
            x = x + getValue();
            return x;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(40, result.AsNumber()); // 10 + 20 + 10 = 40
        }

        [Fact]
        public void LocalNamedFunction_CanAccessVariablesFromEnclosingScope()
        {
            var code = @"
        func main() {
            a = 5;
            b = 10;
            func compute() {
                c = 15;
                return a + b + c;
            }
            return compute();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(30, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanBeDefinedAfterVariableDeclaration()
        {
            var code = @"
        func main() {
            x = 5;
            func double(n) { return n * 2; }
            return double(x);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(10, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_CanBeCalledFromAnotherLocalFunction()
        {
            var code = @"
        func main() {
            func add(a, b) { return a + b; }
            func addAndDouble(a, b) { return add(a, b) * 2; }
            return addAndDouble(3, 4);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(14, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_InNestedBlocks_ProperScoping()
        {
            var code = @"
        func main() {
            result = 0;
            {
                func addOne() { return 1; }
                result = result + addOne();
            }
            {
                func addOne() { return 10; }
                result = result + addOne();
            }
            return result;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(11, result.AsNumber()); // 0 + 1 + 10 = 11
        }

        [Fact]
        public void LocalNamedFunction_CanReturnAnotherFunction()
        {
            var code = @"
        func main() {
            func makeAdder(x) {
                func adder(y) { return x + y; }
                return adder;
            }
            add5 = makeAdder(5);
            return add5(10);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(15, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_MultipleClosuresCaptureDifferentValues()
        {
            var code = @"
        func main() {
            func makeMultiplier(factor) {
                func multiply(n) { return n * factor; }
                return multiply;
            }
            double = makeMultiplier(2);
            triple = makeMultiplier(3);
            return double(5) + triple(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(25, result.AsNumber()); // (5*2) + (5*3) = 10 + 15 = 25
        }

        [Fact]
        public void LocalNamedFunction_CanBeAssignedToVariable()
        {
            var code = @"
        func main() {
            func greet() { return ""Hello""; }
            fn = greet;
            return fn();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("Hello", result.AsString());
        }

        [Fact]
        public void LocalNamedFunction_ShadowsGlobalFunction()
        {
            var code = @"
        func getValue() { return 100; }
        func main() {
            func getValue() { return 200; }
            return getValue();
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(200, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_OuterFunctionStillAccessibleAfterShadowing()
        {
            var code = @"
        func getValue() { return 100; }
        func main() {
            outer = getValue();
            {
                func getValue() { return 200; }
                inner = getValue();
            }
            after = getValue();
            return outer + after;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(200, result.AsNumber()); // 100 + 100 = 200
        }

        [Fact]
        public void LocalNamedFunction_CanBePassedAsArgument()
        {
            var code = @"
        func main() {
            func apply(fn, value) { return fn(value); }
            func square(x) { return x * x; }
            return apply(square, 4);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(16, result.AsNumber());
        }

        [Fact]
        public void LocalNamedFunction_MixedWithLambdas()
        {
            var code = @"
        func main() {
            func named(x) { return x + 1; }
            lambda = func(x) { return x * 2; };
            return named(5) + lambda(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(16, result.AsNumber()); // 6 + 10 = 16
        }
    }
}