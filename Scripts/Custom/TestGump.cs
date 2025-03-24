using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

public class ViewGumpsCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("ViewGumps", AccessLevel.GameMaster, new CommandEventHandler(ViewGumps_OnCommand));
    }

    [Usage("ViewGumps")]
    [Description("Apre un Gump per visualizzare i Gump con ID specificati.")]
    public static void ViewGumps_OnCommand(CommandEventArgs e)
    {
        Mobile from = e.Mobile;
        from.SendGump(new ViewGumpsGump());
    }
}

public class ViewGumpsGump : Gump
{
    public ViewGumpsGump() : base(50, 50)
    {
        AddPage(0);
        AddBackground(0, 0, 400, 300, 9270); // Cambia lo sfondo a nero (ID di esempio)

        // Aggiungi i Gump con gli ID specificati e bordi di debug
        AddLabel(50, 30, 1152, "5507");
        AddImage(50, 50, 5507);
        AddLabel(100, 30, 1152, "5508");
        AddImage(100, 50, 5508);
        AddLabel(150, 30, 1152, "5509");
        AddImage(150, 50, 5509);
        AddLabel(200, 30, 1152, "5510");
        AddImage(200, 50, 5510);
        AddLabel(250, 30, 1152, "5511");
        AddImage(250, 50, 5511);

        AddLabel(300, 30, 1152, "5012");
        AddImage(300, 50, 5012);
        AddLabel(50, 100, 1152, "5516");
        AddImage(50, 120, 5516);
        AddLabel(100, 100, 1152, "5517");
        AddImage(100, 120, 5517);
        AddLabel(150, 100, 1152, "5518");
        AddImage(150, 120, 5518);
        AddLabel(200, 100, 1152, "5543");
        AddImage(200, 120, 5543);
        AddLabel(250, 100, 1152, "5544");
        AddImage(250, 120, 5544);

        AddButton(20, 260, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
        AddLabel(55, 260, 1152, "Close");
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;
        from.SendMessage("Gump closed.");
    }
}