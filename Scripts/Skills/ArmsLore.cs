using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.SkillHandlers
{
    public class ArmsLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ArmsLore].Callback = new SkillUseCallback(OnUse);
        }

    	public static TimeSpan OnUse(Mobile m)
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
        	
        	return TimeSpan.Zero;
        }

        public static bool TriggerSkill(Mobile m)
        {
            m.Target = new InternalTarget();

            m.SendLocalizedMessage(500349); // What item do you wish to get information about?

            return true;
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget() : base(2, false, TargetFlags.None)
            {
                this.AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
				if (targeted is BaseWeapon || targeted is BaseArmor || targeted is SwampDragon && ((SwampDragon)targeted).HasBarding)
				{
					new SkillTimer(from, targeted, SkillRegistry.Delay).Start();
					
					return;
				}
							
                from.SendLocalizedMessage(1046439); // That is not a valid target.
              	
                SkillRegistry.Remove(from);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                SkillRegistry.Remove(from);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1076203); // Target out of range.	
				SkillRegistry.Remove(from);
            }

	        protected override void OnTargetOutOfLOS(Mobile from, object o)
	        {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237);// Target can not be seen.
                SkillRegistry.Remove(from);
	        }
        }

        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly object m_Targ;

            public SkillTimer(Mobile owner, object targ, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
                if (m_Targ is BaseWeapon)
                {
                    if (m_Owner.CheckTargetSkill(SkillName.ArmsLore, m_Targ, 0, 100))
                    {
                        BaseWeapon weap = (BaseWeapon)m_Targ;

                        if (weap.MaxHitPoints != 0)
                        {
                            int hp = (int)((weap.HitPoints / (double)weap.MaxHitPoints) * 10);

                            if (hp < 0)
                                hp = 0;
                            else if (hp > 9)
                                hp = 9;

                            m_Owner.SendLocalizedMessage(1038285 + hp);
                        }

                        int damage = (weap.MaxDamage + weap.MinDamage) / 2;
                        int hand = (weap.Layer == Layer.OneHanded ? 0 : 1);

                        if (damage < 3)
                            damage = 0;
                        else if ( damage < 6 )
	                        damage = 1;
                        else if ( damage < 11 )
	                        damage = 2;
                        else if ( damage < 16 )
	                        damage = 3;
                        else if ( damage < 21 )
	                        damage = 4;
                        else if ( damage < 26 )
	                        damage = 5;
                        else
	                        damage = 6;

                        WeaponType type = weap.Type;

                        if (type == WeaponType.Ranged)
                            m_Owner.SendLocalizedMessage(1038224 + (damage * 9));
                        else if (type == WeaponType.Piercing)
                            m_Owner.SendLocalizedMessage(1038218 + hand + (damage * 9));
                        else if (type == WeaponType.Slashing)
                            m_Owner.SendLocalizedMessage(1038220 + hand + (damage * 9));
                        else if (type == WeaponType.Bashing)
                            m_Owner.SendLocalizedMessage(1038222 + hand + (damage * 9));
                        else
                            m_Owner.SendLocalizedMessage(1038216 + hand + (damage * 9));

                        if (weap.Poison != null && weap.PoisonCharges > 0)
                            m_Owner.SendLocalizedMessage(1038284); // It appears to have poison smeared on it.
                    }
                    else
                    {
                        m_Owner.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else if (m_Targ is BaseArmor)
                {
                    if (m_Owner.CheckTargetSkill(SkillName.ArmsLore, m_Targ, 0, 100))
                    {
                        BaseArmor arm = (BaseArmor)m_Targ;

                        if (arm.MaxHitPoints != 0)
                        {
                            int hp = (int)((arm.HitPoints / (double)arm.MaxHitPoints) * 10);

                            if (hp < 0)
                                hp = 0;
                            else if (hp > 9)
                                hp = 9;

                            m_Owner.SendLocalizedMessage(1038285 + hp);
                        }

                        if ( arm.ArmorRating < 1 )
	                        m_Owner.SendLocalizedMessage( 1038295 ); // This armor offers no defense against attackers.
                        else if ( arm.ArmorRating < 6 )
	                        m_Owner.SendLocalizedMessage( 1038296 ); // This armor provides almost no protection.
                        else if ( arm.ArmorRating < 11 )
	                        m_Owner.SendLocalizedMessage( 1038297 ); // This armor provides very little protection.
                        else if ( arm.ArmorRating < 16 )
	                        m_Owner.SendLocalizedMessage( 1038298 ); // This armor offers some protection against blows.
                        else if ( arm.ArmorRating < 21 )
	                        m_Owner.SendLocalizedMessage( 1038299 ); // This armor serves as sturdy protection.
                        else if ( arm.ArmorRating < 26 )
	                        m_Owner.SendLocalizedMessage( 1038300 ); // This armor is a superior defense against attack.
                        else if ( arm.ArmorRating < 31 )
	                        m_Owner.SendLocalizedMessage( 1038301 ); // This armor offers excellent protection.
                        else
	                        m_Owner.SendLocalizedMessage( 1038302 ); // This armor is superbly crafted to provide maximum protection.
                    }
                    else
                    {
                        m_Owner.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else if (m_Targ is SwampDragon && ((SwampDragon)m_Targ).HasBarding)
                {
                    SwampDragon pet = (SwampDragon)m_Targ;

                    if (m_Owner.CheckTargetSkill(SkillName.ArmsLore, m_Targ, 0, 100))
                    {
                        int perc = (4 * pet.BardingHP) / pet.BardingMaxHP;

                        if (perc < 0)
                            perc = 0;
                        else if (perc > 4)
                            perc = 4;

                        pet.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1053021 - perc, m_Owner.NetState);
                    }
                    else
                    {
                        m_Owner.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else
                {
                    m_Owner.SendLocalizedMessage(500352); // This is neither weapon nor armor.
                }

                SkillRegistry.Remove(m_Owner);
			}
		}
    }
}