using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom
{
    public class NpcFinderCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("NpcFinder", AccessLevel.GameMaster, new CommandEventHandler(NpcFinder_OnCommand));
        }

        [Usage("NpcFinder")]
        [Description("Apre un gump per cercare NPC per nome.")]
        public static void NpcFinder_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new NpcFinderGump(e.Mobile, "", null));
        }
    }

    public class NpcFinderGump : Gump
    {
        private string m_SearchText;
        private List<Mobile> m_Results;

        public NpcFinderGump(Mobile from, string searchText, List<Mobile> results)
            : base(50, 50)
        {
            m_SearchText = searchText;
            m_Results = results;

            AddPage(0);
            AddBackground(0, 0, 420, 350, 9270);
            AddLabel(130, 10, 1152, "Ricerca NPC per Nome");

            AddLabel(25, 45, 1152, "Nome NPC:");
            AddTextEntry(100, 45, 200, 20, 0, 0, searchText);

            AddButton(310, 42, 1209, 1210, 1, GumpButtonType.Reply, 0);
            AddLabel(340, 45, 0, "Cerca");

            int y = 80;
            if (results != null && results.Count > 0)
            {
                AddLabel(25, y, 1152, "Risultati:");
                y += 25;
                int index = 0;
                foreach (Mobile mob in results)
                {
                    if (y > 300) // Limitiamo la visualizzazione a non esondare il gump
                    {
                        AddLabel(25, y, 33, "...altri risultati non mostrati...");
                        break;
                    }
                    AddLabel(35, y, 0, string.Format("{0} ({1}) [{2},{3},{4} {5}]", mob.Name, mob.GetType().Name, mob.X, mob.Y, mob.Z, mob.Map));
                    AddButton(350, y, 4023, 4025, 100 + index, GumpButtonType.Reply, 0);
                    AddLabel(370, y, 0, "Vai");
                    y += 25;
                    index++;
                }
            }
            else if (results != null)
            {
                AddLabel(25, y, 33, "Nessun NPC trovato.");
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (info.ButtonID == 0)
                return;
            if (info.ButtonID == 1)
            {
                string searchText = info.GetTextEntry(0) != null ? info.GetTextEntry(0).Text.Trim() : "";
                List<Mobile> found = new List<Mobile>();
                if (searchText.Length > 0)
                {
                    foreach (Mobile mob in World.Mobiles.Values)
                    {
                        if (mob == null || mob.Deleted)
                            continue;
                        if (!mob.Player && mob.Name != null && mob.Name.ToLower().Contains(searchText.ToLower()))
                            found.Add(mob);
                    }
                }
                from.SendGump(new NpcFinderGump(from, searchText, found));
            }
            else if (info.ButtonID >= 100 && m_Results != null)
            {
                int index = info.ButtonID - 100;
                if (index >= 0 && index < m_Results.Count)
                {
                    Mobile mob = m_Results[index];
                    if (mob != null && !mob.Deleted)
                    {
                        from.Map = mob.Map;
                        from.Location = mob.Location;
                        from.SendMessage(0x44, "Teletrasportato su {0} ({1})", mob.Name, mob.GetType().Name);
                    }
                }
                from.SendGump(new NpcFinderGump(from, m_SearchText, m_Results));
            }
        }
    }
}