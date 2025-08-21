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
        [Description("Apre un gump per cercare NPC per nome o tipo.")]
        public static void NpcFinder_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new NpcFinderGump(e.Mobile, "", false, false, 0));
        }
    }

    public class NpcFinderGump : Gump
    {
        private const int ItemsPerPage = 10;
        private string m_SearchText;
        private bool m_FilterHireable;
        private bool m_FilterEscortable;
        private int m_Page;
        private List<Mobile> m_Results;

        public NpcFinderGump(Mobile from, string searchText, bool filterHireable, bool filterEscortable, int page)
            : base(50, 50)
        {
            m_SearchText = searchText;
            m_FilterHireable = filterHireable;
            m_FilterEscortable = filterEscortable;
            m_Page = page;
            m_Results = new List<Mobile>();

            // Ricerca e filtri
            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob == null || mob.Deleted || mob.Player || mob.Name == null)
                    continue;
                if (m_SearchText != null && m_SearchText.Length > 0 && mob.Name.ToLower().IndexOf(m_SearchText.ToLower()) == -1)
                    continue;
                if (m_FilterHireable && !(mob is BaseHire))
                    continue;
                if (m_FilterEscortable && !(mob is BaseEscortable))
                    continue;
                m_Results.Add(mob);
            }

            int totalPages = (m_Results.Count + ItemsPerPage - 1) / ItemsPerPage;

            // Layout Gump
            AddPage(0);
            AddBackground(0, 0, 490, 440, 9200); // Sfondo chiaro classico
            AddLabel(120, 12, 0, "Ricerca NPC per Nome o Tipo");

            // Ricerca per nome
            AddLabel(20, 45, 0, "Nome NPC:");
            AddTextEntry(100, 45, 200, 20, 0, 0, m_SearchText);

            // Filtri
            AddCheck(320, 45, 210, 211, m_FilterHireable, 1);
            AddLabel(345, 45, 0, "Solo Assoldabili");
            AddCheck(320, 70, 210, 211, m_FilterEscortable, 2);
            AddLabel(345, 70, 0, "Solo Escortabili");

            // Pulsante ricerca
            AddButton(100, 75, 1209, 1210, 3, GumpButtonType.Reply, 0);
            AddLabel(140, 77, 0, "Cerca");

            // Elenco risultati
            int y = 110;
            int startIdx = m_Page * ItemsPerPage;
            int endIdx = m_Results.Count;
            if (endIdx > startIdx + ItemsPerPage)
                endIdx = startIdx + ItemsPerPage;

            if (m_Results.Count > 0)
            {
                AddLabel(20, y, 0, "Risultati: " + m_Results.Count.ToString() + "  Pagina " + (m_Page + 1).ToString() + "/" + (totalPages > 0 ? totalPages : 1).ToString());
                y += 25;
                for (int i = startIdx; i < endIdx; i++)
                {
                    Mobile mob = m_Results[i];
                    AddLabel(35, y, 0, mob.Name + " (" + mob.GetType().Name + ") [" + mob.X + "," + mob.Y + "," + mob.Z + " " + mob.Map + "]");
                    AddButton(410, y, 4023, 4025, 100 + i, GumpButtonType.Reply, 0);
                    AddLabel(435, y, 0, "Vai");
                    y += 25;
                }

                // Paginazione
                if (m_Page > 0)
                {
                    AddButton(180, 400, 4014, 4015, 4, GumpButtonType.Reply, 0); // Indietro
                    AddLabel(210, 403, 0, "Pagina Prec.");
                }
                if (m_Page < totalPages - 1)
                {
                    AddButton(290, 400, 4005, 4006, 5, GumpButtonType.Reply, 0); // Avanti
                    AddLabel(320, 403, 0, "Pagina Succ.");
                }
            }
            else
            {
                AddLabel(20, y, 33, "Nessun NPC trovato.");
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (info.ButtonID == 0)
                return;

            // Filtri
            bool filterHireable = info.IsSwitched(1);
            bool filterEscortable = info.IsSwitched(2);

            // Ricerca
            if (info.ButtonID == 3)
            {
                string searchText = "";
                TextRelay entry = info.GetTextEntry(0);
                if (entry != null)
                    searchText = entry.Text.Trim();
                from.SendGump(new NpcFinderGump(from, searchText, filterHireable, filterEscortable, 0));
            }
            // Indietro
            else if (info.ButtonID == 4)
            {
                from.SendGump(new NpcFinderGump(from, m_SearchText, m_FilterHireable, m_FilterEscortable, m_Page - 1));
            }
            // Avanti
            else if (info.ButtonID == 5)
            {
                from.SendGump(new NpcFinderGump(from, m_SearchText, m_FilterHireable, m_FilterEscortable, m_Page + 1));
            }
            // Pulsante "Vai"
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
                from.SendGump(new NpcFinderGump(from, m_SearchText, m_FilterHireable, m_FilterEscortable, m_Page));
            }
        }
    }
}