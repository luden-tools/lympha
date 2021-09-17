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

            Assert.IsTrue(ast.head == "says");
            Assert.IsTrue(ast.body.Count == 2);
            Assert.IsTrue(ast.body[0].head == "pedro");
            Assert.IsTrue(ast.body[1].head == "hello");
        }

        [TestMethod]
        public void ExplicitResultAsCommand()
        {
            Compiler compiler = new();

            var token = compiler.Tokenize("pedro :(choose default-voice portugal) hello");
            var ast = token.Parse();

            Assert.IsTrue(ast.body.Count == 2);
            Assert.IsTrue(ast.pendingHead.body.Count == 3);
            Assert.IsTrue(ast.body[0].head == "pedro");
            Assert.IsTrue(ast.body[1].head == "hello");
        }
    }
}
