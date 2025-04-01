using System;
using Server.Commands;

namespace Server.Scripts.Commands
{
    public class CoreInfoCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("CoreInfo", AccessLevel.Player, new CommandEventHandler(CoreInfo_OnCommand));
        }

        [Usage("CoreInfo")]
        [Description("Displays the current Core version in use.")]
        public static void CoreInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (Core.AOS)
            {
                from.SendMessage("The server is running on AOS Core.");
            }
            else if (Core.ML)
            {
                from.SendMessage("The server is running on ML Core.");
            }
            else if (Core.SE)
            {
                from.SendMessage("The server is running on SE Core.");
            }
             else if (Core.UOR)
            {
                from.SendMessage("The server is running on UOR Core.");
            }
            else
            {
                from.SendMessage("The server is running on an unknown Core version.");
            }
        }
    }
}