using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lympha
{
    public class Token
    {
        public string value;
        public List<Token> children;

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
