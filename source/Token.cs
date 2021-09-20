using System.Collections.Generic;

namespace Lympha
{
    public class Token
    {
        public string value;
        public List<Token> children;
        public bool isExplicit = false;

        public Token(string value)
        {
            this.value = value;
            this.children = null;
        }

        public Token(List<Token> children)
        {
            this.value = null;
            this.children = children;
        }
    }
}
