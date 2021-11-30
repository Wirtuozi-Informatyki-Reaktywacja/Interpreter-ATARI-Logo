using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter_ATARI_Logo
{
    public interface ICommand
    {
        public void Invoke();
    }

    public class CommandBase
    {
        private string _commandId;
        private string _commandDescription;
        private string _commandFormat;

        public string commandId { get { return _commandId; } }
        public string commandDescription { get { return _commandDescription; } }
        public string commandFormat { get { return _commandFormat; } }

        public CommandBase(string commandId, string commandDescription, string commandFormat)
        {
            _commandId = commandId;
            _commandDescription = commandDescription;
            _commandFormat = commandFormat;
        }
    }
    public class Command : CommandBase, ICommand
    {
        private Action command;

        public Command(string id, string description, string format, Action command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke()
        {
            command.Invoke();
        }
    }

    public class Command<T1> : CommandBase, ICommand
    {
        private Action<T1> command;
        private T1 param;

        public Command(string id, string description, string format, Action<T1> command) : base(id, description, format)
        {
            this.command = command;
        }

        public Command(Command<T1> _command, T1 param) : base(_command.commandId, _command.commandDescription, _command.commandFormat)
        {
            command = _command.command;
            this.param = param;
        }

        public void Invoke()
        {
            if (param != null)
            {
                command.Invoke(param);
            }
        }
    }

    public class Command<T1, T2> : CommandBase, ICommand
    {
        private Action<T1, T2> command;
        private T1 param1;
        private T2 param2;

        public Command(string id, string description, string format, Action<T1, T2> command) : base(id, description, format)
        {
            this.command = command;
        }

        public Command(Command<T1, T2> _command, T1 param1, T2 param2) : base(_command.commandId, _command.commandDescription, _command.commandFormat)
        {
            command = _command.command;
            this.param1 = param1;
            this.param2 = param2;
        }

        public void Invoke()
        {
            if (param1 != null && param2 != null)
            {
                command.Invoke(param1, param2);
            }
        }
    }
}
