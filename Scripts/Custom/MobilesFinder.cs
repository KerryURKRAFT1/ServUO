using System;
using System.Collections;
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
    [Description("Apre una finestra per cercare i mob attivi.")]
    public static void MobilesFinder_OnCommand(CommandEventArgs e)
    {
        e.Mobile.SendGump(new MobilesFinderSearchGump(e.Mobile, ""));
    }
}

public class MobilesFinderSearchGump : Gump
{
    private string m_Search;

    public MobilesFinderSearchGump(Mobile from, string search) : base(100, 100)
    {
        m_Search = search;
        from.CloseGump(typeof(MobilesFinderSearchGump));

        AddPage(0);
        AddBackground(0, 0, 440, 200, 9270);
        AddAlphaRegion(15, 15, 410, 170);

        AddLabelC(220, 28, 1152, "MOBILE SEARCH");
        AddImageTiled(30, 48, 380, 2, 9107);

        AddLabel(30, 60, 1152, "Name, type, or map (partial):");
        AddTextEntry(30, 85, 320, 32, 1152, 0, search == null ? "" : search);

        AddButton(360, 85, 4005, 4007, 1, GumpButtonType.Reply, 0);
        AddLabel(388, 89, 1152, "Cerca");

        AddButton(360, 142, 4017, 4019, 0, GumpButtonType.Reply, 0);
        AddLabel(388, 146, 1152, "Chiudi");

        AddLabel(30, 130, 2118, "Es: nightmare, ostard, felucca...");
    }

    public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;
        if (from == null)
            return;

        if (info.ButtonID == 1)
        {
            TextRelay entry = info.GetTextEntry(0);
            string search = (entry != null ? entry.Text.Trim() : "");
            if (search == null || search.Length == 0)
            {
                from.SendMessage(0x22, "Inserisci almeno una parola.");
                from.SendGump(new MobilesFinderSearchGump(from, search));
                return;
            }
            from.SendGump(new MobilesFinderResultsGump(from, search, 0));
        }
    }

    public void AddLabelC(int x, int y, int color, string text)
    {
        int width = (text != null ? text.Length * 8 : 0);
        AddLabel(x - (width / 2), y, color, text);
    }
}

public class MobilesFinderResultsGump : Gump
{
    private const int PerPage = 10;
    private string m_Filter;
    private int m_Page;
    private ArrayList m_Results;

    public MobilesFinderResultsGump(Mobile from, string filter, int page)
        : base(90, 90)
    {
        m_Filter = filter;
        m_Page = page;
        from.CloseGump(typeof(MobilesFinderResultsGump));

        m_Results = GetCurrentResults(filter);

        int totalPages = (m_Results.Count + PerPage - 1) / PerPage;
        int height = 90 + Math.Max(PerPage, (m_Results.Count - (m_Page * PerPage))) * 28;

        AddPage(0);
        AddBackground(0, 0, 670, height, 9270);
        AddAlphaRegion(18, 18, 634, height - 36);

        AddLabelC(335, 32, 1152, "RISULTATI PER \"" + m_Filter + "\"");
        AddImageTiled(30, 50, 630, 2, 9107);

        int start = m_Page * PerPage;
        int end = Math.Min(start + PerPage, m_Results.Count);
        int y = 60;

        // Intestazione colonne
        AddLabel(34, y, 1152, "Tipo");
        AddLabel(160, y, 1152, "Nome");
        AddLabel(320, y, 1152, "Loc");
        AddLabel(420, y, 1152, "Mappa");
        AddLabel(540, y, 1152, "Stato");
        y += 22;
        AddImageTiled(30, y, 630, 1, 9304);

        y += 5;
        bool even = false;
        int buttonId = 10;
        for (int i = start; i < end; i++, y += 28, buttonId++, even = !even)
        {
            Mobile mob = (Mobile)m_Results[i];
            string mobType = mob.GetType().Name;
            string mobName = mob.Name != null ? mob.Name : "(no name)";
            string mapName = (mob.Map != null ? mob.Map.Name : "(null)");
            string loc = mob.Location.X + "," + mob.Location.Y + "," + mob.Location.Z;
            string status = mob.Hidden ? "Hidden" : (mob.AccessLevel > AccessLevel.Player ? "Staff" : "Visible");

            if (even)
                AddImageTiled(30, y - 3, 630, 26, 2624);

            AddLabel(34, y, 1152, mobType);
            AddLabel(160, y, 1152, mobName);
            AddLabel(320, y, 1152, loc);
            AddLabel(420, y, 1152, mapName);
            AddLabel(540, y, 1152, status);

            AddButton(600, y, 9723, 9721, buttonId, GumpButtonType.Reply, 0);
            AddLabel(626, y, 65, "Go");
        }

        y += 5;
        AddImageTiled(30, y, 630, 2, 9107);

        // Navigazione
        int navY = y + 10;
        if (m_Page > 0)
        {
            AddButton(40, navY, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddLabel(74, navY + 4, 1152, "Precedente");
        }
        if (end < m_Results.Count)
        {
            AddButton(160, navY, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(194, navY + 4, 1152, "Prossima");
        }

        AddButton(590, navY, 4017, 4019, 0, GumpButtonType.Reply, 0);
        AddLabel(624, navY + 4, 1152, "Chiudi");
    }

    public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;
        if (from == null)
            return;

        int start = m_Page * PerPage;
        int end = Math.Min(start + PerPage, m_Results.Count);

        if (info.ButtonID == 1 && m_Page > 0)
        {
            from.SendGump(new MobilesFinderResultsGump(from, m_Filter, m_Page - 1));
            return;
        }
        if (info.ButtonID == 2 && end < m_Results.Count)
        {
            from.SendGump(new MobilesFinderResultsGump(from, m_Filter, m_Page + 1));
            return;
        }
        int goIndex = info.ButtonID - 10;
        if (goIndex >= 0 && goIndex < PerPage)
        {
            int mobIndex = m_Page * PerPage + goIndex;
            if (mobIndex < m_Results.Count)
            {
                Mobile target = (Mobile)m_Results[mobIndex];
                if (target != null && !target.Deleted)
                {
                    if (target.Map != null && target.Map != Map.Internal)
                    {
                        Point3D loc = target.Location;
                        Map map = target.Map;

                        if (target is BaseMount)
                            loc = new Point3D(loc.X + 1, loc.Y, loc.Z);

                        if (map.CanFit(loc, 16, false, false))
                        {
                            from.MoveToWorld(loc, map);
                            from.SendMessage("Teletrasportato su \"{0}\".", target.Name != null ? target.Name : target.GetType().Name);
                        }
                        else
                        {
                            from.SendMessage(33, "Locazione o mappa non valida.");
                        }
                    }
                    else
                    {
                        from.SendMessage(33, "Locazione o mappa non valida.");
                    }
                }
                else
                {
                    from.SendMessage(33, "Il mobile non esiste piu'.");
                }
            }
            from.SendGump(new MobilesFinderResultsGump(from, m_Filter, m_Page));
        }
    }

    // Ricerca compatibile, C# vecchio stile
    public static ArrayList GetCurrentResults(string filter)
    {
        ArrayList results = new ArrayList();
        if (filter == null)
            return results;
        string fil = filter.ToLower();
        foreach (Mobile mob in World.Mobiles.Values)
        {
            if (mob == null || mob.Deleted)
                continue;
            if (mob.Player)
                continue;

            bool match = false;

            // Cerca nel nome
            if (mob.Name != null && mob.Name.ToLower().IndexOf(fil) >= 0)
                match = true;

            // Cerca nel tipo
            else if (mob.GetType().Name.ToLower().IndexOf(fil) >= 0)
                match = true;

            // Cerca in "a <tipo>" (es: "a nightmare")
            else if (("a " + mob.GetType().Name.ToLower()).IndexOf(fil) >= 0)
                match = true;

            // Cerca nella mappa
            else if (mob.Map != null && mob.Map.Name != null && mob.Map.Name.ToLower().IndexOf(fil) >= 0)
                match = true;

            if (match)
                results.Add(mob);
        }
        return results;
    }

    public void AddLabelC(int x, int y, int color, string text)
    {
        int width = (text != null ? text.Length * 8 : 0);
        AddLabel(x - (width / 2), y, color, text);
    }
}