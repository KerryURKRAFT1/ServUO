using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Harvest;

namespace Server.Items
{
    public abstract class BasePoleArm : BaseMeleeWeapon, IUsesRemaining
    {
        private int m_UsesRemaining;
        private bool m_ShowUsesRemaining;
        public BasePoleArm(int itemID)
            : base(itemID)
        {
            this.m_UsesRemaining = 150;
        }

        public BasePoleArm(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound
        {
            get
            {
                return 0x237;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x238;
            }
        }
        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Swords;
            }
        }
        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Polearm;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Slash2H;
            }
        }
        public virtual HarvestSystem HarvestSystem
        {
            get
            {
                return Lumberjacking.System;
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
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get
            {
                return this.m_ShowUsesRemaining;
            }
            set
            {
                this.m_ShowUsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        
                public override void OnDoubleClick( Mobile from )
                {
                    ClickToEquip.OnDoubleClick( from, this );
                }
         
 
/*
public override void OnDoubleClick(Mobile from)
        {
            // UOR behavior for equipping the axe
            if (Core.UOR)
            {
                // If the item is inside the backpack or inside any container within the backpack
                if (IsAccessibleFromBackpack(from.Backpack))
                {
                    RemoveHandItemsForEquip(from, this.Layer);

                    // Try to equip this item
                    if (from.EquipItem(this))
                    {
                        from.PlaySound(0x57); // Equip sound
                    }
                    else
                    {
                        from.SendMessage("You cannot equip this item.");
                    }
                    return;
                }
                // If the item is on the ground, within 2 tiles, movable, and with line of sight
                else if (this.Parent == null && this.Map == from.Map && from.InRange(this.GetWorldLocation(), 2) && this.Movable && from.CanSee(this) && from.InLOS(this))
                {
                    // Try to pick up the item and move it to the backpack
                    if (from.Backpack != null && from.Backpack.TryDropItem(from, this, false))
                    {
                        RemoveHandItemsForEquip(from, this.Layer);

                        // Try to equip this item
                        if (from.EquipItem(this))
                        {
                            from.PlaySound(0x57);
                        }
                        else
                        {
                            from.SendMessage("You cannot equip this item.");
                        }
                    }
                    else
                    {
                        from.SendMessage("You cannot pick up or equip this item.");
                    }
                    return;
                }
                // If already equipped, start targeting logic for harvesting
                else if (from.FindItemOnLayer(Layer.OneHanded) == this || from.FindItemOnLayer(Layer.TwoHanded) == this)
                {
                    if (this.HarvestSystem == null || this.Deleted)
                        return;

                    Point3D loc = this.GetWorldLocation();

                    if (!from.InLOS(loc) || !from.InRange(loc, 2))
                    {
                        from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3E9, 1019045); // I can't reach that
                        return;
                    }
                    else if (!this.IsAccessibleTo(from))
                    {
                        this.PublicOverheadMessage(Server.Network.MessageType.Regular, 0x3E9, 1061637); // You are not allowed to access this.
                        return;
                    }
                    
                    if (!(this.HarvestSystem is Mining))
                        from.SendLocalizedMessage(1010018); // What do you want to use this item on?

                    this.HarvestSystem.BeginHarvesting(from, this);
                    return;
                }
                else
                {
                    from.SendMessage("You must equip the item to use it.");
                    return;
                }
            }
            else
            {
                // Default behavior for non-UOR cores (original harvesting logic)
                if (this.HarvestSystem == null || this.Deleted)
                    return;

                Point3D loc = this.GetWorldLocation();

                if (!from.InLOS(loc) || !from.InRange(loc, 2))
                {
                    from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3E9, 1019045); // I can't reach that
                    return;
                }
                else if (!this.IsAccessibleTo(from))
                {
                    this.PublicOverheadMessage(Server.Network.MessageType.Regular, 0x3E9, 1061637); // You are not allowed to access this.
                    return;
                }
                
                if (!(this.HarvestSystem is Mining))
                    from.SendLocalizedMessage(1010018); // What do you want to use this item on?

                this.HarvestSystem.BeginHarvesting(from, this);
            }
        }

        // Utility: Recursively checks if the item is inside any container within the backpack
        private bool IsAccessibleFromBackpack(Container backpack)
        {
            Item current = this;
            while (current.Parent is Container container)
            {
                if (container == backpack)
                    return true;
                current = container;
            }
            return false;
        }

        
        // Utility: Removes the correct item/equipment based on the layer of the item you are about to equip
        private void RemoveHandItemsForEquip(Mobile from, Layer itemLayer)
        {
            if (itemLayer == Layer.TwoHanded)
            {
                // Remove both main hand and offhand items (weapon + shield)
                Item oneHanded = from.FindItemOnLayer(Layer.OneHanded);
                if (oneHanded != null && from.Backpack != null)
                    from.Backpack.TryDropItem(from, oneHanded, false);

                Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);
                if (twoHanded != null && from.Backpack != null)
                    from.Backpack.TryDropItem(from, twoHanded, false);
            }
            else if (itemLayer == Layer.OneHanded)
            {
                // Remove any item equipped in the OneHanded layer (weapon)
                Item oneHanded = from.FindItemOnLayer(Layer.OneHanded);
                if (oneHanded != null && from.Backpack != null)
                    from.Backpack.TryDropItem(from, oneHanded, false);

                // If there is a TwoHanded weapon (not a shield or equipable light), remove it too
                Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);
                if (twoHanded != null && from.Backpack != null && !(twoHanded is BaseShield) && !(twoHanded is BaseEquipableLight))
                    from.Backpack.TryDropItem(from, twoHanded, false);
                // A shield stays equipped!
            }
        }
            */

        /*
        public override void OnDoubleClick(Mobile from)
        {
            if (this.HarvestSystem == null)
                return;

            if (this.IsChildOf(from.Backpack) || this.Parent == from)
                this.HarvestSystem.BeginHarvesting(from, this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
        */

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (this.HarvestSystem != null)
                BaseHarvestTool.AddContextMenuEntries(from, this, list, this.HarvestSystem);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write((bool)this.m_ShowUsesRemaining);

            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    {
                        this.m_ShowUsesRemaining = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_UsesRemaining = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        if (this.m_UsesRemaining < 1)
                            this.m_UsesRemaining = 150;

                        break;
                    }
            }
        }

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (!Core.AOS && defender is Mobile && (attacker.Player || attacker.Body.IsHuman) && this.Layer == Layer.TwoHanded && (attacker.Skills[SkillName.Anatomy].Value / 400.0) >= Utility.RandomDouble() && Engines.ConPVP.DuelContext.AllowSpecialAbility(attacker, "Concussion Blow", false))
            {
                StatMod mod = ((Mobile)defender).GetStatMod("Concussion");

                if (mod == null)
                {
                    ((Mobile)defender).SendMessage("You receive a concussion blow!");
                    ((Mobile)defender).AddStatMod(new StatMod(StatType.Int, "Concussion", -(((Mobile)defender).RawInt / 2), TimeSpan.FromSeconds(30.0)));

                    attacker.SendMessage("You deliver a concussion blow!");
                    attacker.PlaySound(0x11C);
                }
            }
        }
    }
}