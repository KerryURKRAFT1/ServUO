using System;
using Server.Targets;

namespace Server.Items
{
    public abstract class BaseKnife : BaseMeleeWeapon
    {
        public BaseKnife(int itemID)
            : base(itemID)
        {
        }

        public BaseKnife(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound
        {
            get
            {
                return 0x23B;
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
                return WeaponType.Slashing;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Slash1H;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.FindItemOnLayer(this.Layer) == this)
            {
                from.SendLocalizedMessage(1010018); // What do you want to use this item on?
                from.Target = new BladedItemTarget(this);
            }
            else
            {
                ClickToEquip.OnDoubleClick(from, this);
            }
        }


        /*
                public override void OnDoubleClick(Mobile from)
        {
            // UOR behavior
            if (Core.UOR)
            {
                // If the item is inside the backpack or inside any container within the backpack
                if (IsAccessibleFromBackpack(from.Backpack))
                {
                    RemoveHandItemsForEquip(from, this.Layer);

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
                    if (from.Backpack != null && from.Backpack.TryDropItem(from, this, false))
                    {
                        RemoveHandItemsForEquip(from, this.Layer);

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
                // If already equipped, start targeting
                else if (from.FindItemOnLayer(Layer.OneHanded) == this || from.FindItemOnLayer(Layer.TwoHanded) == this)
                {
                    from.SendLocalizedMessage(1010018); // What do you want to use this item on?
                    from.Target = new BladedItemTarget(this);
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
                // Default behavior for non-UOR cores
                from.SendLocalizedMessage(1010018); // What do you want to use this item on?
                from.Target = new BladedItemTarget(this);
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
            from.SendLocalizedMessage(1010018); // What do you want to use this item on?

            from.Target = new BladedItemTarget(this);
        }
        */

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (!Core.AOS && defender is Mobile && this.Poison != null && this.PoisonCharges > 0)
            {
                --this.PoisonCharges;

                if (Utility.RandomDouble() >= 0.5) // 50% chance to poison
                    ((Mobile)defender).ApplyPoison(attacker, this.Poison);
            }
        }
    }
}