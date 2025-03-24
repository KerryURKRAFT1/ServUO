
/*
using Server.Gumps;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Craft
{
    public class CraftSelectionGump : Gump
    {
        public CraftSelectionGump(Mobile from, BaseTool tool) : base(50, 50)
        {
            from.SendMessage("CraftSelectionGump creato con successo."); // Debug

            AddPage(0);

            AddBackground(0, 0, 300, 200, 5054);
            AddLabel(90, 20, 0, "Select Crafting Menu");

            AddButton(40, 60, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(80, 60, 0, "Pre-AoS Menu");

            AddButton(40, 100, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(80, 100, 0, "AoS Menu");

            AddButton(40, 140, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(80, 140, 0, "Cancel");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            from.SendMessage("ButtonID premuto: " + info.ButtonID); // Debug

            Item foundItem = from.FindItemOnLayer(Layer.OneHanded);
            if (foundItem == null || !(foundItem is BaseTool))
            {
                foundItem = from.Backpack.FindItemByType(typeof(BaseTool)); // Cerca anche nello zaino
            }

            BaseTool tool = foundItem as BaseTool;
            if (tool == null)
            {
                from.SendMessage("Errore: il martello non è valido o non è presente."); // Debug
                return;
            }

            CraftSystem craftSystem;

            switch (info.ButtonID)
            {
                case 1: // Pre-AoS Menu
                    from.SendMessage("Apertura del menu Classic (Pre-AoS)."); // Debug

                    // Logica di Crafting Pre-AoS
                    craftSystem = DefBlacksmithy.CraftSystem;
                    if (craftSystem == null)
                    {
                        from.SendMessage("Errore: il CraftSystem è null."); // Debug
                        return;
                    }

                    from.SendGump(new CraftGump(from, craftSystem, tool, null));
                    from.SendMessage("Menu Pre-AoS aperto con successo."); // Debug
                    break;

                case 2: // AoS Menu
                    from.SendMessage("Tentativo di apertura del menu AoS."); // Debug

                    craftSystem = tool.CraftSystem;
                    if (craftSystem == null)
                    {
                        from.SendMessage("Errore: il CraftSystem è null."); // Debug
                        return;
                    }

                    from.SendGump(new CraftGump(from, craftSystem, tool, null));
                    from.SendMessage("Menu AoS aperto con successo."); // Debug
                    break;

                default:
                    from.SendMessage("Selezione del crafting annullata."); // Debug
                    break;
            }
        }
    }
}
*/
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Menus;

namespace Server.Engines.Craft
{
    public class CraftSelectionGump : Gump
    {
        public CraftSelectionGump(Mobile from, BaseTool tool) : base(50, 50)
        {
            from.SendMessage("CraftSelectionGump creato con successo."); // Debug

            AddPage(0);

            AddBackground(0, 0, 300, 200, 5054);
            AddLabel(90, 20, 0, "Select Crafting Menu");

            AddButton(40, 60, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(80, 60, 0, "Pre-AoS Menu");

            AddButton(40, 100, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(80, 100, 0, "AoS Menu");

            AddButton(40, 140, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(80, 140, 0, "Cancel");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            from.SendMessage("ButtonID premuto: " + info.ButtonID); // Debug

            Item foundItem = from.FindItemOnLayer(Layer.OneHanded);
            if (foundItem == null || !(foundItem is BaseTool))
            {
                foundItem = from.Backpack.FindItemByType(typeof(BaseTool)); // Cerca anche nello zaino
            }

            BaseTool tool = foundItem as BaseTool;
            if (tool == null)
            {
                from.SendMessage("Errore: il martello non è valido o non è presente."); // Debug
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Pre-AoS Menu
                    from.SendMessage("Apertura del menu Classic (Pre-AoS)."); // Debug
                    from.SendMenu(new NewCraftingMenu(from, tool.CraftSystem, tool)); // Apri il nuovo menu di crafting
                    break;

                case 2: // AoS Menu
                    from.SendMessage("Tentativo di apertura del menu AoS."); // Debug

                    CraftSystem craftSystem = tool.CraftSystem;
                    if (craftSystem == null)
                    {
                        from.SendMessage("Errore: il CraftSystem è null."); // Debug
                        return;
                    }

                    from.SendGump(new CraftGump(from, craftSystem, tool, null));
                    from.SendMessage("Menu AoS aperto con successo."); // Debug
                    break;

                default:
                    from.SendMessage("Selezione del crafting annullata."); // Debug
                    break;
            }
        }
    }
}