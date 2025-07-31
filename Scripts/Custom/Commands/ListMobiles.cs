using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Gumps; // Aggiungi questa direttiva
using Server.Network; // Aggiungi questa direttiva

public class ListMobiles
{
    public static void Initialize()
    {
        CommandSystem.Register("ListMobiles", AccessLevel.GameMaster, new CommandEventHandler(ListMobiles_OnCommand));
    }

    [Usage("ListMobiles")]
    [Description("Lists all mobiles and provides teleport options.")]
    public static void ListMobiles_OnCommand(CommandEventArgs e)
    {
        Mobile from = e.Mobile;
        StringBuilder sb = new StringBuilder();

        foreach (Mobile mobile in World.Mobiles.Values)
        {
            sb.AppendLine(mobile.GetType().Name + " - " + mobile.Name + " at " + mobile.Location + ", " + mobile.Map);
            sb.AppendLine("[Go " + mobile.X + " " + mobile.Y + " " + mobile.Z + "]");
        }

        from.SendGump(new ListMobilesGump(from, sb.ToString()));
    }
}

public class ListMobilesGump : Gump
{
    private string m_List;

    public ListMobilesGump(Mobile from, string list) : base(50, 50)
    {
        from.CloseGump(typeof(ListMobilesGump));
        m_List = list;
        
        AddPage(0);

        AddBackground(0, 0, 400, 400, 5054);
        AddAlphaRegion(10, 10, 380, 380);

        AddLabel(20, 15, 1152, "List of Mobiles");
        AddHtml(20, 40, 360, 320, list.Replace("\n", "<br/>"), true, true);

        AddButton(20, 360, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
        AddLabel(55, 360, 1152, "Close");
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;
        from.SendMessage("Gump closed.");
    }
}
