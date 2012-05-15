using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokenizer
{
    class DoTokens
    {
        static string[] pemdasOrder = {"negation", "^", "*/", "+-" };

        static List<Token> pemdas(List<Token> input, string type)
        {
            List<Token> ret = new List<Token>();
            foreach (Token t in input) { ret.Add(t); }

            int i = -1;
            int iTotal = i;

            foreach (Token token in input)
            {
                i++;
                iTotal++;
                string tokenValue = token.value;
                TokenType tokenType = token.tokenType;
                if (tokenType == TokenType.Negating)
                {
                    Token toRight = ret[iTotal + 1];
                    double rightVal = Convert.ToDouble(toRight.value);
                    string newVal = (-rightVal).ToString();

                    ret.RemoveRange(iTotal, 1);
                    iTotal -= 1;
                    ret[iTotal + 1].value = newVal;
                }
                else if (tokenType == TokenType.Operation && type.Contains(tokenValue))
                {
                    Token toLeft = ret[iTotal - 1];
                    Token toRight = ret[iTotal + 1];
                    double leftVal = Convert.ToDouble(toLeft.value);
                    double rightVal = Convert.ToDouble(toRight.value);
                        string newVal = "";
                        switch (tokenValue)
                        {
                            case "^":
                                newVal = Math.Pow(leftVal, rightVal).ToString();
                                break;

                            case "*":
                                newVal = (leftVal * rightVal).ToString();
                                break;

                            case "/":
                                newVal = (leftVal / rightVal).ToString();
                                break;

                            case "+":
                                newVal = (leftVal + rightVal).ToString();
                                break;

                            case "-":
                                newVal = (leftVal - rightVal).ToString();
                                break;
                        }
                        ret.RemoveRange(iTotal, 2);
                        iTotal -= 2;
                        ret[iTotal + 1].value = newVal;
                    }
                    else{
                    }
            }

            return ret;
        }

        public static char[] startingAllowed = { 'y', 'x' };
        static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
        static char[] operators = { '*', '/', '+', '-', '^' };

        public static double parseTokens(List<Token> tokens, Dictionary<char, double> variables)
        {

            foreach (Token t in tokens)
            {
                if (t.tokenType == TokenType.Variable)
                {
                    if (variables.ContainsKey(t.value[0]))
                    {
                        t.tokenType = TokenType.Digit;
                        double val = 0;
                        variables.TryGetValue(t.value[0], out val);
                        t.value = (val.ToString());
                    }
                }
            }
            for (int i = 0; i < pemdasOrder.Length; i++)
            {
                tokens = pemdas(tokens, pemdasOrder[i]);
            }

            return Convert.ToDouble(tokens[2].value);
        }

        public static string TokensToString(List<Token> tokens)
        {
            string ret = "";
            foreach (Token t in tokens)
            {
                ret += t.value;
            }
            return ret;
        }

        public static List<Token> tokenize(char[] input, Dictionary<char, double> variables)
        {
            List<Token> tokens = new List<Token>();
            Token curToken = new Token(TokenType.Starting, "");
            foreach (char c in input)
            {
                if (c == ' ') continue;
                bool error = false;
                switch (curToken.tokenType)
                {

                    case TokenType.Starting:
                        curToken.value += c;
                        if (startingAllowed.Contains(c))
                        {
                            curToken.tokenType = TokenType.LeftVariable;
                            tokens.Add(curToken);
                            curToken = new Token(TokenType.LeftVariable, "");
                        }
                        else
                        {
                            error = true;
                        }
                        break;

                    case TokenType.LeftVariable:
                        curToken.value += c;
                        if (c == '=')
                        {
                            curToken.tokenType = TokenType.LeftAssignment;
                            tokens.Add(curToken);
                            curToken = new Token(TokenType.LeftAssignment, "");
                        }
                        else
                        {
                            error = true;
                        }
                        break;

                    case TokenType.LeftAssignment:
                        if (c == '-')
                        {
                            //tokens.Add(curToken);
                            curToken = new Token(TokenType.Negating, c.ToString());
                        }
                        else if (digits.Contains(c))
                        {
                            curToken.value += c;
                            curToken.tokenType = TokenType.Digit;
                        }
                        else if (variables.ContainsKey(c))
                        {
                            curToken = new Token(TokenType.Variable, "" + c);
                        }
                        else if (c == '"')
                        {
                            curToken = new Token(TokenType.String, "" + c);
                        }
                        else
                        {
                            error = true;
                        }
                        break;

                    case TokenType.Digit:
                        if (digits.Contains(c))
                        {
                            curToken.value += c;
                        }
                        else if (operators.Contains(c))
                        {
                            tokens.Add(curToken);
                            curToken = new Token();
                            curToken.value = c.ToString();
                            curToken.tokenType = TokenType.Operation;
                        }
                        else if (variables.ContainsKey(c))
                        {
                            tokens.Add(curToken);
                            tokens.Add(new Token(TokenType.Operation, "*"));
                            curToken = new Token(TokenType.Variable, c.ToString());
                        }
                        else
                        {
                            error = true;
                        }
                        break;

                    case TokenType.Negating:
                        if (digits.Contains(c))
                        {
                            tokens.Add(curToken);
                            curToken = new Token();
                            curToken.value = c.ToString();
                            curToken.tokenType = TokenType.Digit;
                        }
                        else if (variables.ContainsKey(c))
                        {
                            tokens.Add(curToken);
                            curToken = new Token();
                            curToken.value = c.ToString();
                            curToken.tokenType = TokenType.Variable;
                        }
                        break;

                    case TokenType.Operation:
                        if (c == '-')
                        {
                            tokens.Add(curToken);
                            curToken = new Token(TokenType.Negating, c.ToString());
                        }
                        else if (digits.Contains(c))
                        {
                            tokens.Add(curToken);
                            curToken = new Token();
                            curToken.value = c.ToString();
                            curToken.tokenType = TokenType.Digit;
                        }
                        else if (variables.ContainsKey(c))
                        {
                            tokens.Add(curToken);
                            curToken = new Token();
                            curToken.value = c.ToString();
                            curToken.tokenType = TokenType.Variable;
                        }
                        else
                        {
                            error = true;
                        }
                        break;


                    case TokenType.Variable:
                        if (variables.ContainsKey(c))
                        {
                            tokens.Add(curToken);
                            tokens.Add(new Token(TokenType.Operation, "*"));
                            curToken = new Token(TokenType.Variable, "" + c);
                        }
                        else if (operators.Contains(c))
                        {
                            tokens.Add(curToken);
                            curToken = new Token(TokenType.Operation, "" + c);
                        }

                        else if (digits.Contains(c))
                        {
                            tokens.Add(curToken);
                            tokens.Add(new Token(TokenType.Operation, "*"));
                            curToken = new Token(TokenType.Digit, "" + c);
                        }

                        else
                        {
                            error = true;
                        }

                        break;

                    case TokenType.String:
                        curToken.value += c;

                        break;
                }
                if (error)
                {
                    tokens.Add(new Token(TokenType.Error, curToken.value));
                    return tokens;
                }
            }
            if (curToken.tokenType != TokenType.Digit && curToken.tokenType != TokenType.Variable)
            {
                curToken.tokenType = TokenType.Error;
            }
            tokens.Add(curToken);
            return tokens;
        }
    }


    public class Token
    {
        public TokenType tokenType;
        public string value;

        public Token() { }

        public Token(TokenType t, string v)
        {
            tokenType = t;
            value = v;
        }
    }

    public enum TokenType
    {
        Starting,
        LeftVariable,
        LeftAssignment,
        Digit,
        Operation,
        Variable,
        Error,
        String,
        Null,
        Negating
    }
}
