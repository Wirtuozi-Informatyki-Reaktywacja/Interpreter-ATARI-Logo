using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace Interpreter_ATARI_Logo
{
    public static class Interpreter
    {
        enum ParamFilter
        {
            Number,
            String,
            Array
        }

        public static List<CommandBase> commandList;

        public static Command HT;
        public static Command ST;
        public static Command<int> FD;
        public static Command<int> BK;
        public static Command<int> RT;
        public static Command<int> LT;
        public static Command PU;
        public static Command PD;
        public static Command CS;
        public static Command<int, Queue<ICommand>> REPEAT;

        static Queue<ICommand> commandQueue = new Queue<ICommand>();

        static MainWindow window = Application.Current.Windows[0] as MainWindow;
        static TextBox inputField = window.input;

        static Interpreter()
        {
            HT = new Command("HT", "Hides turtle.", "HT", () => { MainWindow.turtles[0].Visibility = Visibility.Hidden; });
            ST = new Command("ST", "Shows turtle.", "ST", () => { MainWindow.turtles[0].Visibility = Visibility.Visible; });
            FD = new Command<int>("FD", "Moves turtle forwards.", "FD <distance>", (x) => { window.Move(x); });
            BK = new Command<int>("BK", "Moves turtle backwards.", "BK <distance>", (x) => { window.Move(-x); });
            RT = new Command<int>("RT", "Turns turtle right by asked angle (in degrees).", "RT <angle>", (x) => { MainWindow.turtles[0].Rotate(x); });
            LT = new Command<int>("LT", "Turns turtle left by asked angle (in degrees).", "LT <angle>", (x) => { MainWindow.turtles[0].Rotate(-x); });
            PU = new Command("PU", "Turtle trail turned off.", "PU", () => { window.PenDown = false; });
            PD = new Command("PD", "Turtle trail turned on.", "PD", () => { window.PenDown = true; });
            CS = new Command("CS", "Clears turtle trails.", "CS", () => { window.ClearScreen(); });
            REPEAT = new Command<int, Queue<ICommand>>("REPEAT", "Repeats instructions specified amount of times.", "REPEAT <amount> [<instructions>]", (x, y) =>
            {
                for (int i = 0; i < x; i++)
                {
                    CommandExecute(y);
                }
                y.Clear();
            });

            commandList = new List<CommandBase>
            {
                HT,
                ST,
                FD,
                BK,
                RT,
                LT,
                PU,
                PD,
                CS,
                REPEAT
            };
        }

        public static void Execute()
        {
            StringCollection lines = new StringCollection();

            for (int i = 0; i < inputField.LineCount; i++)
            {
                lines.Add(inputField.GetLineText(i).ToUpper().Trim());
            }

            if (InputParser(lines, out commandQueue))
            {
                CommandExecute(commandQueue);
            }
            commandQueue.Clear();
        }

        private static bool InputParser(StringCollection lines, out Queue<ICommand> queue)
        {
            queue = new Queue<ICommand>();
            CodeLocation location = new CodeLocation();

            for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
            {
                location.Line = lineIndex;
                string line = lines[lineIndex];

                for (int characterIndex = 0; characterIndex < lines[lineIndex].Length; characterIndex++)
                {
                    location.Character = characterIndex;
                    char character = line[characterIndex];

                    if (char.IsLetter(character))
                    {
                        string property = GetConnectedLetters(line, characterIndex, out characterIndex);

                        for (int commandIndex = 0; commandIndex < commandList.Count; commandIndex++)
                        {
                            var command = commandList[commandIndex];

                            if (property.Contains(command.commandId))
                            {
                                if ((command as Command) != null)
                                {
                                    queue.Enqueue(command as ICommand);
                                    break;
                                }
                                else if ((command as Command<int>) != null)
                                {
                                    string paramPreParse = GetParameter(line, characterIndex + 2, out int paramEnd);

                                    if (int.TryParse(paramPreParse, out int param))
                                    {
                                        characterIndex = paramEnd;
                                        Command<int> commandClone = new Command<int>(command as Command<int>, param);

                                        queue.Enqueue(commandClone);
                                        break;
                                    }
                                    else
                                    {
                                        window.PrintLine($"Error at {location} - parameter is not correct");
                                        return false;
                                    }
                                }
                                else if ((command as Command<int, Queue<ICommand>>) != null)
                                {
                                    string paramPreParse = GetParameter(line, characterIndex + 2, out int paramEnd);

                                    if (int.TryParse(paramPreParse, out int param))
                                    {
                                        CodeLocation arrayStart;

                                        if (paramEnd + 2 >= line.Length)
                                        {
                                            arrayStart = new CodeLocation(lineIndex + 1, 0);
                                        }
                                        else
                                        {
                                            arrayStart = new CodeLocation(lineIndex, paramEnd + 2);
                                        }

                                        if (FindArray(lines, arrayStart, out CodeLocation arrayEnd))
                                        {
                                            lineIndex = arrayEnd.Line;
                                            characterIndex = arrayEnd.Character;

                                            StringCollection collection = Trim(lines, 
                                                new CodeLocation(arrayStart.Line, arrayStart.Character + 1), 
                                                new CodeLocation(arrayEnd.Line, arrayEnd.Character - 1));

                                            if (InputParser(collection, out Queue<ICommand> repeatQueue))
                                            {
                                                Command<int, Queue<ICommand>> commandClone = new Command<int,Queue<ICommand>>
                                                    (command as Command<int, Queue<ICommand>>,
                                                    param, repeatQueue);

                                                queue.Enqueue(commandClone);
                                                break;
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        window.PrintLine($"Error at {location} - parameter is not correct");
                                        return false;
                                    }
                                }
                                else
                                {
                                    window.PrintLine($"Error at {location} - command not found");
                                    return false;
                                }
                            }
                            else if (commandIndex == commandList.Count - 1)
                            {
                                window.PrintLine($"Error at {location} - command not found");
                                return false;
                            }
                        }
                    }
                    else if (char.IsWhiteSpace(character))
                    {
                        continue;
                    }
                    else
                    {
                        window.PrintLine($"Error at {location} - command cannot be a number");
                        return false;
                    }
                }
            }

            return true;
        }

        private static string GetConnectedLetters(string line, int start, out int end)
        {
            string output = "";
            end = start;

            for (int i = start; i < line.Length; i++)
            {
                if (char.IsWhiteSpace(line, i))
                {
                    break;
                }
                else
                {
                    output += line[i];
                }
                end = i;
            }

            return output;
        }

        private static string GetParameter(string line, int start, out int end)
        {
            string output = "";
            end = start;

            for (int i = start; i < line.Length; i++)
            {
                if (char.IsWhiteSpace(line, i))
                {
                    break;
                }
                else
                {
                    output += line[i];
                }

                end = i;
            }

            return output;
        }

        private static bool FindArray(StringCollection lines, CodeLocation start, out CodeLocation end)
        {
            CodeLocation location = new CodeLocation();

            end = start;

            for (int lineIndex = start.Line; lineIndex < lines.Count; lineIndex++)
            {
                location.Line = lineIndex;
                string line = lines[lineIndex];

                for (int characterIndex = 0; characterIndex < line.Length; characterIndex++)
                {
                    location.Character = characterIndex;
                    char character = line[characterIndex];

                    if (lineIndex == start.Line && characterIndex < start.Character)
                    {
                        continue;
                    }

                    if (character != '[' && location == start)
                    {
                        window.PrintLine($"Error at {location} - missing '['");
                        return false;
                    }
                    else if (character == '[' && location != start)
                    {
                        if (FindArray(lines, location, out CodeLocation end1))
                        {
                            lineIndex = end1.Line;
                            characterIndex = end1.Character;
                        }
                        else
                        {
                            window.PrintLine($"Error at {location} - array not closed");
                            return false;
                        }
                    }
                    else if (character == ']')
                    {
                        end = location;
                        return true;
                    }
                }
            }

            window.PrintLine($"Error at {location} - array not found");
            return false;
        }

        private static StringCollection Trim(StringCollection collection, CodeLocation start, CodeLocation end)
        {
            StringCollection output = new StringCollection();

            for (int lineIndex = start.Line; lineIndex <= end.Line; lineIndex++)
            {
                string line = collection[lineIndex];
                string outputString = "";

                for (int characterIndex = (lineIndex == start.Line) ? start.Character : 0; (lineIndex == end.Line) ? characterIndex <= end.Character : characterIndex < line.Length; characterIndex++)
                {
                    char character = line[characterIndex];

                    outputString += character;
                }

                output.Add(outputString);
            }

            return output;
        }

        private static void CommandExecute(Queue<ICommand> queue)
        {
            foreach (ICommand command in queue)
            {
                command.Invoke();
            }
        }
    }
}
