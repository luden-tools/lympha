using Lympha;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompilerTests
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void PrintHelloWorld()
        {
            Compiler compiler = new();

            var root = compiler.Tokenize("print hello-world");

            Assert.IsTrue(root.children.Count == 2);
        }

        [TestMethod]
        public void HelloWorldWithStructure()
        {
            Compiler compiler = new();

            var root = compiler.Tokenize("print (hello from (name pedro) (32 :age))");

            Assert.IsTrue(root.children.Count == 2);
        }

        [TestMethod]
        public void HelloWorldWithEncapsulatingBrackets()
        {
            Compiler compiler = new();

            var root = compiler.Tokenize("(print hello-world)");

            Assert.IsTrue(root.children.Count == 1);
            Assert.IsTrue(root.children[0].children.Count == 2);
        }

        [TestMethod]
        public void CommandAsResult()
        {
            Compiler compiler = new();

            var root = compiler.Tokenize("hello-world ((government portugal) pedro) (32 :years)");

            Assert.IsTrue(root.children.Count == 3);
        }

        [TestMethod]
        public void ExplicitResultAsCommand()
        {
            Compiler compiler = new();

            var root = compiler.Tokenize("pedro :(choose default-voice) hello");

            Assert.IsTrue(root.children.Count == 3);
            Assert.IsTrue(root.children[1].children.Count == 2);
        }
    }
}
