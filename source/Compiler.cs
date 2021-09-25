using System;
using System.Linq;
using System.Collections.Generic;

namespace Lympha
{
    public class Compiler
    {
        public Value Compile(string source, CommandsContext context = null)
        {
            var result = 
                Tokenize(source)
                .Parse()
                .ToRuntime(context)
                .Run();

            return (result as Argument).Value;
        }

        public Token Tokenize(string code)
        {
            code = code.Trim();

            var tokens = new List<Token>();
            string currentToken;
            char terminationChar;
            uint depth;
            int charIndex;

            while (code.Length > 0)
            {
                currentToken = "";
                terminationChar = ' ';
                depth = 0;

                charIndex = code[0] == ':' ? 1 : 0;

                if (code[charIndex] == '(')
                {
                    terminationChar = ')';
                    code = code.Remove(0, 1 + charIndex); // skip the '(' or ':(' character(s)
                }
                else
                {
                    if (charIndex > 0)
                    {
                        code = code.Remove(0, 1); // skip the ':' character
                    }
                }

                while (code[0] != terminationChar || depth > 0)
                {
                    if (code[0] == '(')
                        ++depth;

                    if (code[0] == ')')
                        --depth;

                    currentToken += code[0];
                    code = code.Remove(0, 1); // advance to the next character

                    if (code.Length == 0) goto finished_token;
                }

                code = code.Remove(0, 1); // skip the termination character

            finished_token:
                Token token;
                if (terminationChar == ' ') token = new Token(currentToken);
                else if (terminationChar == ')') token = Tokenize(currentToken);
                else throw new Exception("invalid termination character");

                token.isExplicit = charIndex == 1;

                tokens.Add(token);

                code = code.TrimStart();
            }

            return new Token(tokens);
        }

        public static ASTNode Parse(Token root)
        {
            var firstBorn = root.children?.FirstOrDefault();

            if (firstBorn == null)
            {
                return CreateNode(root, null);
            }

            var explicitHead = root.children
                .FirstOrDefault(child => child.isExplicit);

            if (explicitHead == null)
            {
                return CreateNode(
                    firstBorn,
                    root.children
                        .FindAll(child => child != firstBorn)
                );
            }
            else
            {
                return CreateNode(
                    explicitHead,
                    root.children
                        .FindAll(child => child != explicitHead)
                );
            }

            ASTNode CreateNode(Token headToken, List<Token> bodyTokens)
            {
                var body = bodyTokens?
                    .Select(child => Parse(child))
                    .ToList()
                    ?? new List<ASTNode>();

                if (headToken.value != null)
                {
                    return new ASTNode(headToken.value, body);
                }
                else
                {
                    if (bodyTokens?.Count == 0)
                    {
                        throw new Exception("token has neither children nor a value");
                    }

                    var pendingBody = headToken.children?
                        .Select(child => Parse(child))
                        .ToList()
                        ?? new List<ASTNode>();

                    var pendingHead = new ASTNode(pendingBody);

                    return new ASTNode(pendingHead, body);
                }
            }
        }

        static void Main(string[] args)
        {
            
        }
    }

    public static class CompilerExtensions
    {
        public static ASTNode Parse(this Token token)
        {
            return Compiler.Parse(token);
        }

        public static Node ToRuntime(this ASTNode root, CommandsContext context = null)
        {
            if (context == null)
            {
                context = new CommandsContext()
                {
                    { "print", args => new Print(args) },
                    { "sum", args => new Sum(args) }
                };
            }

            return Runtime.FromAST(root, context);
        }

        public static Node Run(this Node root)
        {
            if (root.Type != NodeType.Command) throw new Exception("root element is not a command");

            var result = (root as Command).Execute();

            return result;
        }
    }
}
