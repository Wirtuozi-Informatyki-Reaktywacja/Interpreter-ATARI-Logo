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

        static MainWindow window = Application.Current.Windows[0] as MainWindow;

        static Interpreter()
        {
            HT = new Command("HT", "Hides turtle.", "HT", () => { MainWindow.turtles[0].Visibility = Visibility.Hidden; });
            ST = new Command("ST", "Shows turtle.", "ST", () => { MainWindow.turtles[0].Visibility = Visibility.Visible; });

            commandList = new List<CommandBase>
            {
                HT,
                ST
            };
        }

        public static void ProccessInput()
        {
            string input = window.input.Text;
            string[] properties = input.Split(' ');

            for (int i = 0; i < commandList.Count; i++)
            {
                CommandBase commandBase = commandList[i];

                if (input.Contains(commandBase.commandId))
                {
                    if (commandList[i] as Command != null)
                    {
                        (commandList[i] as Command).Invoke();
                        return;
                    }
                }
            }
        }
    }
}
