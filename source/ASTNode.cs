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
            this.body = body;
            
            head = null;
            pendingHead = null;
        }

        public ASTNode(string head, List<ASTNode> body = null)
        {
            this.head = head;
            this.body = body;
            
            pendingHead = null;
        }

        public ASTNode(ASTNode head, List<ASTNode> body = null)
        {
            pendingHead = head;
            this.body = body;

            this.head = null;
        }
    }
}
