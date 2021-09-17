using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lympha;

namespace CompilerTests
{
    [TestClass]
    public class ASTNodeTests
    {
        [TestMethod]
        public void ExplicitCommand()
        {
            Compiler compiler = new();

            var ast = compiler
                .Tokenize("pedro :says hello")
                .Parse();

            Assert.IsTrue(ast.command == "says");
            Assert.IsTrue(ast.arguments.Count == 2);
            Assert.IsTrue(ast.arguments[0].command == "pedro");
            Assert.IsTrue(ast.arguments[1].command == "hello");
        }

        [TestMethod]
        public void ExplicitResultAsCommand()
        {
            Compiler compiler = new();

            var ast = compiler
                .Tokenize("pedro :(choose default-voice) hello")
                .Parse();

            Assert.IsTrue(ast.command == "says");
            Assert.IsTrue(ast.arguments.Count == 2);
            Assert.IsTrue(ast.arguments[0].command == "pedro");
            Assert.IsTrue(ast.arguments[1].command == "hello");
        }
    }
}
