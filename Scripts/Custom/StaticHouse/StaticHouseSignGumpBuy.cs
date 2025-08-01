using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
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

            AddLabel(80, 10, 1152, "Town House");

            AddLabel(20, 40, 0, "Name:");
            AddLabel(90, 40, 33, m_Sign.HouseName != null ? m_Sign.HouseName : "N/A");

            // Show required title (Karma/Fame)
            string requiredTitle = StaticHouseSign.GetTitleFromKarmaFame(m_Sign.RequiredKarma, m_Sign.RequiredFame);
            AddLabel(20, 65, 0, "Required Title:");
            AddLabel(150, 65, 88, requiredTitle);

            int y = 90;

            if (m_Sign.ForSale)
            {
                AddLabel(20, y, 0, "Sale Price:");
                AddLabel(150, y, 33, m_Sign.SalePrice.ToString() + " gp");
                AddButton(50, y + 55, 247, 248, 1, GumpButtonType.Reply, 0); // Buy
                AddLabel(150, y + 55, 0, "Buy");
            }
            else if (m_Sign.ForRent)
            {
                AddLabel(20, y, 0, "Weekly Rent:");
                AddLabel(150, y, 33, m_Sign.RentPrice.ToString() + " gp");
                AddButton(50, y + 55, 247, 248, 2, GumpButtonType.Reply, 0); // Rent
                AddLabel(90, y + 55, 0, "Rent");
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 0)
                return; // Close on X

            // --- BUY HOUSE ---
            if (info.ButtonID == 1 && m_Sign.ForSale && m_Sign.Owner == null)
            {
                // --- CHECK DOORS ---
                if (m_Sign.AssociatedDoors == null || m_Sign.AssociatedDoors.Count == 0)
                {
                    m_User.SendMessage(33, "Error: this house has no associated doors. Contact a GM.");
                    return;
                }
                // --- CHECK KARMA/FAME/TITLE ---
                if (m_User.Karma < m_Sign.RequiredKarma || m_User.Fame < m_Sign.RequiredFame)
                {
                    m_User.SendMessage(33, "You do not have enough title (karma/fame) to purchase this house.");
                    return;
                }
                // --- CHECK ACCOUNT HOUSE ---
                if (BaseHouse.HasAccountHouse(m_User))
                {
                    m_User.SendMessage(33, "You cannot own a static house if you already own a house.");
                    return;
                }

                bool paid = false;

                if (m_User.Backpack != null)
                {
                    if (m_User.Backpack.ConsumeTotal(typeof(Gold), m_Sign.SalePrice))
                        paid = true;
                    else
                    {
                        foreach (Item item in m_User.Backpack.Items)
                        {
                            BankCheck check = item as BankCheck;
                            if (check != null && check.Worth >= m_Sign.SalePrice)
                            {
                                check.Delete();
                                paid = true;
                                break;
                            }
                        }
                    }
                }

                if (paid)
                {
                    m_Sign.Owner = m_User;
                    m_Sign.ForSale = false;
                    m_Sign.RefreshDecay();

                    // --- Generate keys and associate doors ---
                    m_Sign.AssignKeysToOwner(m_User);

                    // (Optional) Change house name
                    m_Sign.HouseName = "House of " + m_User.Name;

                    m_User.SendMessage("You bought the house! The keys are in your backpack and bank.");
                }
                else
                {
                    m_User.SendMessage("You don't have enough gold or checks in your backpack.");
                }
            }
            // --- RENT HOUSE ---
            else if (info.ButtonID == 2 && m_Sign.ForRent && m_Sign.Owner == null)
            {
                // --- CHECK KARMA/FAME/TITLE also for rent if desired ---
                if (m_User.Karma < m_Sign.RequiredKarma || m_User.Fame < m_Sign.RequiredFame)
                {
                    m_User.SendMessage(33, "You do not have enough title (karma/fame) to rent this house.");
                    return;
                }
                // --- CHECK ACCOUNT HOUSE ---
                if (BaseHouse.HasAccountHouse(m_User))
                {
                    m_User.SendMessage(33, "You cannot own a static house if you already own a house.");
                    return;
                }

                bool paid = false;

                if (m_User.Backpack != null)
                {
                    if (m_User.Backpack.ConsumeTotal(typeof(Gold), m_Sign.RentPrice))
                        paid = true;
                    else
                    {
                        foreach (Item item in m_User.Backpack.Items)
                        {
                            BankCheck check = item as BankCheck;
                            if (check != null && check.Worth >= m_Sign.RentPrice)
                            {
                                check.Delete();
                                paid = true;
                                break;
                            }
                        }
                    }
                }

                if (paid)
                {
                    m_Sign.Owner = m_User;
                    m_Sign.ForRent = false;
                    m_Sign.RefreshDecay();

                    // (Only if you want to give keys to renters too)
                    // m_Sign.AssignKeysToOwner(m_User);

                    m_User.SendMessage("You have rented the house for one week!");
                }
                else
                {
                    m_User.SendMessage("You don't have enough gold or checks in your backpack.");
                }
            }
        }
    }
}