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
                var token = terminationChar switch
                {
                    ' ' => new Token(currentToken),
                    ')' => Tokenize(currentToken),
                    _ => throw new Exception("invalid termination character"),
                };

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
    }
}
