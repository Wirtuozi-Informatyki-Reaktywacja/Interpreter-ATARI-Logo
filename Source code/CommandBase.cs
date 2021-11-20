using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter_ATARI_Logo
{
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
    public class Command : CommandBase
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

    public class Command<T1> : CommandBase
    {
        private Action<T1> command;

        public Command(string id, string description, string format, Action<T1> command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke(T1 value)
        {
            command.Invoke(value);
        }
    }

    public class Command<T1, T2> : CommandBase
    {
        private Action<T1, T2> command;

        public Command(string id, string description, string format, Action<T1, T2> command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke(T1 value1, T2 value2)
        {
            command.Invoke(value1, value2);
        }
    }
}
