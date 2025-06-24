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
        [Description("Fornisce al tuo zaino uno spellbook di magery già pieno di tutte le magie.")]
        public static void FullSpellbook_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            Spellbook book = new Spellbook((ulong)0xFFFFFFFF, 0xFFFF); // Magery (tutte le magie)
            from.AddToBackpack(book);
            from.SendMessage(1152, "Hai ricevuto uno spellbook di magery completo!");
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