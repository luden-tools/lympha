using System.Collections.Generic;

namespace Lympha
{
    public class Token
    {
        public string value;
        public string name;
        public List<Token> children;
        public bool isExplicit = false;

        public Token(string value, string name = "")
        {
            this.value = value;
            this.name = name;
            this.children = null;
        }

        public Token(List<Token> children)
        {
            this.value = null;
            this.children = children;
        }
    }
}
