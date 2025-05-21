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
            : base("Choose a form to Polymorph into", GetPolymorphEntries(caster))
        {
            m_Caster = caster;
            m_Scroll = scroll;
        }

        // Filtra le forme disponibili in base alla skill di magery del caster
        private static ItemListEntry[] GetPolymorphEntries(Mobile caster)
        {
            double magery = caster.Skills[SkillName.Magery].Value;
            List<PolymorphEntry> options = new List<PolymorphEntry>();

            if (magery >= 30)
            {
                options.Add(new PolymorphEntry("Slime", 8424, 0X33, 27, 17 , 20));
                options.Add(new PolymorphEntry("Walrus", 8447, 0XDD, 27, 47, 17));
                options.Add(new PolymorphEntry("Dog", 8476, 0xD9, 33, 40, 33));
                options.Add(new PolymorphEntry("Horse", 8480, 0xc8, 66, 66, 66));
                options.Add(new PolymorphEntry("Wolf", 8426, 0xE1, 58, 33, 58));
                options.Add(new PolymorphEntry("Panther", 8473, 0xD6, 80, 89, 42));
                options.Add(new PolymorphEntry("Gorilla", 8393, 0x1D, 77, 53, 48 ));
            }
            if (magery >= 70)
            {
                options.Add(new PolymorphEntry("Giant serpent", 8444, 0x15, 199, 71, 69));
                options.Add(new PolymorphEntry("Black Bear", 8472, 0xD3, 99, 63,12));
                options.Add(new PolymorphEntry("Grizzly Bear", 8411, 0xD4, 152, 84, 35));
                options.Add(new PolymorphEntry("Polar Bear", 8417, 0xD5, 120, 82, 47));
                options.Add(new PolymorphEntry("Skeleton knight", 8423, 0x32, 63, 60, 31));
            }
            if (magery >= 80)
            {
                options.Add(new PolymorphEntry("Giant Scorpion", 8420, 0x30, 101, 84, 28));
                options.Add(new PolymorphEntry("Ettin", 8408, 0x02, 152, 71, 48));
                options.Add(new PolymorphEntry("Gargoyle", 8409, 0x04, 163, 93, 94 ));
                options.Add(new PolymorphEntry("Ogre", 8415, 0x01, 155, 73, 55 ));
                options.Add(new PolymorphEntry("Troll", 8425, 0x36, 189, 53, 46));
                options.Add(new PolymorphEntry("Orc", 8416, 0x11, 105, 84, 45));
                options.Add(new PolymorphEntry("Lizardman", 8394, 0x21, 108, 96, 44));
                options.Add(new PolymorphEntry("Human Male", 8397, 0x190, 50, 50, 50));
                options.Add(new PolymorphEntry("Human Female", 8398, 0x191, 50, 50, 50));
            }
            if (magery >= 90)
            {
                options.Add(new PolymorphEntry("Beholder", 8436, 0x16, 102, 107, 150));
                options.Add(new PolymorphEntry("Vortex", 8429, 0xA4, 130, 180, 114));
            }
            if (magery >= 100)
            {
                options.Add(new PolymorphEntry("Kraken", 8402, 0x4D, 200, 234, 33 ));
                options.Add(new PolymorphEntry("Daemon", 8452, 0x9, 250, 87, 300));
                options.Add(new PolymorphEntry("Dragon", 8406, 0x3b, 300, 100, 300 ));
            }

            // Salva la lista per OnResponse
            PolymorphForms = options;

            var entries = new List<ItemListEntry>();
            foreach (var form in options)
                entries.Add(new ItemListEntry(form.Name, form.ImageID));

            return entries.ToArray();
        }

        // Lista delle forme attualmente disponibili nel menu
        public static List<PolymorphEntry> PolymorphForms { get; private set; }

        public class PolymorphEntry
        {
            public string Name { get; private set; }
            public int ImageID { get; private set; }
            public int BodyID { get; private set; }
            public int Strength { get; private set; }
            public int Dexterity { get; private set; }
            public int Intelligence { get; private set; }

            public PolymorphEntry(string name, int imageID, int bodyID, int strength = 0, int dexterity = 0, int intelligence = 0)
            {
                Name = name;
                ImageID = imageID;
                BodyID = bodyID;
                Strength = strength;
                Dexterity = dexterity;
                Intelligence = intelligence;
            }
        }

        public override void OnResponse(NetState state, int index)
        {
            Mobile from = state.Mobile;

            if (PolymorphForms == null || index < 0 || index >= PolymorphForms.Count)
            {
                from.SendMessage("You decide not to polymorph.");
                return;
            }

            // Ottieni la forma selezionata
            PolymorphEntry selection = PolymorphForms[index];
            if (selection != null)
            {
                from.SendMessage("You transform into: " + selection.Name);

                // Esegui la spell di Polymorph con il corpo selezionato
                PolymorphSpell spell = new PolymorphSpell(from, m_Scroll, selection.BodyID); 
                spell.Cast();
            }
            else
            {
                from.SendMessage("An error occurred while attempting to polymorph.");
            }
        }
    }
}