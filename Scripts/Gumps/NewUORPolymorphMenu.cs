using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Spells.Seventh;
using Server.Menus.ItemLists;

namespace Server.CustomMenus
{
    public class NewUORPolymorphMenu : ItemListMenu
    {
        private readonly Mobile m_Caster;
        private readonly Item m_Scroll;

        public NewUORPolymorphMenu(Mobile caster, Item scroll)
            : base("Choose a form to Polymorph into", GetPolymorphEntries())
        {
            m_Caster = caster;
            m_Scroll = scroll;
        }



        private static ItemListEntry[] GetPolymorphEntries()
        {
            PolymorphForms = new List<PolymorphEntry>
            {
                //new PolymorphEntry("Chicken", 0xD0),
                new PolymorphEntry("Slime", 8424),
                new PolymorphEntry("walrus", 8447),
                new PolymorphEntry("Dog", 8476),
                new PolymorphEntry("horse", 8480),
                //new PolymorphEntry("alligator", 8480),
                new PolymorphEntry("Wolf", 8426),
                new PolymorphEntry("Panther", 8473),
                new PolymorphEntry("Gorilla", 8393),
                new PolymorphEntry("serpent", 8444),
                new PolymorphEntry("Black Bear", 8472),
                new PolymorphEntry("Grizzly Bear", 8411),
                new PolymorphEntry("Polar Bear", 8417),
                new PolymorphEntry("Skeleton knight", 8423),
                new PolymorphEntry("Giant Scorpion", 8420),
                new PolymorphEntry("Ettin", 8408),
                new PolymorphEntry("Gargoyle", 8409),
                new PolymorphEntry("Ogre", 8415),
                new PolymorphEntry("Troll", 8425),
                new PolymorphEntry("Orc", 8416),
                new PolymorphEntry("Lizardman", 8394),
                new PolymorphEntry("Human Male", 8397),
                new PolymorphEntry("Human Female", 8398),
                new PolymorphEntry("beholder", 8436),
                new PolymorphEntry("vorteex", 8429),
                new PolymorphEntry("kraken", 8402),
                new PolymorphEntry("Daemon", 8452),
                new PolymorphEntry("Dragon", 8406)
            };

            var entries = new List<ItemListEntry>();
            foreach (var form in PolymorphForms)
            {
                entries.Add(new ItemListEntry(form.Name, form.Body));
            }

            return entries.ToArray();
        }

        public static List<PolymorphEntry> PolymorphForms { get; private set; }


        public class PolymorphEntry
        {
            public string Name { get; private set; }
            public int Body { get; private set; }

            public PolymorphEntry(string name, int body)
            {
                Name = name;
                Body = body;
            }
        }

                public override void OnResponse(NetState state, int index)
        {
            Mobile from = state.Mobile;

            if (index < 0 || index >= PolymorphForms.Count)
            {
                from.SendMessage("You decide not to polymorph.");
                return;
            }

            // Ottieni la forma selezionata
            var entry = PolymorphForms[index];
            if (entry != null)
            {
                // Lancia la spell di Polymorph con il body selezionato
                var spell = new PolymorphSpell(from, m_Scroll, entry.Body);
                spell.Cast();
            }
        }

        
    }
}