using System;
using Server;
using Server.Items;
using Server.Commands;

namespace Server.Custom
{
    public class FullSpellbookCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("FullSpellbook", AccessLevel.GameMaster, new CommandEventHandler(FullSpellbook_OnCommand));
        }

        [Usage("FullSpellbook")]
        [Description("Add in your backpack a full magery spellbook")]
        public static void FullSpellbook_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Crea un nuovo spellbook di magery vuoto
            Spellbook book = new Spellbook();

            // Riempi tutte le spell di Magery (i primi 64 slot)
            for (int i = 0; i < 64; i++)
            {
                book.Content |= (ulong)1 << i;
            }

            from.AddToBackpack(book);
            from.SendMessage(1152, "You have received a full magery spellbook!");
        }
    }

    public class FullSpellbookScript
    {
        public static void Initialize()
        {
            FullSpellbookCommand.Initialize();
        }
    }
}