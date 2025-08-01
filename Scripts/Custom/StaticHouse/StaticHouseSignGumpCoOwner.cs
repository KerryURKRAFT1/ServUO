using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.StaticHouse;
using Server.Targeting;
using Server.Prompts;
using Server.Items;

namespace Server.StaticHouse
{
    public class StaticHouseSignGumpCoOwner : Gump
    {
        private StaticHouseSign m_Sign;
        private Mobile m_User;
        private int m_Page;

        public StaticHouseSignGumpCoOwner(StaticHouseSign sign, Mobile from, int page = 0)
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
            }
        }

        private void BuildMainPage()
        {
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

            // NO OPTION TAB!

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

            // NO ABANDON BUTTON!
        }

        private void BuildFriendsPage()
        {
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

            // NO OPTION TAB!

            // --- CO-OWNER SECTION: ONLY VIEW LIST BUTTON ---
            AddLabel(80, 70 + yOffset, 0, "View Co-Owner List");
            AddButton(30, 70 + yOffset, 4005, 4007, 110, GumpButtonType.Reply, 0);

            // --- FRIEND SECTION ---
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

            // NO ADD/REMOVE CO-OWNER BUTTONS!
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_Sign == null || m_Sign.Deleted)
                return;

            if (info.ButtonID == 998)
                return;

            // TAB
            if (info.ButtonID == 10) { m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 0)); return; }
            if (info.ButtonID == 11) { m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1)); return; }

            // MAIN PAGE
            if (m_Page == 0)
            {
                if (info.ButtonID == 20)
                {
                    m_Sign.RefreshDecay();
                    m_User.SendMessage("You have refreshed the house. The decay timer has been reset.");
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 0));
                    return;
                }
            }
            // FRIENDS PAGE
            else if (m_Page == 1)
            {
                if (info.ButtonID == 110)
                {
                    m_User.SendGump(new StaticHouseSignGumpOwner.StaticHouseCoOwnerListGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 113)
                {
                    m_User.SendMessage("Select the player to add as friend.");
                    m_User.Target = new StaticHouseSignGumpOwner.AddFriendTarget(m_Sign);
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 114)
                {
                    m_User.SendGump(new StaticHouseSignGumpOwner.StaticHouseFriendRemoveGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 115)
                {
                    m_User.SendGump(new StaticHouseSignGumpOwner.StaticHouseFriendListGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 116)
                {
                    m_User.SendMessage("Select the player to ban.");
                    m_User.Target = new StaticHouseSignGumpOwner.AddBanTarget(m_Sign, m_User);
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1));
                    return;
                }
                if (info.ButtonID == 117)
                {
                    m_User.SendGump(new StaticHouseSignGumpOwner.StaticHouseBanListGump(m_Sign, m_User));
                    m_User.SendGump(new StaticHouseSignGumpCoOwner(m_Sign, m_User, 1));
                    return;
                }
            }
        }
    }
}