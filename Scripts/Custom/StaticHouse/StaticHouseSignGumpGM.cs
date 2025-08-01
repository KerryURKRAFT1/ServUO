using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.StaticHouse;
using Server.Items;
using System.Collections.Generic;

namespace Server.StaticHouse
{
    public class StaticHouseSignGumpGM : Gump
    {
        private StaticHouseSign m_Sign;
        private Mobile m_User;
        private int m_Page;

        // Variabili temporanee per editing multi-area (aggiunta/gestione aree multiple)
        private int m_EditAreaIndex = -1;

        public StaticHouseSignGumpGM(StaticHouseSign sign, Mobile from, int page = 0)
            : base(50, 50)
        {
            m_Sign = sign;
            m_User = from;
            m_Page = page;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            if (m_Page == 0)
                BuildMainPage();
            else if (m_Page == 1)
                BuildAreaGump();
            else if (m_Page == 2)
                BuildDoorPage();
        }

        private void BuildMainPage()
        {
            AddBackground(0, 0, 460, 460, 5054);
            AddBackground(10, 10, 440, 440, 3000);
            AddImage(150, -50, 100);
            AddLabel(150, 10, 1152, "Configure Static House");
            // Chiudi Gump (in alto a destra)
            AddButton(420, 15, 4017, 4018, 998, GumpButtonType.Reply, 0);

            AddLabel(20, 50, 0, "House Name:");
            AddTextEntry(120, 48, 200, 20, 0, 0, m_Sign.HouseName ?? "");

            AddLabel(20, 80, 0, "Sale Price:");
            AddTextEntry(120, 78, 100, 20, 0, 1, m_Sign.SalePrice.ToString());

            AddLabel(20, 110, 0, "Rent/Week:");
            AddTextEntry(120, 108, 100, 20, 0, 2, m_Sign.RentPrice.ToString());

            // ------------------------
            // AREA MULTIPLA DELLA CASA
            // ------------------------
            AddLabel(20, 140, 0, "House Areas (rectangles):");
            AddButton(290, 140, 4011, 4012, 2000, GumpButtonType.Reply, 0); // Pulsante "Aree"
            AddLabel(325, 140, 0, "Manage Areas");

            // --- CAMPI PRINCIPALI ---
            int areaY = 170;
            AddCheck(20, areaY, 210, 211, m_Sign.ForSale, 20);
            AddLabel(50, areaY, 0, "For Sale");
            AddCheck(120, areaY, 210, 211, m_Sign.ForRent, 21);
            AddLabel(150, areaY, 0, "For Rent");

            areaY += 30;
            AddLabel(20, areaY, 0, "Required Karma:");
            AddTextEntry(120, areaY - 2, 100, 20, 0, 3, m_Sign.RequiredKarma.ToString());

            areaY += 30;
            AddLabel(20, areaY, 0, "Required Fame:");
            AddTextEntry(120, areaY - 2, 100, 20, 0, 4, m_Sign.RequiredFame.ToString());

            areaY += 30;
            AddLabel(20, areaY, 0, "Owner:");
            AddLabel(120, areaY, 33, m_Sign.Owner != null ? m_Sign.Owner.Name : "None");

            int extraY = areaY + 20;
            if (m_Sign.Owner != null)
            {
                AddLabel(20, extraY, 0, "Owner Serial:");
                AddLabel(120, extraY, 33, m_Sign.Owner.Serial.ToString());
                extraY += 20;

                AddLabel(20, extraY, 0, "Account:");
                AddLabel(120, extraY, 33, m_Sign.Owner.Account != null ? m_Sign.Owner.Account.ToString() : "N/A");
                extraY += 20;

                AddLabel(20, extraY, 0, "Last refresh:");
                AddLabel(120, extraY, 33, m_Sign.LastRefresh.ToString());
                extraY += 20;

                TimeSpan left = (m_Sign.LastRefresh + m_Sign.DecayPeriod) - DateTime.UtcNow;
                if (left < TimeSpan.Zero) left = TimeSpan.Zero;
                string expires = left.TotalDays > 0
                    ? string.Format("{0:F1} real days left", left.TotalDays)
                    : "EXPIRED!";
                AddLabel(20, extraY, 0, "Decay expires in:");
                AddLabel(120, extraY, 33, expires);
                extraY += 20;
            }

            // --- PULSANTI FINALI NEL GUMP (in basso) ---
            AddButton(50, 410, 247, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(90, 410, 0, "Save");

            // --- DOOR MANAGEMENT ---
            AddButton(300, 410, 4011, 4012, 100, GumpButtonType.Reply, 0);
            AddLabel(340, 410, 0, "Doors");

            // --- VECCHIO SISTEMA: UN SOLO RECT (SOLO COME BACKUP, NON ATTIVI!) ---
            /*
            AddLabel(20, 140, 0, "Area X:");
            AddTextEntry(120, 138, 40, 20, 0, 3, m_Sign.HouseArea.Start.X.ToString());
            AddLabel(180, 140, 0, "Y:");
            AddTextEntry(230, 138, 40, 20, 0, 4, m_Sign.HouseArea.Start.Y.ToString());
            AddLabel(20, 170, 0, "Width:");
            AddTextEntry(120, 168, 40, 20, 0, 5, m_Sign.HouseArea.Width.ToString());
            AddLabel(180, 170, 0, "Height:");
            AddTextEntry(230, 168, 40, 20, 0, 6, m_Sign.HouseArea.Height.ToString());
            */
        }

        // Nuovo gump per gestione aree
        private void BuildAreaGump()
        {
            AddBackground(0, 0, 460, 460, 5054);
            AddBackground(10, 10, 440, 440, 3000);
            AddImage(150, -50, 100);
            AddLabel(150, 10, 1152, "Manage House Areas");
            AddButton(420, 15, 4017, 4018, 998, GumpButtonType.Reply, 0);

            int areaY = 60;
            AddLabel(20, areaY, 0, "Defined Areas:");

            int idx = 0;
            foreach (var rect in m_Sign.HouseAreas)
            {
                AddLabel(40, areaY + 30 + idx * 25, 0, $"[{idx}] X:{rect.Start.X} Y:{rect.Start.Y} W:{rect.Width} H:{rect.Height}");
                AddButton(320, areaY + 30 + idx * 25, 4014, 4015, 3000 + idx, GumpButtonType.Reply, 0); // Remove
                AddLabel(370, areaY + 30 + idx * 25, 0, "Remove");
                idx++;
            }
            if (m_Sign.HouseAreas.Count == 0)
            {
                AddLabel(40, areaY + 30, 33, "No areas defined!");
            }

            // Campi per aggiungere nuova area
            int addY = areaY + 60 + Math.Max(0, m_Sign.HouseAreas.Count) * 25;
            AddLabel(20, addY, 0, "Add Area:");
            AddLabel(100, addY, 0, "X:");
            AddTextEntry(120, addY - 2, 40, 20, 0, 10, "");
            AddLabel(170, addY, 0, "Y:");
            AddTextEntry(190, addY - 2, 40, 20, 0, 11, "");
            AddLabel(240, addY, 0, "W:");
            AddTextEntry(260, addY - 2, 40, 20, 0, 12, "");
            AddLabel(310, addY, 0, "H:");
            AddTextEntry(330, addY - 2, 40, 20, 0, 13, "");
            AddButton(380, addY , 2460, 2461, 4000, GumpButtonType.Reply, 0); // Add button Area


            //AddLabel(420, addY, 0, "OKAY Add");

            // Bottone per tornare indietro
            AddButton(50, 410, 4014, 4015, 1999, GumpButtonType.Reply, 0);
            AddLabel(120, 410, 0, "Back");
        }

        private void BuildDoorPage()
        {
            AddPage(2);
            AddBackground(0, 0, 460, 460, 5054);
            AddBackground(10, 10, 440, 440, 3000);
            AddImage(150, -50, 100);
            AddLabel(180, 10, 1152, "House Doors");
            AddButton(420, 15, 4017, 4018, 998, GumpButtonType.Reply, 0);

            int yOffset = 20;

            AddButton(50, 50 + yOffset, 4011, 4012, 101, GumpButtonType.Reply, 0);
            AddLabel(90, 50 + yOffset, 0, "Select Door(s)");

            int y = 100 + yOffset;
            AddLabel(50, y - 20, 0, "Associated doors:");

            if (m_Sign.AssociatedDoors.Count == 0)
            {
                AddLabel(60, y, 33, "None");
            }
            else
            {
                for (int i = 0; i < m_Sign.AssociatedDoors.Count; i++)
                {
                    BaseDoor door = m_Sign.AssociatedDoors[i];
                    if (door != null && !door.Deleted)
                    {
                        AddLabel(60, y, 0, string.Format("Serial: {0}", door.Serial));
                        AddButton(280, y, 4014, 4015, 200 + i, GumpButtonType.Reply, 0); // Remove btn
                        AddLabel(320, y, 0, "Remove");
                        y += 25;
                    }
                }
            }

            AddButton(50, 410, 4014, 4015, 102, GumpButtonType.Reply, 0);
            AddLabel(120, 410, 0, "Back");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 998)
                return;

            // MAIN PAGE: Pulsante Aree (apre nuovo gump)
            if (m_Page == 0 && info.ButtonID == 2000)
            {
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 1));
                return;
            }

            // MAIN PAGE: Save
            if (m_Page == 0 && info.ButtonID == 1)
            {
                string name = info.GetTextEntry(0) != null ? info.GetTextEntry(0).Text.Trim() : "";
                int salePrice = Utility.ToInt32(info.GetTextEntry(1) != null ? info.GetTextEntry(1).Text.Trim() : "0");
                int rentPrice = Utility.ToInt32(info.GetTextEntry(2) != null ? info.GetTextEntry(2).Text.Trim() : "0");
                int requiredKarma = Utility.ToInt32(info.GetTextEntry(3) != null ? info.GetTextEntry(3).Text.Trim() : "0");
                int requiredFame = Utility.ToInt32(info.GetTextEntry(4) != null ? info.GetTextEntry(4).Text.Trim() : "0");
                bool forSale = info.IsSwitched(20);
                bool forRent = info.IsSwitched(21);

                m_Sign.DefaultSettings.HouseName = name;
                m_Sign.DefaultSettings.SalePrice = salePrice;
                m_Sign.DefaultSettings.RentPrice = rentPrice;
                m_Sign.DefaultSettings.ForSale = forSale;
                m_Sign.DefaultSettings.ForRent = forRent;
                m_Sign.DefaultSettings.RequiredKarma = requiredKarma;
                m_Sign.DefaultSettings.RequiredFame = requiredFame;

                m_Sign.HouseName = name;
                m_Sign.SalePrice = salePrice;
                m_Sign.RentPrice = rentPrice;
                m_Sign.ForSale = forSale;
                m_Sign.ForRent = forRent;
                m_Sign.RequiredKarma = requiredKarma;
                m_Sign.RequiredFame = requiredFame;

                m_User.SendMessage("Settings saved.");
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 0));
                return;
            }

            // MAIN PAGE: Vai alla pagina porte
            if (m_Page == 0 && info.ButtonID == 100)
            {
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 2));
                return;
            }

            // AREA PAGE: Back
            if (m_Page == 1 && info.ButtonID == 1999)
            {
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 0));
                return;
            }

            // AREA PAGE: Add Area
            if (m_Page == 1 && info.ButtonID == 4000)
            {
                int areaX = Utility.ToInt32(info.GetTextEntry(10)?.Text.Trim() ?? "0");
                int areaY = Utility.ToInt32(info.GetTextEntry(11)?.Text.Trim() ?? "0");
                int areaW = Utility.ToInt32(info.GetTextEntry(12)?.Text.Trim() ?? "0");
                int areaH = Utility.ToInt32(info.GetTextEntry(13)?.Text.Trim() ?? "0");

                if (areaW > 0 && areaH > 0)
                {
                    m_Sign.HouseAreas.Add(new Rectangle2D(areaX, areaY, areaW, areaH));
                    m_User.SendMessage("Area added.");
                }
                else
                {
                    m_User.SendMessage("Invalid area dimensions.");
                }
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 1));
                return;
            }

            // AREA PAGE: Remove Area
            if (m_Page == 1 && info.ButtonID >= 3000 && info.ButtonID < 4000)
            {
                int idx = info.ButtonID - 3000;
                if (idx >= 0 && idx < m_Sign.HouseAreas.Count)
                {
                    m_Sign.HouseAreas.RemoveAt(idx);
                    m_User.SendMessage("Area removed.");
                }
                m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 1));
                return;
            }

            // DOOR PAGE
            if (m_Page == 2)
            {
                if (info.ButtonID == 101)
                {
                    m_User.SendMessage("Select a door to associate with this house.");
                    m_Sign.BeginAssociateDoor(m_User, this);
                }
                else if (info.ButtonID == 102)
                {
                    m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 0));
                }
                else if (info.ButtonID >= 200)
                {
                    int idx = info.ButtonID - 200;
                    if (idx >= 0 && idx < m_Sign.AssociatedDoors.Count)
                    {
                        m_Sign.AssociatedDoors.RemoveAt(idx);
                        m_User.SendMessage("Door removed from the house.");
                    }
                    m_User.SendGump(new StaticHouseSignGumpGM(m_Sign, m_User, 2));
                }
            }
        }
    }
}