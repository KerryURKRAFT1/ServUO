using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Menus.ItemLists
{
    public class ClassicCraftGump : ItemListMenu
    {
        private Mobile m_Mobile;
        private BaseTool m_Tool;
        private ItemListEntry[] m_Entries;

        public ClassicCraftGump(Mobile m, ItemListEntry[] entries, BaseTool tool) : base("What would you like to make?", entries)
        {
            m_Mobile = m;
            m_Tool = tool;
            m_Entries = entries;
        }

        //MAIN
        public static ItemListEntry[] Main(Mobile from)
        {
            ItemListEntry[] entries = new ItemListEntry[4];

            entries[0] = new ItemListEntry("Repair", 4015);

            entries[1] = new ItemListEntry("Shields", 7026);

            entries[2] = new ItemListEntry("Armor", 5141);

            entries[3] = new ItemListEntry("Weapons", 5049);

            return entries;
        }
    }
}