using System;
using Server.Engines.Craft;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.SkillHandlers;

namespace Server.Items
{
    public enum ToolQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public abstract class BaseTool : Item, IUsesRemaining, ICraftable
    {
        private Mobile m_Crafter;
        private ToolQuality m_Quality;
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ToolQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.UnscaleUses();
                this.m_Quality = value;
                this.InvalidateProperties();
                this.ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        public void ScaleUses()
        {
            this.m_UsesRemaining = (this.m_UsesRemaining * this.GetUsesScalar()) / 100;
            this.InvalidateProperties();
        }

        public void UnscaleUses()
        {
            this.m_UsesRemaining = (this.m_UsesRemaining * 100) / this.GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (this.m_Quality == ToolQuality.Exceptional)
                return 200;

            return 100;
        }

        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public abstract CraftSystem CraftSystem { get; }

        public BaseTool(int itemID)
            : this(Utility.RandomMinMax(25, 75), itemID)
        {
        }

        public BaseTool(int uses, int itemID)
            : base(itemID)
        {
            this.m_UsesRemaining = uses;
            this.m_Quality = ToolQuality.Regular;
        }

        public BaseTool(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Quality == ToolQuality.Exceptional)
                list.Add(1060636); // exceptional

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo(Mobile m)
        {
            this.LabelToAffix(m, 1017323, AffixType.Append, ": " + this.m_UsesRemaining.ToString()); // Durability
        }

        public static bool CheckAccessible(Item tool, Mobile m)
        {
            return (tool.IsChildOf(m) || tool.Parent == m);
        }

        public static bool CheckTool(Item tool, Mobile m)
        {
            Item check = m.FindItemOnLayer(Layer.OneHanded);

            if (check is BaseTool && check != tool && !(check is AncientSmithyHammer))
                return false;

            check = m.FindItemOnLayer(Layer.TwoHanded);

            if (check is BaseTool && check != tool && !(check is AncientSmithyHammer))
                return false;

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            this.DisplayDurabilityTo(from);

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (this.IsChildOf(m.Backpack) || this.Parent == m)
            {
	    		if (!SkillRegistry.Contains(m))
	    		{
		    		SkillRegistry.Add(m);
		        	
		        	if (!TriggerSkill(m))
		        	{
			    		SkillRegistry.Remove(m);
		        	}
	    		}
	    		else if (SkillRegistry.WaitMsg)
	    		{
	                m.SendMessage("You must wait to perform another action.");
	    		}
            }
            else
            {
                m.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public bool TriggerSkill(Mobile m)
        {
            new SkillTimer(m, this, SkillRegistry.ShortDelay).Start();
            
			return true;
        }
        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly BaseTool m_BaseTool;

            public SkillTimer(Mobile owner, BaseTool tool, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_BaseTool = tool;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
                CraftSystem system = m_BaseTool.CraftSystem;

                int num = system.CanCraft(m_Owner, m_BaseTool, null);

                if (num > 0 && (num != 1044267 || !Core.SE))
                {
                    m_Owner.SendLocalizedMessage(num);
                }
                else
                {                    
                    if (system == DefCarpentry.CraftSystem)
                    {
                        NewCarpentryMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefTinkering.CraftSystem)
                    {
                        NewTinkeringMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefClassicTailoring.CraftSystem)
                    {
                        NewTailoringMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefClassicBlacksmithy.CraftSystem)
                    {
                        NewBlacksmithyMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefClassicBowFletching.CraftSystem)
                    {
                        NewFletchingMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefClassicAlchemy.CraftSystem)
                    {
                        NewAlchemyMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefClassicInscription.CraftSystem)
                    {
                        NewInscriptionMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefCartography.CraftSystem)
                    {
                        NewCartographyMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else if (system == DefClassicCooking.CraftSystem)
                    {
                        NewCookingMenu.CreateMenu(m_Owner, system, m_BaseTool, 0, true);
                    }
                    else
                    {
		                m_Owner.SendLocalizedMessage(1005213); // You can't do that
                    }
                }
                
				SkillRegistry.Remove(m_Owner);
			}
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((Mobile)this.m_Crafter);
            writer.Write((int)this.m_Quality);

            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        this.m_Crafter = reader.ReadMobile();
                        this.m_Quality = (ToolQuality)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ToolQuality)quality;

            if (makersMark)
                this.Crafter = from;

            return quality;
        }
        #endregion
    }
}