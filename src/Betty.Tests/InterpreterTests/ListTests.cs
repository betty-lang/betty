using Betty.Core.Interpreter;

namespace Betty.Tests.InterpreterTests
{
    public class ListTests : InterpreterTestBase
    {
        [Fact]
        public void ForEachLoop_RangeSyntax_ReturnsCorrectValue()
        {
            var code = "x = 0; foreach (i in [1..5]) { x += i; } return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(10, result.AsNumber());
        }

        [Fact]
        public void ForEachLoop_List_ReturnsCorrectValue()
        {
            var code = "x = 0; foreach (i in [1, 2, 3, 4]) { x += i; } return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(10, result.AsNumber());
        }

        [Fact]
        public void ForEachLoop_RangeFunction_ReturnsCorrectValue()
        {
            var code = "x = 0; foreach (i in range(1, 5)) { x += i; } return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(10, result.AsNumber());
        }

        [Fact]
        public void AppendFunction_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; append(x, 4); return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromNumber(2),
                Value.FromNumber(3),
                Value.FromNumber(4)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListRangeFunction_ReturnsCorrectValue()
        {
            var code = "return range(1, 5);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromNumber(2),
                Value.FromNumber(3),
                Value.FromNumber(4)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListElement_PostfixIncrement_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x[1]++; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(3, result.AsNumber());
        }

        [Fact]
        public void List_CompoundAssignment_NestedList_PlusAnotherList_ReturnsCorrectValue()
        {
            var code = "x = [[1, 2], [3, 4]]; x += [[5, 6]]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromList([
                    Value.FromNumber(1),
                    Value.FromNumber(2),
                ]),
                Value.FromList([Value.FromNumber(3), Value.FromNumber(4)]),
                Value.FromList([Value.FromNumber(5), Value.FromNumber(6)])
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListLength_CompoundAssignment_NestedList_ReturnsCorrectValue()
        {
            var code = "x = [[1, 2], [3, 4]]; x[0] += [5, 6]; return len(x);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void ListLength_CompoundAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x += 4; return len(x);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void List_CompoundAssignment_NestedList_ReturnsCorrectValue()
        {
            var code = "x = [[1, 2], [3, 4]]; x[0] += [5, 6]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromList([
                    Value.FromNumber(1),
                    Value.FromNumber(2),
                    Value.FromNumber(5),
                    Value.FromNumber(6)
                ]),
                Value.FromList([Value.FromNumber(3), Value.FromNumber(4)]),
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void NestedListAccess_ReturnsCorrectInnerMember()
        {
            var code = """
            func main()
            {
                outerList = [1, [2, [3, 4]], 5];
                innerList = outerList[1]; # This should be [2, [3, 4]]
                innerInnerList = innerList[1]; # This should be [3, 4]

                # This should return 4, since we're incrementing the first element of the innerInnerList
                return ++innerInnerList[0]; 
            }
            """;

            var interpreter = SetupInterpreter(code, true);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void ListConcatenation_CompoundAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x += [ 4, 5, 6 ]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromNumber(2),
                Value.FromNumber(3),
                Value.FromNumber(4),
                Value.FromNumber(5),
                Value.FromNumber(6)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListConcatenation_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ] + [ 4, 5, 6 ]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromNumber(2),
                Value.FromNumber(3),
                Value.FromNumber(4),
                Value.FromNumber(5),
                Value.FromNumber(6)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void List_CompoundAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x += 4; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromNumber(2),
                Value.FromNumber(3),
                Value.FromNumber(4)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void AddElementToList_WithPlusOperator_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x = x + 4; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromNumber(2),
                Value.FromNumber(3),
                Value.FromNumber(4),
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListDeclarationWithDifferentTypes_ReturnsCorrectValue()
        {
            var code = "x = [ 1, \"hello\", 3 ]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            var expected = new List<Value>
            {
                Value.FromNumber(1),
                Value.FromString("hello"),
                Value.FromNumber(3)
            };

            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListElementAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x[1] = 5; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5, result.AsNumber());
        }

        [Fact]
        public void ListAccess()
        {
            var code = "return [ 1, 2, 3 ][1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2, result.AsNumber());
        }

        [Fact]
        public void ListVariableAccess()
        {
            var code = "x = [ 1, 2, 3 ]; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2, result.AsNumber());
        }

        [Fact]
        public void MultipleListAccess()
        {
            var code = "x = [ 1, 2, 3 ]; return x[1] + x[2];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5, result.AsNumber());
        }

        [Fact]
        public void MultipleTypeListAccess()
        {
            var code = "x = [ 1, \"hello\", 3 ]; return x[1] + x[2];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal("hello3", result.AsString());
        }

        [Fact]
        public void StringAccess()
        {
            var code = "return \"hello\"[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void StringVariableAccess()
        {
            var code = "x = \"hello\"; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void StringVariableAccess_WithExpression()
        {
            var code = "x = \"hello\"; return x[1 + 1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('l', result.AsChar());
        }

        [Fact]
        public void StringVariableAccess_WithVariableExpression()
        {
            var code = "x = \"hello\"; y = 1; return x[y];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void FunctionCallWithListAccess()
        {
            var code = """
                func foo() {
                    return "hello";
                }
                func main() {
                    return foo()[1];
                }
                """;
            var interpreter = SetupInterpreter(code, true);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void ListReferenceType_MutationAndSharing_WorksCorrectly()
        {
            var code = @"
                        # Create an initial list
                        x = [1, 2, 3];
    
                        # Create a reference to the same list
                        y = x;
    
                        # Modify the list through one reference
                        append(x, 4);
    
                        # Check that both references reflect the change
                        z1 = x[3];
                        z2 = y[3];
    
                        # Verify that z1 and z2 are the same value (4)
                        # and that both x and y have the same length
                        return [len(x), len(y), z1, z2];
                    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();

            // Convert the result to a list to check its contents
            var resultList = result.AsList();

            // Assertions to verify reference type behavior
            Assert.Equal(4, resultList.Count);
            Assert.Equal(4, resultList[0].AsNumber()); // len(x)
            Assert.Equal(4, resultList[1].AsNumber()); // len(y)
            Assert.Equal(4, resultList[2].AsNumber()); // z1
            Assert.Equal(4, resultList[3].AsNumber()); // z2
        }

        [Fact]
        public void ListReferenceType_SeparateListsMaintainIndependence()
        {
            var code = @"
                        # Create two separate lists
                        x = [1, 2, 3];
                        y = [1, 2, 3];
    
                        # Modify one list
                        append(x, 4);
    
                        # Check that the other list remains unchanged
                        return [len(x), len(y)];
                    ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();

            // Convert the result to a list to check its contents
            var resultList = result.AsList();

            // Assertions to verify list independence
            Assert.Equal(4, resultList[0].AsNumber()); // len(x)
            Assert.Equal(3, resultList[1].AsNumber()); // len(y)
        }

        [Fact]
        public void ListReferenceType_FunctionParameterMutation()
        {
            var code = @"
                        # Define a function that modifies a list
                        func modify_list(list) {
                            append(list, 4);
                            return list;
                        }
                        
                        func main() {
                            # Create an initial list
                            x = [1, 2, 3];
    
                            # Pass the list to a function and modify it
                            y = modify_list(x);
    
                            # Check that the original list and the returned list are the same
                            return [len(x), len(y), x[3], y[3]];
                        }
                    ";
            var interpreter = SetupInterpreter(code, true);
            var result = interpreter.Interpret();

            // Convert the result to a list to check its contents
            var resultList = result.AsList();

            // Assertions to verify list mutation through function
            Assert.Equal(4, resultList.Count);
            Assert.Equal(4, resultList[0].AsNumber()); // len(x)
            Assert.Equal(4, resultList[1].AsNumber()); // len(y)
            Assert.Equal(4, resultList[2].AsNumber()); // x[3]
            Assert.Equal(4, resultList[3].AsNumber()); // y[3]
        }

        [Fact]
        public void DeepCopy_NestedListMutation_DoesNotAffectOriginal()
        {
            var code = @"
                # Create a nested list
                original = [1, [2, 3], 4];
    
                # Create a deep copy
                copy = clone(original);
    
                # Modify the nested list in the copy
                copy[1][0] = 5;
    
                # Return original and modified copy to verify independence
                return [original[1][0], copy[1][0]];
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();

            // Convert the result to a list to check its contents
            var resultList = result.AsList();

            // Assertions to verify deep copy behavior
            Assert.Equal(2, resultList[0].AsNumber()); // Original list unchanged
            Assert.Equal(5, resultList[1].AsNumber()); // Copy list modified
        }

        [Fact]
        public void DeepCopy_ComplexNestedStructure_FullyIndependent()
        {
            var code = @"
                    # Create a complex nested list
                    original = [1, [2, [3, 4]], 5];
    
                    # Create a deep copy
                    copy = clone(original);
    
                    # Modify deeply nested elements
                    copy[1][1][0] = 6;
    
                    # Return original and modified copy to verify independence
                    return [original[1][1][0], copy[1][1][0]];
                ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();

            // Convert the result to a list to check its contents
            var resultList = result.AsList();

            // Assertions to verify deep copy behavior
            Assert.Equal(3, resultList[0].AsNumber()); // Original list unchanged
            Assert.Equal(6, resultList[1].AsNumber()); // Copy list modified
        }
    }
}