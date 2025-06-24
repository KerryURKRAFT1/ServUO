using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Gumps;

public class MobilesFinder
{
    public static void Initialize()
    {
        CommandSystem.Register("MobilesFinder", AccessLevel.GameMaster, new CommandEventHandler(MobilesFinder_OnCommand));
    }

    [Usage("MobilesFinder")]
    [Description("Apre il gump per cercare mobiles attivi.")]
    public static void MobilesFinder_OnCommand(CommandEventArgs e)
    {
        e.Mobile.SendGump(new MobilesFinderSearchGump(e.Mobile, ""));
    }
}

public class MobilesFinderSearchGump : Gump
{
    private readonly string m_Search;

    public MobilesFinderSearchGump(Mobile from, string search) : base(100, 100)
    {
        m_Search = search;
        from.CloseGump(typeof(MobilesFinderSearchGump));

        AddPage(0);
        AddBackground(0, 0, 400, 150, 5054);
        AddAlphaRegion(10, 10, 380, 130);

        AddLabel(20, 20, 1152, "Ricerca Mobiles (per nome o tipo):");
        AddTextEntry(20, 50, 320, 32, 0, 0, search ?? "");

        AddButton(20, 100, 4023, 4025, 1, GumpButtonType.Reply, 0); // Cerca
        AddLabel(60, 100, 1152, "Cerca");

        AddButton(300, 100, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0); // Chiudi
        AddLabel(335, 100, 1152, "Chiudi");
    }

    public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;
        if (from == null)
            return;

        if (info.ButtonID == 1)
        {
            string search = info.GetTextEntry(0) != null ? info.GetTextEntry(0).Text.Trim() : "";
            if (string.IsNullOrEmpty(search))
            {
                from.SendMessage("Inserisci almeno una parola per la ricerca.");
                from.SendGump(new MobilesFinderSearchGump(from, search));
                return;
            }
            List<Mobile> results = new List<Mobile>();
            string filtro = search.ToLower();
            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob == null || mob.Deleted)
                    continue;
                string mobName = (mob.Name ?? "").ToLower();
                string mobType = mob.GetType().Name.ToLower();
                if (mobName.IndexOf(filtro) >= 0 || mobType.IndexOf(filtro) >= 0)
                    results.Add(mob);
            }
            if (results.Count == 0)
            {
                from.SendMessage("Nessun mobile trovato per '{0}'.", search);
                from.SendGump(new MobilesFinderSearchGump(from, search));
                return;
            }
            from.SendGump(new MobilesFinderResultsGump(from, results, search, 0));
        }
        // ButtonID 0 = Chiudi, non fa nulla
    }
}

public class MobilesFinderResultsGump : Gump
{
    private const int PerPage = 10;
    private readonly List<Mobile> m_List;
    private readonly string m_Filter;
    private readonly int m_Page;

    public MobilesFinderResultsGump(Mobile from, List<Mobile> list, string filter, int page) : base(100, 100)
    {
        m_List = list;
        m_Filter = filter;
        m_Page = page;
        from.CloseGump(typeof(MobilesFinderResultsGump));

        int totalPages = (int)Math.Ceiling((double)m_List.Count / PerPage);
        AddPage(0);
        AddBackground(0, 0, 560, 55 + PerPage * 30 + 50, 5054);
        AddAlphaRegion(10, 10, 540, 35 + PerPage * 30);

        AddLabel(20, 15, 1152, string.Format("Risultati per \"{0}\" (pagina {1} di {2})", filter, m_Page + 1, totalPages));

        int start = m_Page * PerPage;
        int end = Math.Min(start + PerPage, m_List.Count);
        int y = 45;

        if (m_List.Count == 0)
        {
            AddLabel(20, y, 33, "Nessun mobile trovato.");
        }
        else
        {
            int buttonId = 10;
            for (int i = start; i < end; i++, y += 30, buttonId++)
            {
                Mobile mob = m_List[i];
                string mobType = mob.GetType().Name;
                string mobName = mob.Name ?? "(no name)";
                string mapName = mob.Map != null ? mob.Map.Name : "(null)";
                string loc = string.Format("{0},{1},{2}", mob.Location.X, mob.Location.Y, mob.Location.Z);

                AddLabel(20, y, 0, string.Format("{0} - \"{1}\"", mobType, mobName));
                AddLabel(280, y, 0, string.Format("@ {0} [{1}]", loc, mapName));
                AddButton(490, y, 0x15E1, 0x15E5, buttonId, GumpButtonType.Reply, 0);
                AddLabel(520, y, 61, "Go");
            }
        }

        // Navigazione
        if (m_Page > 0)
            AddButton(20, 35 + PerPage * 30, 4014, 4016, 1, GumpButtonType.Reply, 0); // Prev
        if (end < m_List.Count)
            AddButton(100, 35 + PerPage * 30, 4005, 4007, 2, GumpButtonType.Reply, 0); // Next

        AddButton(420, 35 + PerPage * 30, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0); // Chiudi
        AddLabel(455, 35 + PerPage * 30, 1152, "Chiudi");
    }

    public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;
        if (from == null)
            return;

        int start = m_Page * PerPage;
        int end = Math.Min(start + PerPage, m_List.Count);

        // Navigazione
        if (info.ButtonID == 1 && m_Page > 0)
        {
            from.SendGump(new MobilesFinderResultsGump(from, m_List, m_Filter, m_Page - 1));
            return;
        }
        if (info.ButtonID == 2 && end < m_List.Count)
        {
            from.SendGump(new MobilesFinderResultsGump(from, m_List, m_Filter, m_Page + 1));
            return;
        }
        // Go buttons
        int goIndex = info.ButtonID - 10;
        if (goIndex >= 0 && goIndex < PerPage)
        {
            int mobIndex = m_Page * PerPage + goIndex;
            if (mobIndex < m_List.Count)
            {
                Mobile target = m_List[mobIndex];
                if (target != null && !target.Deleted)
                {
                    from.MoveToWorld(target.Location, target.Map);
                    from.SendMessage("Teletrasportato a \"{0}\".", target.Name ?? target.GetType().Name);
                }
            }
            // Rimostra il gump dopo il go
            from.SendGump(new MobilesFinderResultsGump(from, m_List, m_Filter, m_Page));
        }
        // ButtonID 0 = Chiudi, non fa nulla
    }
}