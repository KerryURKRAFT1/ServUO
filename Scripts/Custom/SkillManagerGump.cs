using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Custom
{
    public class SkillManagerCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SkillManager", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        [Usage("SkillManager")]
        [Description("Opens a gump to manage your skills and stats (respecting skill/stat cap).")]
        public static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.CloseGump(typeof(SkillManagerGump));
            from.SendGump(new SkillManagerGump(from, 0, 0));
        }
    }

    public class SkillManagerGump : Gump
    {
        private const int ButtonsStartId = 1000;
        private const int StatsButtonsStartId = 2000;
        private const int UtilityButtonsStartId = 9100;
        private Mobile m_User;
        private int m_SelectedSkillIdx;
        private int m_SelectedStatIdx;
        private List<SkillName> m_SkillNames;
        private static readonly string[] StatNames = { "Strength", "Dexterity", "Intelligence" };
        private const int GumpWidth = 1030;
        private const int GumpHeight = 780;
        private const int SkillColumnWidth = 150;
        private const int SkillColumnPadding = 20;
        private const int SkillRadioHeight = 22;
        private const int SkillRadiosPerCol = 13;

        public SkillManagerGump(Mobile user, int selectedSkillIdx, int selectedStatIdx)
            : base(50, 50)
        {
            m_User = user;
            m_SelectedSkillIdx = selectedSkillIdx;
            m_SelectedStatIdx = selectedStatIdx;
            m_SkillNames = new List<SkillName>();

            Closable = true;
            AddPage(0);

			CustomBackground(0, 0, GumpWidth, GumpHeight, 9270, 2624);
//			AddBackground(0, 0, GumpWidth, GumpHeight, 9200); // Light background

            AddLabel(40, 20, 60, "Skill & Stat Manager");
            AddImageTiled(30, 50, GumpWidth - 60, 2, 9304); // Divider line

            AddLabel(40, 65, 1153, "Skill Management");
            AddLabel(40 + 230 * 3, 65, 54, $"Skill Cap: {user.Skills.Cap / 10.0:F1}");
            AddLabel(40 + 120 + 230 * 3, 65, 54, $"Total: {user.Skills.Total / 10.0:F1}");

            // Compile list of visible skills
            for (int i = 0; i < user.Skills.Length; i++)
            {
                if (user.Skills[i].Cap > 0)
                    m_SkillNames.Add(user.Skills[i].SkillName);
            }

            // Skill radio selection (up to 4 columns)
//            AddLabel(40, 80, 1153, "Select a skill:");
            int radiosPerCol = SkillRadiosPerCol;
            int maxColumns = (int)Math.Ceiling(m_SkillNames.Count / (double)radiosPerCol);
            int radio = 0;
            int col = 0;

            for (int i = 0; i < m_SkillNames.Count; i++)
            {
                int rx = 40 + (col * (SkillColumnWidth + SkillColumnPadding));
                int ry = 110 + (radio * SkillRadioHeight);
                AddRadio(rx, ry, 210, 211, i == m_SelectedSkillIdx, i);
                AddLabel(rx + 25, ry, 60, m_User.Skills[m_SkillNames[i]].Info.Name);

                radio++;
                if (radio >= radiosPerCol)
                {
                    col++;
                    radio = 0;
                }
            }

            // Skill details and buttons to the right, well spaced
            int skillDetailX = 850;
            int skillDetailY = 120;
            SkillName selectedSkill = m_SkillNames[Math.Max(0, Math.Min(m_SelectedSkillIdx, m_SkillNames.Count - 1))];
            Skill skill = m_User.Skills[selectedSkill];

            AddLabel(skillDetailX+30, skillDetailY, 54, $"Skill: {skill.Info.Name}");
            AddLabel(skillDetailX+30, skillDetailY + 30, 54, $"Current: {skill.Base:F1}");
            AddLabel(skillDetailX+30, skillDetailY + 60, 54, $"Cap: {skill.Cap:F1}");

            int btnY = skillDetailY + 100;
            int btnSpacing = 48;
            AddButton(skillDetailX+30, btnY, 4014, 4015, ButtonsStartId + 0, GumpButtonType.Reply, 0);
            AddLabel(skillDetailX+70, btnY + 3, 33, "-10");

            AddButton(skillDetailX+30, btnY + btnSpacing, 4014, 4015, ButtonsStartId + 1, GumpButtonType.Reply, 0);
            AddLabel(skillDetailX+70, btnY + btnSpacing + 3, 33, "-1");

            AddButton(skillDetailX+30, btnY + btnSpacing * 2, 4011, 4012, ButtonsStartId + 2, GumpButtonType.Reply, 0);
            AddLabel(skillDetailX+70, btnY + btnSpacing * 2 + 3, 33, "+1");

            AddButton(skillDetailX+30, btnY + btnSpacing * 3, 4011, 4012, ButtonsStartId + 3, GumpButtonType.Reply, 0);
            AddLabel(skillDetailX+70, btnY + btnSpacing * 3 + 3, 33, "+10");

            // Skillcap status message
            int msgY = btnY + btnSpacing * 4 + 10;
            if (m_User.Skills.Total >= m_User.Skills.Cap)
                AddLabel(skillDetailX, msgY, 33, "You have reached your skill cap! You cannot raise further skills.");
            else if (skill.Base >= skill.Cap)
                AddLabel(skillDetailX, msgY, 33, "This skill is already at its maximum cap!");

            // ---- STAT SECTION ----
            int statSectionY = 110 + (radiosPerCol * SkillRadioHeight) + 40;
            AddImageTiled(30, statSectionY - 18, GumpWidth - 60, 2, 9304); // Divider line
            AddLabel(40, statSectionY, 1153, "Stats Management");

            int statColWidth = 230;
            int statBaseX = 40;
            int statBaseY = statSectionY + 40;
            int statRowSpacing = 60;

            for (int s = 0; s < StatNames.Length; s++)
            {
                int sx = statBaseX + s * statColWidth;
                int sy = statBaseY;
                bool isSelected = (s == m_SelectedStatIdx);

                AddRadio(sx, sy, 210, 211, isSelected, 100 + s); // stat selection radios
                AddLabel(sx + 25, sy, 60, StatNames[s]);

                int statValue = 0, statCap = 0;
                switch (s)
                {
                    case 0: statValue = m_User.RawStr; statCap = m_User.StatCap; break;
                    case 1: statValue = m_User.RawDex; statCap = m_User.StatCap; break;
                    case 2: statValue = m_User.RawInt; statCap = m_User.StatCap; break;
                }

                AddLabel(sx, sy + 25, 54, $"Current: {statValue}");
                AddLabel(sx, sy + 45, 54, $"Cap: {statCap}");

                // Stat buttons
                int statBtnY = sy + 4;
                int statBtnSpacing = 27;
                AddButton(sx + 95, statBtnY, 4014, 4015, StatsButtonsStartId + (s * 4) + 0, GumpButtonType.Reply, 0); // -10
                AddLabel(sx + 135, statBtnY + 3, 33, "-10");

                AddButton(sx + 95, statBtnY + statBtnSpacing, 4014, 4015, StatsButtonsStartId + (s * 4) + 1, GumpButtonType.Reply, 0); // -1
                AddLabel(sx + 135, statBtnY + statBtnSpacing + 3, 33, "-1");

                AddButton(sx + 95, statBtnY + statBtnSpacing * 2, 4011, 4012, StatsButtonsStartId + (s * 4) + 2, GumpButtonType.Reply, 0); // +1
                AddLabel(sx + 135, statBtnY + statBtnSpacing * 2 + 3, 33, "+1");

                AddButton(sx + 95, statBtnY + statBtnSpacing * 3, 4011, 4012, StatsButtonsStartId + (s * 4) + 3, GumpButtonType.Reply, 0); // +10
                AddLabel(sx + 135, statBtnY + statBtnSpacing * 3 + 3, 33, "+10");
            }

            // Stat cap info
            int statTotal = m_User.RawStr + m_User.RawDex + m_User.RawInt;
            AddLabel(statBaseX + statColWidth * 3, statSectionY, 54, $"Stat Cap: {m_User.StatCap}");
            AddLabel(statBaseX + 120 + statColWidth * 3, statSectionY, 54, $"Total: {statTotal}");

            // Stat cap status message
            int statMsgY = statBaseY + statRowSpacing + 20;
            if (statTotal >= m_User.StatCap)
                AddLabel(statBaseX + statColWidth, statMsgY + 50, 33, "You have reached your stat cap! You cannot raise further stats.");

            // --- Utility Buttons Section (bottom left) ---
            int utilBtnY = GumpHeight - 120;
            int utilBtnX = 60;

            // Bank Check 100k button
            AddButton(utilBtnX, utilBtnY, 4011, 4012, UtilityButtonsStartId + 1, GumpButtonType.Reply, 0);
            AddLabel(utilBtnX + 35, utilBtnY + 3, 54, "Bank Check 100k");

            // Bag of Reagents button
            AddButton(utilBtnX, utilBtnY + 40, 4011, 4012, UtilityButtonsStartId + 2, GumpButtonType.Reply, 0);
            AddLabel(utilBtnX + 35, utilBtnY + 43, 54, "Bag of Reagents");

            // Full Spellbook button
            AddButton(utilBtnX, utilBtnY + 80, 4011, 4012, UtilityButtonsStartId + 3, GumpButtonType.Reply, 0);
            AddLabel(utilBtnX + 35, utilBtnY + 83, 54, "Full Spellbook");

            // OK/Close button bottom right
            AddButton(GumpWidth - 110, GumpHeight - 60, 4023, 4024, 9000, GumpButtonType.Reply, 0);
//            AddLabel(GumpWidth - 78, GumpHeight - 57, 54, "OK");
        }

		public void CustomBackground( int x, int y, int width, int height, int bg, int it )
		{
			AddBackground (x, y, width, height, bg);
			AddImageTiled( x+5, y+5, width-10, height-10, it );
			AddAlphaRegion( x+5, y+5, width-10, height-10 );
		}
		
        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            int selectedSkillIdx = m_SelectedSkillIdx;
            int selectedStatIdx = m_SelectedStatIdx;

            // Handle skill/stat selection
            if (info.Switches != null)
            {
                foreach (int idx in info.Switches)
                {
                    if (idx < 100) // Skill radio
                        selectedSkillIdx = idx;
                    else if (idx >= 100 && idx < 200) // Stat radio
                        selectedStatIdx = idx - 100;
                }
            }

            // Skill and stat lists
            var skillNames = new List<SkillName>();
            for (int i = 0; i < m_User.Skills.Length; i++)
                if (m_User.Skills[i].Cap > 0)
                    skillNames.Add(m_User.Skills[i].SkillName);

            SkillName selectedSkill = skillNames[Math.Max(0, Math.Min(selectedSkillIdx, skillNames.Count - 1))];
            Skill skill = m_User.Skills[selectedSkill];
            double cap = m_User.Skills.Cap;
            
            switch (info.ButtonID)
            {
                case 0: //Close
                case 9000: // OK
					return;

				// Skill buttons
                case ButtonsStartId + 0: // -10
                    if (skill.Base >= 10)
                        { skill.Base -= 10; m_User.SendMessage(33, $"-10 to {skill.Info.Name}. New value: {skill.Base / 10.0:F1}"); }
                    else
                        m_User.SendMessage(33, "You cannot go below 0.");
                    break;
                case ButtonsStartId + 1: // -1
                    if (skill.Base >= 1)
                        { skill.Base -= 1; m_User.SendMessage(33, $"-1 to {skill.Info.Name}. New value: {skill.Base / 10.0:F1}"); }
                    else
                        m_User.SendMessage(33, "You cannot go below 0.");
                    break;
                case ButtonsStartId + 2: // +1
                    if (skill.Base < skill.Cap && m_User.Skills.Total < cap)
                    {
                        double maxAdd = Math.Min(1, cap - m_User.Skills.Total);
                        if (maxAdd > 0)
                            { skill.Base += 1; m_User.SendMessage(68, $"+1 to {skill.Info.Name}. New value: {skill.Base / 10.0:F1}"); }
                        else
                            m_User.SendMessage(33, "You have reached your skill cap!");
                    }
                    else
                        m_User.SendMessage(33, "You have reached the skill or skill cap!");
                    break;
                case ButtonsStartId + 3: // +10
                    if (skill.Base < skill.Cap && m_User.Skills.Total < cap)
                    {
                        double maxAdd = Math.Min(10, skill.Cap - skill.Base);
                        maxAdd = Math.Min(maxAdd, cap - m_User.Skills.Total);
                        if (maxAdd > 0)
                            { skill.Base += (ushort)maxAdd; m_User.SendMessage(68, $"+{maxAdd} to {skill.Info.Name}. New value: {skill.Base / 10.0:F1}"); }
                        else
                            m_User.SendMessage(33, "You have reached your skill cap!");
                    }
                    else
                        m_User.SendMessage(33, "You have reached the skill or skill cap!");
                    break;
                // Stat buttons
                default:
                    if (info.ButtonID >= StatsButtonsStartId && info.ButtonID < StatsButtonsStartId + 12)
                    {
                        int statIndex = (info.ButtonID - StatsButtonsStartId) / 4;
                        int action = (info.ButtonID - StatsButtonsStartId) % 4;

                        int str = m_User.RawStr, dex = m_User.RawDex, intel = m_User.RawInt;
                        int statCap = m_User.StatCap;
                        int statTotal = str + dex + intel;

                        int stat = str;
                        switch (statIndex)
                        {
                            case 0: stat = str; break;
                            case 1: stat = dex; break;
                            case 2: stat = intel; break;
                        }

                        int amount = 0;
                        string statName = StatNames[statIndex];

                        switch (action)
                        {
                            case 0: amount = -10; break;
                            case 1: amount = -1; break;
                            case 2: amount = 1; break;
                            case 3: amount = 10; break;
                        }

                        if (amount < 0)
                        {
                            if (stat + amount >= 10)
                            {
                                stat += amount;
                                m_User.SendMessage(33, $"{amount} to {statName}. New value: {stat}");
                            }
                            else
                                m_User.SendMessage(33, $"You cannot go below 10 in {statName}.");
                        }
                        else if (amount > 0)
                        {
                            if (stat + amount <= 125 && statTotal - stat + (stat + amount) <= statCap)
                            {
                                stat += amount;
                                m_User.SendMessage(68, $"+{amount} to {statName}. New value: {stat}");
                            }
                            else if (stat + amount > 125)
                                m_User.SendMessage(33, $"You cannot go above 125 in {statName}.");
                            else
                                m_User.SendMessage(33, "You have reached your stat cap!");
                        }
						
                        //modded here
                        switch (statIndex)
                        {
                            case 0: str = stat; break;
                            case 1: dex = stat; break;
                            case 2: intel = stat; break;
                        }

                        // Apply changes
                        m_User.RawStr = str;
                        m_User.RawDex = dex;
                        m_User.RawInt = intel;
                    }
                    // --- Utility buttons ---
                    else if (info.ButtonID == UtilityButtonsStartId + 1)
                    {
                        // Bank Check 100k in bank
                        BankCheck check = new BankCheck(100000);
                        check.LootType = LootType.Blessed;
                        if (m_User.BankBox != null && m_User.BankBox.TryDropItem(m_User, check, false))
                            m_User.SendMessage(68, "You received a 100,000 gold bank check in your bank.");
                        else
                            m_User.SendMessage(33, "Could not place the bank check in your bank.");
                    }
                    else if (info.ButtonID == UtilityButtonsStartId + 2)
                    {
                        // Bag of Reagents in backpack
                        BagOfReagents bag = new BagOfReagents();
                        if (m_User.Backpack != null && m_User.Backpack.TryDropItem(m_User, bag, false))
                            m_User.SendMessage(68, "You received a Bag of Reagents in your backpack.");
                        else
                            m_User.SendMessage(33, "Could not place the Bag of Reagents in your backpack.");
                    }
                    else if (info.ButtonID == UtilityButtonsStartId + 3)
                    {
                        // Full Spellbook in backpack
                        Spellbook book = new Spellbook(ulong.MaxValue); // Magery (tutte le magie)
                        book.LootType = LootType.Blessed;
                        if (m_User.Backpack != null && m_User.Backpack.TryDropItem(m_User, book, false))
                            m_User.SendMessage(68, "You received a full spellbook in your backpack.");
                        else
                            m_User.SendMessage(33, "Could not place the full spellbook in your backpack.");
                    }
                    else
                    {
                    	return;
                    }
                    break;
            }

            m_User.SendGump(new SkillManagerGump(m_User, selectedSkillIdx, selectedStatIdx));
        }
    }
}