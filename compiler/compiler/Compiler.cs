using System;
using System.Linq;
using System.Collections.Generic;

namespace Lympha
{
    public class Compiler
    {
        public Program Compile(string code)
        {
            var program = new Program();

            

            return program;
        }

        public Token Tokenize(string code)
        {
            code = code.Trim();

            var tokens = new List<Token>();
            string currentToken;
            char terminationChar;
            uint depth;

            while (code.Length > 0)
            {
                currentToken = "";
                terminationChar = ' ';
                depth = 0;

                if (code[0] == '(')
                {
                    terminationChar = ')';
                    code = code.Remove(0, 1); // skip the ( character
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
                var token = terminationChar switch
                {
                    ' ' => new Token(currentToken),
                    ')' => Tokenize(currentToken),
                    _ => throw new Exception("invalid termination character"),
                };

                tokens.Add(token);

                code = code.TrimStart();
            }

            return new Token(tokens);
        }

        public static ASTNode FromToken(Token root)
        {
            var firstBorn = root.children?.FirstOrDefault();

            if (firstBorn == null)
            {
                return CommandOrResult(root.value, null);
            }

            var explicitCommand = root.children
                .FirstOrDefault(child => child.value?.StartsWith(':') ?? false);

            if (explicitCommand == null)
            {
                return CommandOrResult(
                    firstBorn.value,
                    root.children
                        .FindAll(child => child != firstBorn)
                );
            }
            else
            {
                return CommandOrResult(
                    explicitCommand.value,
                    root.children
                        .FindAll(child => child != explicitCommand)
                );
            }

            ASTNode CommandOrResult(string commandValue, List<Token> children)
            {
                if (commandValue == null)
                {
                    if (children?.Count == 0)
                    {
                        throw new Exception("token has neither children nor a value");
                    }

                    var pendingCommandChildren = children?
                        .Select(child => FromToken(child))
                        .ToList()
                        ?? new List<ASTNode>();

                    return new ASTNode(pendingCommandChildren);
                }

                var command = commandValue.TrimStart(':');
                var commandArguments = children?
                    .Select(child => FromToken(child))
                    .ToList()
                    ?? new List<ASTNode>();

                return new ASTNode(command, commandArguments);
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
            return Compiler.FromToken(token);
        }
    }
}
