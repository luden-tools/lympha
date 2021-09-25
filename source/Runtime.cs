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
                    else return new Argument(new Value(node.head, parse: true));
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

    public class Value
    {
        public SupportedType Type;

        public Value(float value) : this(value as object)
        {
            Type = SupportedType.Number;
        }

        public Value(string valueAsText, bool parse = false)
        {
            if (parse && float.TryParse(valueAsText, out float valueAsFloat))
            {
                Type = SupportedType.Number;
                value = valueAsFloat;
                return;
            }

            Type = SupportedType.String;
            value = valueAsText;
        }

        public void Get(out float value) {
            validateRequestedType(SupportedType.Number);
            value = (float)this.value;
        }

        public void Get(out string value) {
            if (Type == SupportedType.Number)
            {
                value = $"{this.value}";
                return;
            }

            value = (string)this.value;
        }

        private Value(object value)
        {
            this.value = value;
        }

        private void validateRequestedType(SupportedType requestedType)
        {
            if (Type != requestedType)
            {
                string currentType;
                switch (Type)
                {
                    case SupportedType.None:
                        currentType = "None";
                        break;
                    case SupportedType.Number:
                        currentType = "Number";
                        break;
                    case SupportedType.String:
                        currentType = "String";
                        break;
                    case SupportedType.Object:
                        currentType = "Object";
                        break;
                    default:
                        currentType = "Unknown";
                        break;
                }
                throw new Exception($"requested type doesn't match stored {currentType} type");
            }
        }

        public enum SupportedType
        {
            None,
            Number,
            String,
            Object,
        }

        private object value;
    }

    public class Argument : Node
    {
        public Value Value { get; private set; }

        public Argument(Value value)
        {
            Type = NodeType.Argument;
            Value = value;
        }
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
                .Aggregate((left, right) => 
                {
                    left.Get(out string vleft);
                    right.Get(out string vright);
                    return new Value($"{vleft} {vright}", parse: false);
                });

            output.Get(out string outputText);

            Console.WriteLine(outputText);

            return new Argument(output);
        }
    }

    public class Sum : Command
    {
        public Sum(List<Node> args) : base("sum", args) { }

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
                .Aggregate((left, right) =>
                {
                    left.Get(out float vleft);
                    right.Get(out float vright);
                    return new Value(vleft + vright);
                });

            output.Get(out string outputText);

            Console.WriteLine(outputText);

            return new Argument(output);
        }
    }
}
