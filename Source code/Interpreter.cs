using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;

namespace Interpreter_ATARI_Logo
{
    public static class Interpreter
    {
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
        public static Command<int, List<string>> REPEAT;

        static MainWindow window = Application.Current.Windows[0] as MainWindow;

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
            REPEAT = new Command<int, List<string>>("REPEAT", "Repeats instructions specified amount of times.", "REPEAT <amount> [<instructions>]", (x, y) =>
            {
                for (int i = 0; i < x; i++)
                {
                    ProccessInput(y);
                }
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

        public static void ProccessInput(List<string> properties = null)
        {
            if (properties == null)
            {
                string input = "";

                for (int i = 0; i < window.input.LineCount; i++)
                {
                    input += window.input.GetLineText(i) + " ";
                }

                properties = new List<string>(input.ToUpper().Split(' '));
            }

            for (int i = 0; i < properties.Count; i++)
            {
                string property = properties[i];

                for (int i1 = 0; i1 < commandList.Count; i1++)
                {
                    CommandBase commandBase = commandList[i1];

                    if (property.Contains(commandBase.commandId))
                    {
                        if ((commandList[i1] as Command) != null)
                        {
                            (commandList[i1] as Command).Invoke();
                            break;
                        }
                        else if ((commandList[i1] as Command<int>) != null)
                        {
                            try
                            {
                                (commandList[i1] as Command<int>).Invoke(int.Parse(properties[i + 1]));
                            }
                            catch
                            {
                                //window.PrintLine(properties[i + 1]);
                            }
                            i++;
                            break;
                        }
                        else if ((commandList[i1] as Command<int, List<string>>) != null)
                        {
                            if (ProccessInputArray(properties, i + 2, out int end))
                            {
                                List<string> array = new List<string>();
                                for (int i2 = i + 2; i2 <= end; i2++)
                                {
                                    string arrayContent = properties[i2];

                                    if (i2 == i || i2 == end)
                                    {
                                        arrayContent = arrayContent.Trim(new char[] { '[', ']' });
                                    }

                                    if (arrayContent != "")
                                    {
                                        window.PrintLine(arrayContent);
                                        array.Add(arrayContent);
                                    }
                                }

                                (commandList[i1] as Command<int, List<string>>).Invoke(int.Parse(properties[i + 1]), array);
                                i = end;
                            }
                        }
                    }
                }
            }
        }

        private static bool ProccessInputArray(List<string> properties, int start, out int end)
        {
            end = -1;

            if (properties[start].Contains('['))
            {
                bool foundEnd = false;

                for (int i = start; i < properties.Count; i++)
                {
                    string property = properties[i];

                    if (property.Contains('[') && i != start)
                    {
                        if (ProccessInputArray(properties, i, out int end2))
                        {
                            i = end2;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (property.Contains(']'))
                    {
                        foundEnd = true;
                        end = i;
                        break;
                    }
                }

                return foundEnd;
            }

            return false;
        }
    }
}
