# Math Parser

<b>Requirements:</b> Windows or Linux Environment
<br>
<br>
<b>Quick Install:</b>
<br>
1. Download the latest <b><a href="https://github.com/JJDOESIT/math-parser/releases/tag/v1.0.0">math-parser.zip</a></b> release for Windows
3. Extract the files
4. Run MathParser.exe
    * If permissions are denied, run ```chmod 700 <path to MathParser.exe>``` and try running again



<b>Build Yourself:</b>
<br>
1. Clone the repo or download the source code in a suitable .NET environment
2. Run Program.cs

# Features

- This project features a math expression parser that supports ```+, -, *, /, (,), and ^```
- It uses a recursive algorithim pipleine created by JJDOESIT (me)

# How It Works

- There are 4 steps to the pipeline
   1. Convert user input into a stack
      - ie. 10 (2 + 3) + 1 is parsed into [10, (, 2, +, 3, ), +, 1]
   2. Add implicit multiplication
      - ie. 10 (2 + 3) + 1 becomes 10 * (2 + 3) + 1
   3. Add parenthesis to force precedence
      - ie. 10 * (2 + 3) + 1 becomes (10 * (2 + 3)) + 1
   4. Calculate the expression
 
- The recursive algorithim to calculate the expression works inside out
   


