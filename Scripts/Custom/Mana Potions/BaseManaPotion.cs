using System;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseManaPotion : BasePotion
    {
        public BaseManaPotion(PotionEffect effect) : base(0xF0C, effect)
        {
        }

        public abstract int MinMana { get; }
        public abstract int MaxMana { get; }
        public abstract double Delay { get; }

        public void DoMana(Mobile from)
        {
            int min = Scale(from, this.MinMana);
            int max = Scale(from, this.MaxMana);

            int amount = Utility.RandomMinMax(min, max);

            if (!from.Alive || from.IsDeadBondedPet)
			{
            	return;
			}

			if (!from.Region.OnHeal(from, ref amount)) //use same region control as Heal
			{
				return;
			}

			if ((from.Mana + amount) > from.ManaMax)
			{
				amount = from.ManaMax - from.Mana;
			}

			from.Mana += amount;

			from.SendMessage(String.Format("Some mana is healed ({0})", amount));
        }

        public override void Drink(Mobile from)
        {
            if (from.Mana < from.ManaMax)
            {
                if (from.Poisoned || MortalStrike.IsWounded(from))
                {
                    from.SendMessage(0x22, "You cannot heal mana in your current state.");
                }
                else
                {
                    if (from.BeginAction(typeof(BaseManaPotion)))
                    {
                        DoMana(from);
                        PlayDrinkEffect(from);
                        Consume();

                        Timer.DelayCall(TimeSpan.FromSeconds(this.Delay), new TimerStateCallback(ReleaseManaLock), from);
                    }
                    else
                    {
                        from.PrivateOverheadMessage(MessageType.Regular, 0x22, false, "You must wait 10 seconds before using another mana potion", from.NetState);
                    }
                }
            }
            else
            {
                from.SendMessage("You decide against drinking this potion, as you are already at full mana"); // You decide against drinking this potion, as you are already at full Manath.
            }
        }

        private static void ReleaseManaLock(object state)
        {
            ((Mobile)state).EndAction(typeof(BaseManaPotion));
        }

        public BaseManaPotion(Serial serial) : base(serial)
        {
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
    }
}
