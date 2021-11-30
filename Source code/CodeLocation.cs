using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter_ATARI_Logo
{
    internal struct CodeLocation
    {
        public CodeLocation(int line, int character)
        {
            Line = line;
            Character = character;
        }

        public int Line { get; set; }
        public int Character { get; set; }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CodeLocation p = (CodeLocation)obj;
                return (Line == p.Line) && (Character == p.Character);
            }
        }

        public override string ToString() => $"character {Character + 1} in line {Line + 1}";
        public string ToString(bool original) => $"character {Character} in line {Line}";

        public override int GetHashCode() => (Line, Character).GetHashCode();

        public static bool operator ==(CodeLocation lhs, CodeLocation rhs) => lhs.Equals(rhs);

        public static bool operator !=(CodeLocation lhs, CodeLocation rhs) => !(lhs == rhs);
    }
}