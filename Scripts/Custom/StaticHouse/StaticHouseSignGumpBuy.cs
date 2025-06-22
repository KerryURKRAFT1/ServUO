using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.StaticHouse;

namespace Server.StaticHouse 
{
    public class StaticHouseSignGumpBuy : Gump
    {
        private StaticHouseSign m_Sign;
        private Mobile m_User;

        public StaticHouseSignGumpBuy(StaticHouseSign sign, Mobile from)
            : base(100, 100)
        {
            m_Sign = sign;
            m_User = from;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 320, 220, 9200);

            AddButton(285, 10, 4017, 4018, 0, GumpButtonType.Reply, 0);

            AddLabel(80, 10, 1152, "Casa Cittadina");

            AddLabel(20, 40, 0, "Nome:");
            AddLabel(90, 40, 33, m_Sign.HouseName != null ? m_Sign.HouseName : "N/A");

            // Mostra il titolo richiesto (Karma/Fama)
            string titolo = StaticHouseSign.GetTitleFromKarmaFame(m_Sign.RequiredKarma, m_Sign.RequiredFame);
            AddLabel(20, 65, 0, "Titolo richiesto:");
            AddLabel(150, 65, 88, titolo);

            int y = 90;

            if (m_Sign.ForSale)
            {
                AddLabel(20, y, 0, "Prezzo di vendita:");
                AddLabel(150, y, 33, m_Sign.SalePrice.ToString() + " gp");
                AddButton(50, y + 55, 247, 248, 1, GumpButtonType.Reply, 0); // Compra
                AddLabel(90, y + 55, 0, "Compra");
            }
            else if (m_Sign.ForRent)
            {
                AddLabel(20, y, 0, "Affitto settimanale:");
                AddLabel(150, y, 33, m_Sign.RentPrice.ToString() + " gp");
                AddButton(50, y + 55, 247, 248, 2, GumpButtonType.Reply, 0); // Affitta
                AddLabel(90, y + 55, 0, "Affitta");
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 0)
                return; // premi X: chiudi

            // --- ACQUISTO CASA ---
            if (info.ButtonID == 1 && m_Sign.ForSale && m_Sign.Owner == null)
            {
                // --- CHECK KARMA/FAMA/TITOLO ---
                if (m_User.Karma < m_Sign.RequiredKarma || m_User.Fame < m_Sign.RequiredFame)
                {
                    m_User.SendMessage(33, "Non hai il titolo sufficiente (karma/fama) per acquistare questa casa.");
                    return;
                }

                bool pagato = false;

                if (m_User.Backpack != null)
                {
                    if (m_User.Backpack.ConsumeTotal(typeof(Gold), m_Sign.SalePrice))
                        pagato = true;
                    else
                    {
                        foreach (Item item in m_User.Backpack.Items)
                        {
                            BankCheck check = item as BankCheck;
                            if (check != null && check.Worth >= m_Sign.SalePrice)
                            {
                                check.Delete();
                                pagato = true;
                                break;
                            }
                        }
                    }
                }

                if (pagato)
                {
                    m_Sign.Owner = m_User;
                    m_Sign.ForSale = false;
                    m_Sign.RefreshDecay();

                    // --- Genera chiavi e abbina porte ---
                    m_Sign.AssignKeysToOwner(m_User);

                    // (Opzionale) Cambia nome casa
                    m_Sign.HouseName = "Casa di " + m_User.Name;

                    m_User.SendMessage("Hai comprato la casa! Le chiavi sono nel tuo zaino e banca.");
                }
                else
                {
                    m_User.SendMessage("Non hai abbastanza oro o check nello zaino.");
                }
            }
            // --- AFFITTO CASA ---
            else if (info.ButtonID == 2 && m_Sign.ForRent && m_Sign.Owner == null)
            {
                // --- CHECK KARMA/FAMA/TITOLO anche per affitto se desiderato ---
                if (m_User.Karma < m_Sign.RequiredKarma || m_User.Fame < m_Sign.RequiredFame)
                {
                    m_User.SendMessage(33, "Non hai il titolo sufficiente (karma/fama) per affittare questa casa.");
                    return;
                }

                bool pagato = false;

                if (m_User.Backpack != null)
                {
                    if (m_User.Backpack.ConsumeTotal(typeof(Gold), m_Sign.RentPrice))
                        pagato = true;
                    else
                    {
                        foreach (Item item in m_User.Backpack.Items)
                        {
                            BankCheck check = item as BankCheck;
                            if (check != null && check.Worth >= m_Sign.RentPrice)
                            {
                                check.Delete();
                                pagato = true;
                                break;
                            }
                        }
                    }
                }

                if (pagato)
                {
                    m_Sign.Owner = m_User;
                    m_Sign.ForRent = false;
                    m_Sign.RefreshDecay();

                    // (Solo se vuoi dare le chiavi anche agli affittuari)
                    // m_Sign.AssignKeysToOwner(m_User);

                    m_User.SendMessage("Hai affittato la casa per una settimana!");
                }
                else
                {
                    m_User.SendMessage("Non hai abbastanza oro o check nello zaino.");
                }
            }
        }
    }
}