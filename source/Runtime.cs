using System;
using System.Collections.Generic;
using System.Linq;

namespace Lympha
{
    class Runtime
    {
        public static Node FromAST(ASTNode root, CommandsContext context)
        {
            List<Node> arguments = root.body
                .Select(node =>
                {
                    if (node.body.Count > 0) return FromAST(node, context);
                    else return new Argument(node.head);
                })
                .ToList();

            Command command;
            if (root.pendingHead != null)
                command = new FutureCommand(arguments);
            else if (root.head != null)
                command = context.CreateCommand(root.head, arguments);
            else 
                throw new Exception("node doesn't have an head nor a pendingHead");

            return command;
        }
    }

    public enum NodeType
    {
        Command,
        Argument,
    }

    public class Node
    {
        public NodeType Type;
    }

    public class Command : Node
    {
        public string Symbol { get; private set; }

        public Command(string symbol, List<Node> args, bool pendingCommand = false)
        {
            Type = NodeType.Command;
            Symbol = symbol;

            this.args = args;
            this.pendingCommand = pendingCommand;
        }

        protected virtual Node Run()
        {
            if (!pendingCommand) return null;

            var pending = args.FirstOrDefault() as Command;

            if (pending.Type != NodeType.Command) throw new Exception("pending command is not a command");

            dynamicCommand = pending.Execute() as Command;

            return dynamicCommand.Run();
        }

        public Node Execute()
        {
            args.Select(arg =>
                {
                    switch (arg.Type)
                    {
                        case NodeType.Command:
                            var command = (Command)arg;
                            return command.Execute();
                        default:
                            return arg;
                    }
                }
            ).ToList();

            return Run();
        }

        protected List<Node> args;
        private bool pendingCommand;
        private Command dynamicCommand;
    }

    public class Argument : Node
    {
        public string Value { get; private set; }

        public Argument(string value)
        {
            Type = NodeType.Argument;
            Value = value;
        }
    }

    public static class Commands
    {
        public static Command CreateCommand(string symbol, List<Node> arguments)
        {
            if (CommandsMap.ContainsKey(symbol)) return CommandsMap[symbol](arguments);
            else return CommandsMap["<no-op>"](arguments);
        }

        public static CommandsContext CommandsMap = new CommandsContext()
        {
            { "print", args => new Print(args) },
            { "<no-op>", args => new Print(args) },
        };
    }

    public class CommandsContext : Dictionary<string, Func<List<Node>, Command>>
    {
        public Command CreateCommand(string head, List<Node> arguments)
        {
            return this[head](arguments);
        }
    }

    public class FutureCommand : Command
    {
        public FutureCommand(List<Node> args) : base("future", args, true) { }
    }

    public class Print : Command
    {
        public Print(List<Node> args) : base("print", args) { }

        protected override Node Run()
        {
            var output = args
                .Select(arg =>
                {
                    switch (arg.Type)
                    {
                        case NodeType.Command:
                            return (arg as Command).Run() as Argument;
                        default:
                            return arg as Argument;
                    }
                })
                .Select(arg => arg.Value)
                .Aggregate((left, right) => $"{left} {right}");

            Console.WriteLine(output);

            return new Argument(output);
        }
    }
}
