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
    }
}