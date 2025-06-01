using System;
using Server;
using Server.Gumps;
using Custom;
using Server.Mobiles;

namespace Custom
{
    public class StaticHouseSignGumpOwner : Gump
    {
        private StaticHouseSign m_Sign;
        private Mobile m_User;

        public StaticHouseSignGumpOwner(StaticHouseSign sign, Mobile from)
            : base(100, 100)
        {
            m_Sign = sign;
            m_User = from;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 320, 180, 9270);
            AddLabel(80, 10, 1152, "Gestione Casa Cittadina");

            AddLabel(20, 40, 0, "Nome:");
            AddLabel(90, 40, 33, m_Sign.HouseName ?? "N/A");

            AddLabel(20, 65, 0, "Proprietario:");
            AddLabel(120, 65, 33, m_User.Name);

            AddLabel(20, 90, 0, "Tempo rimanente:");
            TimeSpan left = (m_Sign.LastRefresh + m_Sign.DecayPeriod) - DateTime.UtcNow;
            if (left < TimeSpan.Zero) left = TimeSpan.Zero;
            AddLabel(150, 90, 33, left.Days + " giorni");

            AddButton(50, 130, 247, 248, 1, GumpButtonType.Reply, 0); // Rinnova
            AddLabel(90, 130, 0, "Rinnova (refresh)");

            AddButton(180, 130, 4020, 4021, 2, GumpButtonType.Reply, 0); // Abbandona
            AddLabel(220, 130, 0, "Abbandona casa");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 1) // Rinnova
            {
                m_Sign.RefreshDecay();
                m_User.SendMessage("Hai rinnovato la casa. Il timer di decay è stato resettato.");
            }
            else if (info.ButtonID == 2) // Abbandona
            {
                m_Sign.Owner = null;
                m_Sign.ForSale = true;
                m_User.SendMessage("Hai abbandonato la casa, ora è tornata disponibile.");
            }
        }
    }
}