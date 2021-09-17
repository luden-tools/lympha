using System.Collections.Generic;

namespace Lympha
{
    public class ASTNode
    {
        public string head;
        public ASTNode pendingHead;
        public List<ASTNode> body;

        public ASTNode(List<ASTNode> body)
        {
            head = null;
            this.body = body;
        }

        public ASTNode(string head, List<ASTNode> body = null)
        {
            this.head = head;
            this.body = body;
        }

        public ASTNode(ASTNode head, List<ASTNode> body = null)
        {
            this.pendingHead = head;
            this.body = body;
        }
    }
}
