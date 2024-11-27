using System.Collections.Generic;
public class Driver()
{
    private static List<string> stack = new List<string>();
    private static Dictionary<string, int> precedenceDict = new Dictionary<string, int> { { "^", 0 }, { "*", 1 }, { "/", 1 } };
    private static Dictionary<int, string> operandOrderDict = new Dictionary<int, string> { { 0, "right-to-left" }, { 1, "left-to-right" } };

    public static void Main(string[] args)
    {
        string userInput = Console.ReadLine();
        string token = "";
        stack.Add("(");
        foreach (char character in userInput)
        {
            if (character >= '0' && character <= '9')
            {
                token += character;
            }
            else if (isOperand(character.ToString()) || character == '(' || character == ')')
            {
                if (token != "")
                {
                    stack.Add(token);
                }

                stack.Add(character.ToString());
                token = "";
            }

        }
        if (token != "")
        {
            stack.Add(token);
        }

        stack.Add(")");

        addImplicitMultiplication();
        addParenthesis();
        Console.Write("Processed Expression: ");
        foreach (string c in stack)
        {
            Console.Write(c);
        }
        Console.WriteLine("");
        float value;
        (value, _) = calculateEquation(0);
        Console.Write("Final Answer: ");
        Console.WriteLine(value);
    }

    // Function to calculate the result given 2 numbers and an operation
    public static float calculateOperation(float firstNumber, string operand, float secondNumber)
    {
        float result = 0;
        switch (operand)
        {
            case "+":
                result = firstNumber + secondNumber;
                break;
            case "-":
                result = firstNumber - secondNumber;
                break;
            case "*":
                result = firstNumber * secondNumber;
                break;
            case "/":
                result = firstNumber / secondNumber;
                break;
            case "^":
                result = (float)Math.Pow(firstNumber, secondNumber);
                break;
        }
        return result;
    }

    // Function to add implicit multiplication
    // ie. '2 ( 2 + 1 )' -> '2 * ( 2 + 1 )'
    public static void addImplicitMultiplication()
    {
        for (int index = 0; index < stack.Count - 1; index++)
        {
            if (double.TryParse(stack[index], out _) && stack[index + 1] == "(")
            {
                stack.Insert(index + 1, "*");
            }
            else if (double.TryParse(stack[index + 1], out _) && stack[index] == ")")
            {
                stack.Insert(index + 1, "*");
            }
            else if (stack[index] == ")" && stack[index + 1] == "(")
            {
                stack.Insert(index + 1, "*");
            }
        }
    }

    public static bool isOperand(string operand)
    {
        if (operand == "+" || operand == "-" || operand == "*" || operand == "/" || operand == "^")
        {
            return true;
        }
        return false;
    }

    // Function to add parenthesis around a given expression
    public static void addParenthesis()
    {
        // Loop through the precedence list
        // Note: This is needed because '^' has higher order than '*' or '/'
        for (int precedenceIndex = 0; precedenceIndex
        < precedenceDict.Count - 1; precedenceIndex++)
        {
            // Loop through each character in the expression either left-to-right or right-to-left
            for (int outerIndex = operandOrderDict[precedenceIndex] == "left-to-right" ? 0 : stack.Count - 1; operandOrderDict[precedenceIndex] == "left-to-right" ? outerIndex < stack.Count : outerIndex >= 0; outerIndex += operandOrderDict[precedenceIndex] == "left-to-right" ? 1 : -1)
            {
                // If the character is in the precedence dictionary and is currenly getting parsed
                // Note: We add parenthesis 
                if (precedenceDict.TryGetValue(stack[outerIndex], out int value) && value == precedenceIndex)
                {
                    // If we want to add parenthesis around just 2 numbers
                    // ie. 2 * 8 -> ( 2 * 8 )
                    if (!isOperand(stack[outerIndex - 1]) && stack[outerIndex - 1] != ")" && !isOperand(stack[outerIndex + 1]) && stack[outerIndex + 1] != "(")
                    {
                        stack.Insert(outerIndex - 1, "(");
                        stack.Insert(outerIndex + 3, ")");
                        outerIndex += 1;
                    }
                    // If we want to add parenthesis around one number and an expression (right adjusted)
                    // ie. ( 2 * 8 ) * 2 -> ( ( 2 * 8 ) * 2)
                    // ie. ( ( 2 * 8 ) ) * 2 -> ( ( ( 2 * 8 ) ) * 2 )
                    if (stack[outerIndex - 1] == ")" && !isOperand(stack[outerIndex + 1]) && stack[outerIndex + 1] != "(")
                    {
                        int innerIndex = outerIndex - 1;
                        int parenthesisOffset = 0;

                        while (stack[innerIndex] != "(")
                        {
                            if (stack[innerIndex] == ")")
                            {
                                parenthesisOffset += 1;
                            }

                            innerIndex -= 1;
                        }

                        innerIndex = outerIndex - 1;
                        int parenthesisFound = 0;
                        while (parenthesisFound != parenthesisOffset)
                        {
                            if (stack[innerIndex] == "(")
                            {
                                parenthesisFound += 1;
                            }
                            innerIndex -= 1;
                        }

                        stack.Insert(outerIndex + 2, ")");
                        stack.Insert(innerIndex + 1, "(");
                        outerIndex += 1;
                    }
                    // If we want to add parenthesis around one number and an expression (left adjusted)
                    // ie. 2 * ( 2 * 8 ) -> ( 2 * ( 2 * 8 ) )
                    // ie. 2 * ( ( 2 * 8 ) ) -> ( 2 * ( ( 2 * 8 ) ) )
                    if (stack[outerIndex + 1] == "(" && !isOperand(stack[outerIndex - 1]) && stack[outerIndex - 1] != ")")
                    {
                        int innerIndex = outerIndex + 1;
                        int parenthesisOffset = 0;

                        while (stack[innerIndex] != ")")
                        {
                            if (stack[innerIndex] == "(")
                            {
                                parenthesisOffset += 1;
                            }

                            innerIndex += 1;
                        }

                        int parenthesisFound = 0;
                        innerIndex = outerIndex + 1;
                        while (parenthesisFound != parenthesisOffset)
                        {
                            if (stack[innerIndex] == ")")
                            {
                                parenthesisFound += 1;
                            }
                            innerIndex += 1;
                        }

                        stack.Insert(innerIndex, ")");
                        stack.Insert(outerIndex - 1, "(");
                        outerIndex += 1;
                    }
                    // If we want to add parenthesis around two expressions
                    // ie. ( 1 + 2 ) * ( 1 + 2 ) ->  ( ( 1 + 2 ) * ( 1 + 2 ) )
                    if (stack[outerIndex - 1] == ")" && stack[outerIndex + 1] == "(")
                    {
                        int rightInnerIndex = outerIndex - 1;
                        int rightParenthesisOffset = 0;

                        while (stack[rightInnerIndex] != "(")
                        {
                            if (stack[rightInnerIndex] == ")")
                            {
                                rightParenthesisOffset += 1;
                            }

                            rightInnerIndex -= 1;
                        }

                        rightInnerIndex = outerIndex - 1;
                        int rightParenthesisFound = 0;
                        while (rightParenthesisFound != rightParenthesisOffset)
                        {
                            if (stack[rightInnerIndex] == "(")
                            {
                                rightParenthesisFound += 1;
                            }
                            rightInnerIndex -= 1;
                        }

                        stack.Insert(rightInnerIndex + 1, "(");


                        int leftInnerIndex = outerIndex + 1;
                        int leftParenthesisOffset = 0;

                        while (stack[leftInnerIndex] != ")")
                        {
                            if (stack[leftInnerIndex] == "(")
                            {
                                leftParenthesisOffset += 1;
                            }

                            leftInnerIndex += 1;
                        }

                        int leftParenthesisFound = 0;
                        leftInnerIndex = outerIndex + 1;
                        while (leftParenthesisFound != leftParenthesisOffset)
                        {
                            if (stack[leftInnerIndex] == ")")
                            {
                                leftParenthesisFound += 1;
                            }
                            leftInnerIndex += 1;
                        }
                        stack.Insert(leftInnerIndex, ")");
                        outerIndex += 1;
                    }
                }
            }
        }
    }

    // Recursive function to calculate the given expresssion
    public static (float, int) calculateEquation(int startIndex)
    {
        float firstNumber = 0;
        float secondNumber = 0;
        int jumpOffset = 0;
        string operand;
        // Loop through the expression stack, starting at the given start index
        for (int index = startIndex; index < stack.Count; index++)
        {
            // If the character is an open parenthesis
            // Note: We want to calculate expressions within parethesis first
            if (stack[index] == "(")
            {
                // If the second character is NOT an open parathesis
                // Note: This signifies that we have not calculated the first number yet
                // ie. If we have '( 2 + 1 )', we want to fill the first number with '2', 
                // the operand with '+', and the second number with '1'
                if (stack[index + 1] != "(")
                {
                    // Fill the first number since it's guarenteed to be a number
                    // Note: We can safely make this assumption since the we already checked
                    // that it wasn't another nested expression, and that there will never be 
                    // two operands next to each other
                    firstNumber = Int32.Parse(stack[index + 1]);

                    // If only one input is given inside a parenthesis
                    // ie. ( ( 25 ) )
                    if (stack[index + 2] == ")")
                    {
                        return (firstNumber, index + 2);
                    }
                    // The second character is guarenteed to be an operand
                    operand = stack[index + 2];
                    // If the third character is NOT an open parethesis
                    // Note: This is to check that we can safely fill the second number with
                    // the third character because it could be another nested expression
                    // ie. We can fill '1 + 1' but NOT '1 + ( 2 + 2 )'
                    if (stack[index + 3] != "(")
                    {
                        // Fill the second number at the third index
                        secondNumber = Int32.Parse(stack[index + 3]);
                        Console.Write("Step: ");
                        Console.Write(firstNumber);
                        Console.Write(operand);
                        Console.Write(secondNumber);
                        Console.Write("=");
                        // Return the result of the first number, operand, and second number
                        firstNumber = calculateOperation(firstNumber, operand, secondNumber);
                        Console.WriteLine(firstNumber);
                        // If the fourth character is NOT an end parenthesis
                        // Note: This is needed because if we DO reach the end of 
                        // expression, then we want to return our result back to the calling
                        // function. But if there are more expressions to calculate inside our parenthesis,
                        // then we want to move our index up by two, to the second number. This is because
                        // the function knows that our first number already holds the result, so it
                        // will only fill the second number next time.
                        // ie. '( 1 + 1 + 5 )' will store '2' in the first number. So next time we only need
                        // to add the '5' back to the fist number since we are working from left to right. 
                        if (stack[index + 4] != ")")
                        {
                            index += 2;
                        }
                        // Else if we do reach the end of nested expression, simply return the result
                        // ie. '( 1 + 1 )' will return '2' back to the parent.
                        else
                        {
                            return (firstNumber, index + 4);
                        }

                    }
                    // Else if the second number is a nested expression, we want to recurse
                    // the expression and return the final result back into our second number,
                    // so that we can eventually calculate it with the first number
                    // ie. In '( 1 + ( 2 * 2 ) )', the second number is another expression, so we 
                    // start the whole process again, but only with '( 2 * 2 )', which will break down
                    // to '4', and then be returned back to be '( 1 + 4 )'.
                    else
                    {
                        (secondNumber, jumpOffset) = calculateEquation(index + 3);
                        Console.Write("Step: ");
                        Console.Write(firstNumber);
                        Console.Write(operand);
                        Console.Write(secondNumber);
                        Console.Write("=");
                        firstNumber = calculateOperation(firstNumber, operand, secondNumber);
                        Console.WriteLine(firstNumber);
                        index = jumpOffset - 1;
                    }


                }
                // If the second character is another expression, then we simply want
                // to recurse, starting at the new expression, and return the result back into the
                // first number
                // ie. In '( ( 1 * 2 ) + 3 )', we recurse staring at '( 1 * 2 )', and return the result
                // back to become '( 2 + 3 )'
                else
                {
                    (firstNumber, jumpOffset) = calculateEquation(index + 1);
                    index = jumpOffset - 1;
                }

            }
            // When looping through our expression, if we ever reach the end of 
            // a set of nested parenthesis, then we know it's time to return back to the calling
            // function. Except we have to check if we are in nested parethesis or not because
            // we only want to return back if so.
            // Note: We set the offset by 1 just incase we have to return back from many sets
            // of nested parenthesis. 
            // ie. We DON'T want to return back with '( 1 + 1 ) + 2 ',
            // but we DO want to return back with '( ( 1 + 1) ) + 2'
            else if (stack[index] == ")" && stack[index + 1] == ")")
            {
                return (firstNumber, jumpOffset + 1);
            }
            // Else if we don't start with an open or end parenthesis, we can safley assume
            // that are first number has been filled above and that we just need to work with
            // the second number
            else
            {
                // We can safely assume that the operand will always be the second character from
                // the current index since we always move our index in accordance with the jump offset
                operand = stack[index + 1];
                // If the second character is NOT an open parenthesis, then we can safely assume
                // that the second character is a number
                if (stack[index + 2] != "(")
                {
                    secondNumber = Int32.Parse(stack[index + 2]);
                    Console.Write("Step: ");
                    Console.Write(firstNumber);
                    Console.Write(operand);
                    Console.Write(secondNumber);
                    Console.Write("=");
                    firstNumber = calculateOperation(firstNumber, operand, secondNumber);
                    Console.WriteLine(firstNumber);
                    if (stack[index + 3] != ")")
                    {
                        index += 1;
                    }
                    else
                    {
                        return (firstNumber, index + 3);
                    }

                }
                // Else if the second character IS an open parenthesis, then that means
                // we have another nested equation, in which we want to recurse and return the
                // result back into the second number
                else
                {
                    (secondNumber, jumpOffset) = calculateEquation(index + 2);
                    Console.Write("Step: ");
                    Console.Write(firstNumber);
                    Console.Write(operand);
                    Console.Write(secondNumber);
                    Console.Write("=");
                    firstNumber = calculateOperation(firstNumber, operand, secondNumber);
                    Console.WriteLine(firstNumber);
                    index = jumpOffset - 1;
                }
            }
        }
        return (firstNumber, jumpOffset);

    }
}
