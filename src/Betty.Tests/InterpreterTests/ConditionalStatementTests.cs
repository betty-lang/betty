namespace Betty.Tests.InterpreterTests
{
    public class ConditionalStatementTests : InterpreterTestBase
    {
        [Fact]
        public void SwitchExpression_WithLambdaResults()
        {
            var code = @"
        func main() {
            operation = ""square"";
            fn = operation switch {
                ""square"" => func(x) { return x * x; },
                ""double"" => func(x) { return x * 2; },
                ""increment"" => func(x) { return x + 1; },
                _ => func(x) { return x; }
            };
            return fn(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(25, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_ReturningDifferentLambdas()
        {
            var code = @"
        func main() {
            mode = 2;
            calculator = mode switch {
                1 => func(a, b) { return a + b; },
                2 => func(a, b) { return a * b; },
                3 => func(a, b) { return a - b; },
                _ => func(a, b) { return 0; }
            };
            return calculator(7, 3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(21, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_OnLambdaResult()
        {
            var code = @"
        func main() {
            getValue = func() { return 3; };
            result = getValue() switch {
                1 => ""One"",
                2 => ""Two"",
                3 => ""Three"",
                _ => ""Other""
            };
            return result;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("Three", result.AsString());
        }

        [Fact]
        public void SwitchExpression_WithClosureCapture()
        {
            var code = @"
        func main() {
            multiplier = 10;
            type = ""add"";
            operation = type switch {
                ""add"" => func(x) { return x + multiplier; },
                ""multiply"" => func(x) { return x * multiplier; },
                _ => func(x) { return x; }
            };
            return operation(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(15, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_NestedWithLambdas()
        {
            var code = @"
        func main() {
            x = 2;
            y = 1;
            result = x switch {
                1 => (func() { return ""One""; })(),
                2 => y switch {
                    1 => (func() { return ""Two-One""; })(),
                    2 => (func() { return ""Two-Two""; })(),
                    _ => ""Two-Other""
                },
                _ => ""Other""
            };
            return result;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("Two-One", result.AsString());
        }

        [Fact]
        public void SwitchExpression_ReturningListOfLambdas()
        {
            var code = @"
        func main() {
            type = ""math"";
            operations = type switch {
                ""math"" => [
                    func(x) { return x + 1; },
                    func(x) { return x * 2; }
                ],
                ""string"" => [
                    func(x) { return x; }
                ],
                _ => []
            };
            return operations[0](5) + operations[1](3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(12, result.AsNumber()); // 6 + 6
        }

        [Fact]
        public void SwitchExpression_WithHigherOrderFunction()
        {
            var code = @"
        func main() {
            apply = func(fn, value) { return fn(value); };
            operation = ""triple"";
            result = apply(
                operation switch {
                    ""double"" => func(x) { return x * 2; },
                    ""triple"" => func(x) { return x * 3; },
                    _ => func(x) { return x; }
                },
                4
            );
            return result;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(12, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_ChainedLambdaCalls()
        {
            var code = @"
        func main() {
            stage = 1;
            process = stage switch {
                1 => func(x) { return func(y) { return x + y; }; },
                2 => func(x) { return func(y) { return x * y; }; },
                _ => func(x) { return func(y) { return 0; }; }
            };
            return process(5)(3);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(8, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_WithRecursiveLambda()
        {
            var code = @"
        func main() {
            type = ""factorial"";
            compute = type switch {
                ""factorial"" => func(n) {
                    if (n <= 1) { return 1; }
                    return n * compute(n - 1);
                },
                ""sum"" => func(n) {
                    if (n <= 0) { return 0; }
                    return n + compute(n - 1);
                },
                _ => func(n) { return 0; }
            };
            return compute(5);
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(120, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_MatchingOnLambdaEquality()
        {
            var code = @"
        func main() {
            fn1 = func() { return 1; };
            fn2 = func() { return 2; };
            target = fn1;
            
            result = target switch {
                fn1 => ""First function"",
                fn2 => ""Second function"",
                _ => ""Unknown function""
            };
            return result;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("First function", result.AsString());
        }

        [Fact]
        public void SwitchExpression_AfterVariable()
        {
            var code = @"
        x = 2;
        result = x switch {
            1 => ""One"",
            2 => ""Two"",
            3 => ""Three"",
            _ => ""Other""
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Two", result.AsString());
        }

        [Fact]
        public void SwitchExpression_WithNumericResults()
        {
            var code = @"
        grade = 'B';
        points = grade switch {
            'A' => 4,
            'B' => 3,
            'C' => 2,
            'D' => 1,
            _ => 0
        };
        return points;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(3, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_WithDefault_NoMatch()
        {
            var code = @"
        value = 99;
        result = value switch {
            1 => ""One"",
            2 => ""Two"",
            _ => ""Default""
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Default", result.AsString());
        }

        [Fact]
        public void SwitchExpression_SingleCase()
        {
            var code = @"
        x = 42;
        result = x switch {
            42 => ""Answer"",
            _ => ""Not Answer""
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Answer", result.AsString());
        }

        [Fact]
        public void SwitchExpression_OnlyDefault()
        {
            var code = @"
        x = 123;
        result = x switch {
            _ => ""Always This""
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Always This", result.AsString());
        }

        [Fact]
        public void SwitchExpression_Nested()
        {
            var code = @"
        x = 1;
        y = 2;
        result = x switch {
            1 => y switch {
                1 => ""One-One"",
                2 => ""One-Two"",
                _ => ""One-Other""
            },
            2 => ""Two"",
            _ => ""Other""
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("One-Two", result.AsString());
        }

        [Fact]
        public void SwitchExpression_InFunctionCall()
        {
            var code = @"
            func double(n) { return n * 2; }
            func main() {
                x = 5;
                result = double(x switch {
                    5 => 10,
                    10 => 20,
                    _ => 0
                });
                return result;
            }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(20, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_ChainedWithIndexer()
        {
            var code = @"
        arr = [""zero"", ""one"", ""two""];
        x = 1;
        result = arr[x switch {
            0 => 0,
            1 => 1,
            2 => 2,
            _ => 0
        }];
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("one", result.AsString());
        }

        [Fact]
        public void SwitchExpression_InBinaryExpression()
        {
            var code = @"
        x = 2;
        result = (x switch {
            1 => 10,
            2 => 20,
            _ => 0
        }) + 5;
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(25, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_BooleanConditions()
        {
            var code = @"
        isActive = true;
        status = isActive switch {
            true => ""Active"",
            false => ""Inactive""
        };
        return status;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Active", result.AsString());
        }

        [Fact]
        public void SwitchExpression_WithStrings()
        {
            var code = @"
        day = ""Monday"";
        type = day switch {
            ""Monday"" => ""Weekday"",
            ""Tuesday"" => ""Weekday"",
            ""Saturday"" => ""Weekend"",
            ""Sunday"" => ""Weekend"",
            _ => ""Unknown""
        };
        return type;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Weekday", result.AsString());
        }

        [Fact]
        public void SwitchExpression_MultipleDefaults_ShouldFail()
        {
            var code = @"
        x = 1;
        result = x switch {
            1 => ""One"",
            _ => ""Default1"",
            _ => ""Default2""
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            // Parser should report error for multiple defaults, or interpreter should handle it
            // Either way, the code shouldn't execute correctly
            var exception = Record.Exception(() => interpreter.Interpret());
            // We're lenient here - either an exception or errors are acceptable
            Assert.True(exception != null || interpreter.Parser.Errors.Count > 0);
        }

        [Fact]
        public void SwitchExpression_AfterFunctionCall()
        {
            var code = @"
        func getValue() { return 3; }
        func main() {
            result = getValue() switch {
            1 => ""One"",
            2 => ""Two"",
            3 => ""Three"",
            _ => ""Other""
            };
            return result;
        }
    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal("Three", result.AsString());
        }

        [Fact]
        public void SwitchExpression_WithExpressionResults()
        {
            var code = @"
        x = 3;
        y = 5;
        result = x switch {
            1 => y + 1,
            2 => y * 2,
            3 => y - 2,
            _ => 0
        };
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(3, result.AsNumber());
        }

        [Fact]
        public void SwitchExpression_AfterLiteral()
        {
            var code = @"
            result = 3 switch {
                1 => ""One"",
                2 => ""Two"",
                _ => ""Other""
            };
            return result;
            ";

            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal("Other", result.AsString());  // 3 does not match any specific case, so it returns 'Other'
        }

        [Fact]
        public void SwitchStatement_MixedBracedAndUnbraced()
        {
            var code = @"
        x = 2;
        result = 0;
        switch (x) {
            case 1:
                result = 10;
                break;
            case 2: {
                temp = 15;
                result = temp + 5;
                break;
            }
            case 3:
                result = 30;
                break;
        }
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(20.0, result.AsNumber());
        }

        [Fact]
        public void SwitchStatement_FallThroughSharesScope()
        {
            var code = @"
        x = 1;
        result = 0;
        switch (x) {
            case 1:
                x = 2;
                temp = 50;
                # no break - falls through
            case 2:
                result = temp + 10;  # Uses temp from case 1
                break;
        }
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(60.0, result.AsNumber());  // temp (50) + 10 = 60
        }

        [Fact]
        public void SwitchStatement_CompoundWithScopedVariables()
        {
            var code = @"
        x = 1;
        result = 0;
        switch (x) {
            case 1: {
                temp = 100;
                result = temp;
                break;
            }
            case 2: {
                temp = 200;
                result = temp;
                break;
            }
        }
        return result;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            // temp is scoped inside the braces, so we return it via result
            Assert.Equal(100.0, result.AsNumber());
        }

        [Fact]
        public void SwitchStatement_BreakInCases()
        {
            var code = @"
                x = 1;
                switch (x) {
                    case 1:
                        x = 2;
                        break;
                    case 2:
                        x = 3;
                        break;
                    case 3:
                        x = 4;
                        break;
                    default:
                        x = 100;
                }
                return x;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void SwitchStatement_FallThroughCases()
        {
            var code = @"
                x = 1;
                switch (x) {
                    case 1:
                        x = 2;
                    case 2:
                        x = 3;
                    case 3:
                        x = 4;
                    default:
                        x = 100;
                }
                return x;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(100.0, result.AsNumber());
        }

        [Fact]
        public void SwitchStatement_DefaultCaseExecuted()
        {
            var code = @"
                x = 5;
                switch (x) {
                    case 1:
                        return 10;
                    case 2:
                        return 20;
                    case 3:
                        return 30;
                    default:
                        return 40;
                }
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(40.0, result.AsNumber());
        }

        [Fact]
        public void SwitchStatement_ExecutesCorrectCase()
        {
            var code = @"
                x = 2;
                switch (x) {
                    case 1:
                        return 10;
                    case 2:
                        return 20;
                    case 3:
                        return 30;
                    default:
                        return 40;
                }
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(20.0, result.AsNumber());
        }

        [Fact]
        public void NestedWhileLoops_ContinueStatement_OutsideInnerIf()
        {
            var code = @"
                i = 0;
                j = 0;
                while (i < 10) {
                    if (j == 8) {
		                while (j < 10) j++;
		                continue;
	                }
	                i++;
                }
                return i;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(10.0, result.AsNumber());
        }

        [Fact]
        public void NestedWhileLoops_ContinueStatement()
        {
            var code = @"
                counter = 0;
                i = 0;
                while (i < 5) {
                    i = i + 1;
                    j = 0;
                    while (j < 5) {
                        j = j + 1;
                        if (j == 3) {
                            continue;
                        }
                        counter = counter + 1;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(20.0, result.AsNumber());
        }

        [Fact]
        public void DoWhileLoop_ExecutesCorrectNumberOfTimes_WithContinueStatement()
        {
            var code = @"
                counter = 0;
                i = 0;
                do {
                    counter++;
                    if (counter == 3)
                        continue;
                    i++;
                } while (counter < 5);
                return i;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void DoWhileLoop_ExecutesCorrectNumberOfTimes_WithBreakStatement()
        {
            var code = @"
                counter = 0;
                do {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                } while (true);
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void DoWhileLoop_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                do {
                    counter = counter + 1;
                } while (counter < 5);
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void Function_WithForLoop_ReturnsCorrectValue()
        {
            var code = @"
                func myfunc() {
                    counter = 0;
                    for (i = 0; i < 5; i++) {
                        if (counter == 3) {
                            return counter;
                        }
                        counter = counter + 1;
                    }
                }

                func main() {
                    result = myfunc();
                    return result;
                }
            ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_WithReturnStatement()
        {
            var code = @"
                for (i = 0; i < 5; i = i + 1) {
                    if (i == 3) {
                        return i;
                    }
                }
                return 0;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyConditionAndIncrement()
        {
            var code = @"
                counter = 0;
                for (; ; ) {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_SingleStatement_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; i = i + 1) counter = counter + 1;
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_WithContinueStatement()
        {

            var code = @"
                counter = 0;
                for (i = 0; i < 5; i = i + 1) {
                    if (i == 2) {
                        continue;
                    }
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_WithContinueStatement()
        {
            var code = @"
                counter = 0;
                i = 0;
                while (i < 5) {
                    i = i + 1;
                    if (i == 2) {
                        continue;
                    }
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyIncrement()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; ) {
                    counter = counter + 1;
                    i = i + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyCondition()
        {
            var code = @"
                counter = 0;
                for (i = 0; ; i = i + 1) {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }
        
        [Fact]
        public void ForLoop_ShorthandIcrement_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; i++) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; i = i + 1) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyInitializer()
        {
            var code = @"
                counter = 0;
                for (; counter < 5; counter = counter + 1) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch()
        {
            var code = @"
                    if (true) { return 42; }
                    return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(42.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch()
        {
            var code = @"
                    if (true) return 42;
                    return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(42.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElse()
        {
            var code = @"
                    if (false) { return 42; }
                    else { return 0; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElse()
        {
            var code = @"
                    if (false) return 42;
                    else return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (true) { return 0; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf()
        {
            var code = @"
                    if (false) return 42;
                    elif (true) return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf_WithElse()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (false) { return 0; }
                    else { return 1; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf_WithElse()
        {
            var code = @"
                    if (false) return 42;
                    elif (false) return 0;
                    else return 1;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf_WithElse_WithNestedIf()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (false) { return 0; }
                    elif (true) { if (true) { return 1; } }
                    else { return 2; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf_WithElse_WithNestedIf()
        {
            var code = @"
                    if (false) return 42;
                    elif (false) return 0;
                    elif (true) if (true) return 1;
                    else return 2;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_ExecutesMultipleTimes()
        {
            var code = @"
                counter = 0;
                while (counter < 5) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_ReturnsCorrectValue()
        {
            var code = "return true ? 1 : 0;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_FalseCondition_ReturnsCorrectValue()
        {
            var code = "return false ? 1 : 2;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void NestedTernaryOperator_ReturnsCorrectValue()
        {
            var code = "return (true ? false : true) ? 1 : 2;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void ComparisonExpression_ReturnsTrue()
        {
            var code = "return (1 + 1 == 2);";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.True(result.AsBoolean());
        }

        [Fact]
        public void TernaryOperator_WithArithmeticExpressions()
        {
            var code = "return (1 + 1 == 2) ? (3 * 2) : (4 / 2);";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_WithinArithmeticExpressions()
        {
            var code = "return 10 + (false ? 100 : 200) * 2;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(410.0, result.AsNumber());
        }

        [Fact]
        public void ComplexNestedTernaryOperator_ReturnsCorrectValue()
        {
            var code = "return (5 > 3 ? 3 < 2 : 4 > 2) ? (7 == 7 ? 9 : 8) : (6 == 6 ? 10 : 11);";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(10.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_WithFunctionCalls()
        {
            var code = @"
            func add(a, b) { return a + b; }
            func multiply(a, b) { return a * b; }
            func main() {
                return (2 == 2) ? add(4, 5) : multiply(3, 3);
            }
        ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_SingleStatement_ExecutesMultipleTimes()
        {
            var code = @"
                counter = 0;
                while (counter < 5) counter = counter + 1;
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_WithBreakStatement()
        {
            var code = @"
                counter = 0;
                while (true) {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void IfExpression_ExecutesCorrectBranch()
        {
            var code = @"
        x = if 1 == 1 then 2 else 6;
        return x;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void ElifExpression_ExecutesCorrectBranch()
        {
            var code = @"
        x = if 1 == 2 then 2 elif 2 == 2 then 5 else 6;
        return x;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ElseExpression_ExecutesCorrectBranch()
        {
            var code = @"
        x = if 1 == 2 then 2 elif 2 == 3 then 5 else 6;
        return x;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void NestedIfElifElse_ExecutesCorrectBranch()
        {
            var code = @"
        x = if 1 == 2 then 2 elif 2 == 2 then if 3 == 3 then 8 else 7 else 6;
        return x;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(8.0, result.AsNumber());
        }

        [Fact]
        public void MultipleElifConditions_ExecutesCorrectBranch()
        {
            var code = @"
        x = if 1 == 2 then 2 elif 2 == 3 then 5 elif 4 == 4 then 7 else 6;
        return x;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(7.0, result.AsNumber());
        }

        [Fact]
        public void NoConditionTrue_ExecutesElseBranch()
        {
            var code = @"
        x = if 1 == 3 then 2 elif 2 == 5 then 5 else 9;
        return x;
    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsNumber());
        }

    }
}