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

        static MainWindow window = Application.Current.Windows[0] as MainWindow;

        static Interpreter()
        {
            HT = new Command("HT", "Hides turtle.", "HT", () => { MainWindow.turtles[0].Visibility = Visibility.Hidden; });
            ST = new Command("ST", "Shows turtle.", "ST", () => { MainWindow.turtles[0].Visibility = Visibility.Visible; });
            FD = new Command<int>("FD", "Shows turtle.", "FD", (x) => { window.Move(x); });
            BK = new Command<int>("BK", "Shows turtle.", "BK", (x) => { window.Move(-x); });
            RT = new Command<int>("RT", "Shows turtle.", "RT", (x) => { MainWindow.turtles[0].Rotate(x); });
            LT = new Command<int>("LT", "Shows turtle.", "LT", (x) => { MainWindow.turtles[0].Rotate(-x); });

            commandList = new List<CommandBase>
            {
                HT,
                ST,
                FD,
                BK,
                RT,
                LT
            };
        }

        public static void ProccessInput()
        {
            string input = "";

            for (int i = 0; i < window.input.LineCount; i++)
            {
                input += window.input.GetLineText(i) + " ";
            }

            List<string> properties = new List<string>(input.ToUpper().Split(' '));

            for (int i = 0; i < properties.Count; i++)
            {
                string property = properties[i];

                for (int i1 = 0; i1 < commandList.Count; i1++)
                {
                    CommandBase commandBase = commandList[i1];

                    if (property.Contains(commandBase.commandId))
                    {
                        if (commandList[i1] as Command != null)
                        {
                            (commandList[i1] as Command).Invoke();
                            //properties.RemoveAt(i);
                            break;
                        }
                        else if (commandList[i1] as Command<int> != null)
                        {
                            (commandList[i1] as Command<int>).Invoke(int.Parse(properties[i + 1]));
                            //properties.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
    }
}
