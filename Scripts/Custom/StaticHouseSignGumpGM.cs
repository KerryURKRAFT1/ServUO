using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.StaticHouse;

namespace Server.StaticHouse 
{
    public class StaticHouseSignGumpGM : Gump
    {
        private StaticHouseSign m_Sign;
        private Mobile m_User;

        public StaticHouseSignGumpGM(StaticHouseSign sign, Mobile from)
            : base(50, 50)
        {
            m_Sign = sign;
            m_User = from;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 350, 400, 9200);

            AddButton(320, 10, 4017, 4018, 0, GumpButtonType.Reply, 0);
            AddLabel(120, 10, 1152, "Configura Casa Statica");

            AddLabel(20, 50, 0, "Nome Casa:");
            AddTextEntry(120, 48, 200, 20, 0, 0, m_Sign.HouseName != null ? m_Sign.HouseName : "");

            AddLabel(20, 80, 0, "Prezzo Vendita:");
            AddTextEntry(120, 78, 100, 20, 0, 1, m_Sign.SalePrice.ToString());

            AddLabel(20, 110, 0, "Affitto/Settimana:");
            AddTextEntry(120, 108, 100, 20, 0, 2, m_Sign.RentPrice.ToString());

            AddLabel(20, 140, 0, "Area X:");
            AddTextEntry(120, 138, 40, 20, 0, 3, m_Sign.HouseArea.Start.X.ToString());
            AddLabel(180, 140, 0, "Y:");
            AddTextEntry(210, 138, 40, 20, 0, 4, m_Sign.HouseArea.Start.Y.ToString());
            AddLabel(20, 170, 0, "Width:");
            AddTextEntry(120, 168, 40, 20, 0, 5, m_Sign.HouseArea.Width.ToString());
            AddLabel(180, 170, 0, "Height:");
            AddTextEntry(210, 168, 40, 20, 0, 6, m_Sign.HouseArea.Height.ToString());

            AddCheck(20, 200, 210, 211, m_Sign.ForSale, 10);
            AddLabel(50, 200, 0, "In vendita");
            AddCheck(120, 200, 210, 211, m_Sign.ForRent, 11);
            AddLabel(150, 200, 0, "In affitto");

            AddLabel(20, 240, 0, "Proprietario:");
            AddLabel(120, 240, 33, m_Sign.Owner != null ? m_Sign.Owner.Name : "Nessuno");

            int extraY = 260;
            if (m_Sign.Owner != null)
            {
                AddLabel(20, extraY, 0, "Serial Owner:");
                AddLabel(120, extraY, 33, m_Sign.Owner.Serial.ToString());
                extraY += 20;

                AddLabel(20, extraY, 0, "Account:");
                AddLabel(120, extraY, 33, m_Sign.Owner.Account != null ? m_Sign.Owner.Account.ToString() : "N/A");
                extraY += 20;

                AddLabel(20, extraY, 0, "Ultimo refresh:");
                AddLabel(120, extraY, 33, m_Sign.LastRefresh.ToString());
                extraY += 20;

                // Calcolo giorni rimasti con decimale (uniforme per tutti)
                TimeSpan left = (m_Sign.LastRefresh + m_Sign.DecayPeriod) - DateTime.UtcNow;
                if (left < TimeSpan.Zero) left = TimeSpan.Zero;
                string scadenza = left.TotalDays > 0 
                    ? string.Format("{0:F1} giorni reali rimasti", left.TotalDays) 
                    : "SCADUTA!";
                AddLabel(20, extraY, 0, "Scadenza decay:");
                AddLabel(120, extraY, 33, scadenza);
                extraY += 20;
            }

            AddButton(50, 340, 247, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(90, 340, 0, "Salva");

            AddButton(50, 370, 247, 248, 2, GumpButtonType.Reply, 0);
            AddLabel(90, 370, 0, "Abbina Porta");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 0)
                return;

            if (info.ButtonID == 1)
            {
                string nome = info.GetTextEntry(0) != null ? info.GetTextEntry(0).Text.Trim() : "";
                int prezzoVendita = Utility.ToInt32(info.GetTextEntry(1) != null ? info.GetTextEntry(1).Text.Trim() : "0");
                int prezzoAffitto = Utility.ToInt32(info.GetTextEntry(2) != null ? info.GetTextEntry(2).Text.Trim() : "0");
                int areaX = Utility.ToInt32(info.GetTextEntry(3) != null ? info.GetTextEntry(3).Text.Trim() : "0");
                int areaY = Utility.ToInt32(info.GetTextEntry(4) != null ? info.GetTextEntry(4).Text.Trim() : "0");
                int areaW = Utility.ToInt32(info.GetTextEntry(5) != null ? info.GetTextEntry(5).Text.Trim() : "0");
                int areaH = Utility.ToInt32(info.GetTextEntry(6) != null ? info.GetTextEntry(6).Text.Trim() : "0");

                m_Sign.HouseName = nome;
                m_Sign.SalePrice = prezzoVendita;
                m_Sign.RentPrice = prezzoAffitto;
                m_Sign.HouseArea = new Rectangle2D(areaX, areaY, areaW, areaH);
                m_Sign.ForSale = info.IsSwitched(10);
                m_Sign.ForRent = info.IsSwitched(11);

                m_User.SendMessage("Impostazioni salvate.");
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User));
            }
            else if (info.ButtonID == 2)
            {
                m_User.SendMessage("Seleziona una porta da abbinare a questa casa.");
                m_Sign.BeginAssociateDoor(m_User);
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User));
            }
        }
    }
}