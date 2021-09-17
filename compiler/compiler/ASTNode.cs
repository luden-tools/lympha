using System;
using System.Collections.Generic;
using System.Linq;

namespace Lympha
{
    public class ASTNode
    {
        public string command;
        public ASTNode pendingCommand;
        public List<ASTNode> arguments;

        public ASTNode(List<ASTNode> arguments)
        {
            command = null;
            this.arguments = arguments;
        }

        public ASTNode(string command, List<ASTNode> arguments = null)
        {
            this.command = command;
            this.arguments = arguments;
        }
    }
}
