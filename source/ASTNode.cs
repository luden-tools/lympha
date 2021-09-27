using System.Collections.Generic;

namespace Lympha
{
    public class ASTNode
    {
        public string head;
        public string name;
        public ASTNode pendingHead;
        public List<ASTNode> body;

        public ASTNode(List<ASTNode> body)
        {
            this.body = body;
            
            head = null;
            name = null;
            pendingHead = null;
        }

        public ASTNode(string head, string name, List<ASTNode> body = null)
        {
            this.head = head;
            this.name = name;
            this.body = body;

            pendingHead = null;
        }

        public ASTNode(ASTNode head, string name, List<ASTNode> body = null)
        {
            pendingHead = head;
            this.name = name;
            this.body = body;

            this.head = null;
        }
    }
}
