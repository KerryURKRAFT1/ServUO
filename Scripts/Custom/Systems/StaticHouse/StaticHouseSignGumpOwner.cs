using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.StaticHouse;
using Server.Targeting;
using Server.Prompts;
using Server.Items;
using System.Collections.Generic;

namespace Server.StaticHouse
{
    public class StaticHouseSignGumpOwner : Gump
    {

        private StaticHouseSign m_Sign;
        private Mobile m_User;
        private int m_Page;

        public StaticHouseSignGumpOwner(StaticHouseSign sign, Mobile from, int page = 0)
            : base(100, 100)
        {
            m_Sign = sign;
            m_User = from;
            m_Page = page;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            switch (m_Page)
            {
                case 0:
                    BuildMainPage();
                    break;
                case 1:
                    BuildFriendsPage();
                    break;
                case 2:
                    BuildOptionPage();
                    break;
            }
        }

        private void BuildMainPage()
        {
            AddPage(0);

            AddBackground(0, 0, 460, 460, 5054);
            AddBackground(10, 10, 440, 440, 3000);
            AddImage(150, -50, 100);
            AddLabel(150, 0, 0, "Town House Management");
            AddButton(420, 15, 4017, 4018, 998, GumpButtonType.Reply, 0);

            int tabYOffset = 30;
            int yOffset = 60;

            AddButton(40, 50 + tabYOffset, 4005, 4007, 10, GumpButtonType.Reply, 0); // TAB INFO
            AddLabel(40, 22 + tabYOffset, 0, "INFO");

            AddButton(160, 50 + tabYOffset, 4005, 4007, 11, GumpButtonType.Reply, 0); // TAB FRIENDS
            AddLabel(160, 22 + tabYOffset, 0, "FRIENDS");

            AddButton(280, 50 + tabYOffset, 4005, 4007, 12, GumpButtonType.Reply, 0); // TAB OPTION
            AddLabel(280, 22 + tabYOffset, 0, "OPTION");

            AddLabel(20, 80 + yOffset, 0, "House's name:");
            AddLabel(150, 80 + yOffset, 0, !string.IsNullOrEmpty(m_Sign.HouseName) ? m_Sign.HouseName : "No one");

            AddLabel(20, 105 + yOffset, 0, "Owner:");
            AddLabel(150, 105 + yOffset, 0, m_Sign.Owner != null ? m_Sign.Owner.Name : "No one");

            AddLabel(20, 130 + yOffset, 0, "Time remaining:");
            TimeSpan left = (m_Sign.LastRefresh + m_Sign.DecayPeriod) - DateTime.UtcNow;
            if (left < TimeSpan.Zero) left = TimeSpan.Zero;
            AddLabel(150, 130 + yOffset, 0, string.Format("{0:F1} days", left.TotalDays));

            AddButton(20, 170 + yOffset, 247, 248, 20, GumpButtonType.Reply, 0); // Refresh
            AddLabel(95, 170 + yOffset, 0, "Refresh");

            //AddButton(280, 170 + yOffset, 4020, 4021, 21, GumpButtonType.Reply, 0); // Abandon
            //AddLabel(320, 170 + yOffset, 0, "Abandon house");
        }

        private void BuildFriendsPage()
        {
            AddPage(1);

            AddBackground(0, 0, 460, 460, 5054);
            AddBackground(10, 10, 440, 440, 3000);
            AddImage(150, -50, 100);
            AddLabel(150, 0, 0, "Town House Management");
            AddButton(420, 15, 4017, 4018, 998, GumpButtonType.Reply, 0);

            int tabYOffset = 30;
            int yOffset = 60;

            AddButton(40, 50 + tabYOffset, 4005, 4007, 10, GumpButtonType.Reply, 0); // TAB INFO
            AddLabel(40, 22 + tabYOffset, 0, "INFO");

            AddButton(160, 50 + tabYOffset, 4005, 4007, 11, GumpButtonType.Reply, 0); // TAB FRIENDS
            AddLabel(160, 22 + tabYOffset, 0, "FRIENDS");

            AddButton(280, 50 + tabYOffset, 4005, 4007, 12, GumpButtonType.Reply, 0); // TAB OPTION
            AddLabel(280, 22 + tabYOffset, 0, "OPTION");

            AddLabel(80, 70 + yOffset, 0, "View Co-Owner List");
            AddButton(30, 70 + yOffset, 4005, 4007, 110, GumpButtonType.Reply, 0);

            AddLabel(80, 100 + yOffset, 0, "Add a Co-Owner");
            AddButton(30, 100 + yOffset, 4005, 4007, 111, GumpButtonType.Reply, 0);

            AddLabel(80, 130 + yOffset, 0, "Remove a Co-Owner");
            AddButton(30, 130 + yOffset, 4005, 4007, 112, GumpButtonType.Reply, 0);

            AddLabel(300, 70 + yOffset, 0, "Add a Friend");
            AddButton(260, 70 + yOffset, 4005, 4007, 113, GumpButtonType.Reply, 0);

            AddLabel(300, 100 + yOffset, 0, "Remove a Friend");
            AddButton(260, 100 + yOffset, 4005, 4007, 114, GumpButtonType.Reply, 0);

            AddLabel(300, 130 + yOffset, 0, "View Friend List");
            AddButton(260, 130 + yOffset, 4005, 4007, 115, GumpButtonType.Reply, 0);

            AddLabel(300, 160 + yOffset, 0, "Ban a Player");
            AddButton(260, 160 + yOffset, 4005, 4007, 116, GumpButtonType.Reply, 0);

            AddLabel(300, 190 + yOffset, 0, "View Ban List");
            AddButton(260, 190 + yOffset, 4005, 4007, 117, GumpButtonType.Reply, 0);
        }

        private void BuildOptionPage()
        {

            //AddPage(2);

            AddBackground(0, 0, 460, 460, 5054);
            AddBackground(10, 10, 440, 440, 3000);
            AddImage(150, -50, 100);
            AddLabel(150, 0, 0, "Town House Management");
            AddButton(420, 15, 4017, 4018, 998, GumpButtonType.Reply, 0);


            int tabYOffset = 30;
            int yOffset = 60;

            AddButton(40, 50 + tabYOffset, 4005, 4007, 10, GumpButtonType.Reply, 0); // TAB INFO
            AddLabel(40, 22 + tabYOffset, 0, "INFO");

            AddButton(160, 50 + tabYOffset, 4005, 4007, 11, GumpButtonType.Reply, 0); // TAB FRIENDS
            AddLabel(160, 22 + tabYOffset, 0, "FRIENDS");

            AddButton(280, 50 + tabYOffset, 4005, 4007, 12, GumpButtonType.Reply, 0); // TAB OPTION
            AddLabel(280, 22 + tabYOffset, 0, "OPTION");

            AddButton(40, 150, 4005, 4007, 210, GumpButtonType.Reply, 0);
            AddLabel(80, 150, 0, "Transfer ownership of the house");

            AddButton(40, 200, 4005, 4007, 211, GumpButtonType.Reply, 0);
            AddLabel(80, 200, 0, "Change this house's name");

            AddButton(40, 250, 4005, 4007, 212, GumpButtonType.Reply, 0);
            AddLabel(80, 250, 0, "Abandon house and get money back");




        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {

            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 998)
                return;

            // TAB
            if (info.ButtonID == 10) { m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 0)); return; }
            if (info.ButtonID == 11) { m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1)); return; }
            if (info.ButtonID == 12) { m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 2)); return; }

            // MAIN PAGE
            if (m_Page == 0)
            {
                if (info.ButtonID == 20)
                {
                    m_Sign.RefreshDecay();
                    m_User.SendMessage("You have refreshed the house. The decay timer has been reset.");
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 0));
                    return;
                }
                // REMOVED ABADON BUTTON
                /*if (info.ButtonID == 21) 
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Solo il proprietario può abbandonare o vendere la casa.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 0));
                        return;
                    }
                    m_Sign.UnassignKeysFromDoors();
                    m_Sign.Owner = null;
                    m_Sign.ForSale = true;
                    m_Sign.HouseName = null;
                    m_User.SendMessage("You have abandoned the house, it is now available again.");
                    // NON RIAPRIRE IL GUMP QUI!
                    return;
                }*/
            }
            // FRIENDS PAGE
            else if (m_Page == 1)
            {
                if (info.ButtonID == 110)
                {
                    m_User.SendGump(new StaticHouseCoOwnerListGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 111)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Solo il proprietario può modificare la lista amici o co-owner.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                        return;
                    }
                    m_User.SendMessage("Select the player to add as co-owner.");
                    m_User.Target = new AddCoOwnerTarget(m_Sign);
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 112)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Solo il proprietario può modificare la lista amici o co-owner.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                        return;
                    }
                    m_User.SendGump(new StaticHouseCoOwnerRemoveGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 113)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Solo il proprietario può modificare la lista amici o co-owner.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                        return;
                    }
                    m_User.SendMessage("Select the player to add as friend.");
                    m_User.Target = new AddFriendTarget(m_Sign);
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 114)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Solo il proprietario può modificare la lista amici o co-owner.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                        return;
                    }
                    m_User.SendGump(new StaticHouseFriendRemoveGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 115)
                {
                    m_User.SendGump(new StaticHouseFriendListGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 116)
                {
                    if (!(m_Sign.Owner == m_User || m_Sign.IsCoOwner(m_User)))
                    {
                        m_User.SendMessage("Solo il proprietario o i co-owner possono bannare.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                        return;
                    }
                    m_User.SendMessage("Seleziona il giocatore da bannare.");
                    m_User.Target = new AddBanTarget(m_Sign, m_User);
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 117)
                {
                    m_User.SendGump(new StaticHouseBanListGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 1));
                    return;
                }
            }
            // OPTION PAGE
            else if (m_Page == 2)
            {
                if (info.ButtonID == 210)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Only the owner can transfer the house.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 2));
                        return;
                    }
                    m_User.SendMessage("Select the player to transfer the house to.");
                    m_User.Target = new TransferOwnerTarget(m_Sign, m_User);
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 2));
                    return;
                }
                if (info.ButtonID == 211)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Only the owner can change the house's name.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 2));
                        return;
                    }
                    m_User.SendMessage("Type the new house name in the chat bar and press ENTER.");
                    m_User.Prompt = new ChangeNamePrompt(m_Sign, m_User);
                    m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 2));
                    return;
                }
                if (info.ButtonID == 212)
                {
                    if (m_Sign.Owner != m_User)
                    {
                        m_User.SendMessage("Only the owner can abandon the house.");
                        m_User.SendGump(new StaticHouseSignGumpOwner(m_Sign, m_User, 2));
                        return;
                    }
                    int refund = m_Sign.SalePrice > 0 ? m_Sign.SalePrice : 0;
                    if (refund > 0 && m_User.Backpack != null)
                    {
                        m_User.Backpack.DropItem(new Gold(refund));
                        m_User.SendMessage("You have received a refund of {0} gp.", refund);
                    }

                    RemoveAllHouseKeys(m_Sign);
                    m_Sign.UnassignKeysFromDoors();
                    m_Sign.UnlockAllItemsInHouse();
                    m_Sign.Owner = null;
                    
                    // SET ALL THE VALUE TO DEFAULT
                    var def = m_Sign.DefaultSettings;
                    m_Sign.HouseName = def.HouseName;
                    m_Sign.SalePrice = def.SalePrice;
                    m_Sign.RentPrice = def.RentPrice;
                    //m_Sign.HouseArea = def.HouseArea;
                    m_Sign.HouseAreas = new List<Rectangle2D>(def.HouseAreas); // NUOVO SISTEMA MULTI-AREA
                    m_Sign.ForSale = def.ForSale;
                    m_Sign.ForRent = def.ForRent;
                    m_Sign.RequiredKarma = def.RequiredKarma;
                    m_Sign.RequiredFame = def.RequiredFame;
                    m_User.SendMessage("You have abandoned the house, it is now available again.");
                    // NOT OPEN THE GUMP AGAIN
                    m_User.CloseGump(typeof(StaticHouseSignGumpOwner));
                    return;
                }
            }
        }

        // ----------- SUB-GUMP E TARGET ----------- 

        public class StaticHouseCoOwnerListGump : Gump
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;
            public StaticHouseCoOwnerListGump(StaticHouseSign sign, Mobile user)
                : base(100, 100)
            {
                m_Sign = sign;
                m_User = user;
                AddBackground(0, 0, 300, 300, 9270);
                AddLabel(90, 10, 0, "Co-Owner List");
                int y = 40;
                foreach (Mobile m in sign.CoOwners)
                {
                    AddLabel(40, y, 33, m.Name);
                    y += 25;
                }
                AddButton(100, 260, 4020, 4021, 2998, GumpButtonType.Reply, 0); // Close
                AddLabel(130, 260, 0, "Close");
            }
            public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 2998) return;
            }
        }

        public class StaticHouseCoOwnerRemoveGump : Gump
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;
            public StaticHouseCoOwnerRemoveGump(StaticHouseSign sign, Mobile user)
                : base(100, 100)
            {
                m_Sign = sign;
                m_User = user;
                AddBackground(0, 0, 300, 300, 9270);
                AddLabel(90, 10, 0, "Remove Co-Owner");
                int y = 40, idx = 0;
                foreach (Mobile m in sign.CoOwners)
                {
                    AddLabel(40, y, 33, m.Name);
                    AddButton(200, y, 4005, 4007, 2000 + idx, GumpButtonType.Reply, 0);
                    y += 25; idx++;
                }
                AddButton(100, 260, 4020, 4021, 2998, GumpButtonType.Reply, 0); // Close
                AddLabel(130, 260, 0, "Close");
            }
            public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 2998) return;
                int idx = info.ButtonID - 2000;
                if (idx >= 0 && m_Sign != null && idx < m_Sign.CoOwners.Count)
                {
                    Mobile toRemove = m_Sign.CoOwners[idx];
                    m_Sign.RemoveCoOwner(toRemove);
                    m_User.SendMessage("Co-owner removed.");
                }
            }
        }

        public class StaticHouseFriendListGump : Gump
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;
            public StaticHouseFriendListGump(StaticHouseSign sign, Mobile user)
                : base(100, 100)
            {
                m_Sign = sign;
                m_User = user;
                AddBackground(0, 0, 300, 300, 9270);
                AddLabel(90, 10, 0, "Friend List");
                int y = 40;
                foreach (Mobile m in sign.Friends)
                {
                    AddLabel(40, y, 33, m.Name);
                    y += 25;
                }
                AddButton(100, 260, 4020, 4021, 2998, GumpButtonType.Reply, 0); // Close
                AddLabel(130, 260, 0, "Close");
            }
            public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 2998) return;
            }
        }

        public class StaticHouseFriendRemoveGump : Gump
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;
            public StaticHouseFriendRemoveGump(StaticHouseSign sign, Mobile user)
                : base(100, 100)
            {
                m_Sign = sign;
                m_User = user;
                AddBackground(0, 0, 300, 300, 9270);
                AddLabel(90, 10, 0, "Remove Friend");
                int y = 40, idx = 0;
                foreach (Mobile m in sign.Friends)
                {
                    AddLabel(40, y, 33, m.Name);
                    AddButton(200, y, 4005, 4007, 2100 + idx, GumpButtonType.Reply, 0);
                    y += 25; idx++;
                }
                AddButton(100, 260, 4020, 4021, 2998, GumpButtonType.Reply, 0); // Close
                AddLabel(130, 260, 0, "Close");
            }
            public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 2998) return;
                int idx = info.ButtonID - 2100;
                if (idx >= 0 && m_Sign != null && idx < m_Sign.Friends.Count)
                {
                    Mobile toRemove = m_Sign.Friends[idx];
                    m_Sign.RemoveFriend(toRemove);
                    m_User.SendMessage("Friend removed.");
                }
            }
        }

        public class StaticHouseBanListGump : Gump
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;
            public StaticHouseBanListGump(StaticHouseSign sign, Mobile user)
                : base(100, 100)
            {
                m_Sign = sign;
                m_User = user;
                AddBackground(0, 0, 300, 300, 9270);
                AddLabel(90, 10, 0, "Ban List");
                int y = 40;
                foreach (Mobile m in sign.Bans)
                {
                    AddLabel(40, y, 33, m.Name);
                    y += 25;
                }
                AddButton(100, 260, 4020, 4021, 2998, GumpButtonType.Reply, 0); // Close
                AddLabel(130, 260, 0, "Close");
            }
            public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 2998) return;
            }
        }

        // ---- Target e Prompt ----

        public class AddCoOwnerTarget : Target
        {
            private StaticHouseSign m_Sign;
            public AddCoOwnerTarget(StaticHouseSign sign) : base(10, false, TargetFlags.None)
            {
                m_Sign = sign;
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                Mobile m = targeted as Mobile;
                if (m != null)
                {
                    m_Sign.AddCoOwner(m);
                    from.SendMessage("Added as co-owner.");
                }
            }
        }

        public class AddFriendTarget : Target
        {
            private StaticHouseSign m_Sign;
            public AddFriendTarget(StaticHouseSign sign) : base(10, false, TargetFlags.None)
            {
                m_Sign = sign;
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                Mobile m = targeted as Mobile;
                if (m != null)
                {
                    m_Sign.AddFriend(m);
                    from.SendMessage("Added as friend.");
                }
            }
        }

        public class AddBanTarget : Target
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;
            public AddBanTarget(StaticHouseSign sign, Mobile user) : base(10, false, TargetFlags.None)
            {
                m_Sign = sign;
                m_User = user;
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                Mobile targetMobile = targeted as Mobile;
                if (targetMobile == null)
                    return;
                if (!(m_Sign.Owner == from || m_Sign.IsCoOwner(from)))
                {
                    from.SendMessage("Solo il proprietario o i co-owner possono bannare.");
                    return;
                }
                if (targetMobile == m_Sign.Owner)
                {
                    from.SendMessage("Non puoi bannare il proprietario della casa.");
                    return;
                }
                m_Sign.AddBan(targetMobile);
                from.SendMessage("Hai bannato {0}.", targetMobile.Name);
            }
        }

        private class ChangeNamePrompt : Prompt
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;

            public ChangeNamePrompt(StaticHouseSign sign, Mobile user)
            {
                m_Sign = sign;
                m_User = user;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Sign != null && !m_Sign.Deleted && m_Sign.Owner == from)
                {
                    string newName = text.Trim();
                    if (string.IsNullOrEmpty(newName))
                    {
                        from.SendMessage("Invalid house name.");
                        from.SendGump(new StaticHouseSignGumpOwner(m_Sign, from, 2));
                        return;
                    }
                    m_Sign.HouseName = newName;
                    from.SendMessage("House name changed to: {0}", newName);
                    from.SendGump(new StaticHouseSignGumpOwner(m_Sign, from, 2));
                }
            }
        }

        private class TransferOwnerTarget : Target
        {
            private StaticHouseSign m_Sign;
            private Mobile m_User;

            public TransferOwnerTarget(StaticHouseSign sign, Mobile user) : base(10, false, TargetFlags.None)
            {
                m_Sign = sign;
                m_User = user;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Mobile m = targeted as Mobile;
                if (m == null || m == from)
                {
                    from.SendMessage("Invalid target.");
                    return;
                }
                if (m_Sign.Owner != from)
                {
                    from.SendMessage("Only the owner can transfer the house.");
                    return;
                }
                if (Server.StaticHouse.StaticHouseHelper.HasAnyHouse(m))
                {
                    from.SendMessage("That player already owns a house and cannot receive this one!");
                    return;
                }
                m_Sign.Owner = m;
                from.SendMessage("You have transferred the house to {0}.", m.Name);
                m.SendMessage("You are now the owner of the house!");
                m_Sign.AssignKeysToOwner(m);

                from.CloseGump(typeof(StaticHouseSignGumpOwner));
            }
        }


        private void RemoveAllHouseKeys(StaticHouseSign house)
        {
            if (house.AssociatedDoors != null)
            {
                foreach (BaseDoor door in house.AssociatedDoors)
                {
                    if (door != null && door.KeyValue != 0)
                    {
                        
                        List<Key> toDelete = new List<Key>();
                        foreach (Item item in World.Items.Values)
                        {
                            Key key = item as Key;
                            if (key != null && key.KeyValue == door.KeyValue)
                                toDelete.Add(key);
                        }
                        
                        foreach (Key key in toDelete)
                            key.Delete();
                    }
                }
            }
        }


    }
}