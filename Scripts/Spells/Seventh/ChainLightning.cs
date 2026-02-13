using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;
using Server.Network;

namespace Server.Spells.Seventh
{
    public class ChainLightningSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Chain Lightning", "Vas Ort Grav",
            209,
            9022,
            false,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        public ChainLightningSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Seventh;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return true;
            }
        }

        public override bool Cast()
        {
        	if (this.Caster.Mana > (Mana = ScaleMana(GetMana())))
        	{
        		return (this.Caster.Target = new InternalTarget(this)) != null;
        	}

        	this.Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana
        	
        	return false;
        }

        public override void OnCast()
        {
        	Target ((IPoint3D)ObjectTargeted);
        }

        public void Target(IPoint3D p)
        {
            if (Core.UOR)
            {
                SpellHelper.Turn(this.Caster, p);
                if (p is Item)
                    p = ((Item)p).GetWorldLocation();
                List<IDamageable> targets = new List<IDamageable>();
                Map map = this.Caster.Map;
                if (map != null)
                {
                    IPooledEnumerable eable = map.GetObjectsInRange(new Point3D(p), 2);
                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;
                        if (id == null || (id is Mobile && (Mobile)id == this.Caster))
                            continue;
                        targets.Add(id);
                    }
                    eable.Free();
                }
                double damage = 0;
                Effects.PlaySound(p, this.Caster.Map, 0x29);
                bool reflected = false;
                for (int i = 0; i < targets.Count; ++i)
                {
                    IDamageable id = targets[i];
                    Mobile m = id as Mobile;
                    damage = Utility.Random(43, 13); // 43-56
                    damage /= targets.Count > 0 ? targets.Count : 1;
                    this.Caster.DoHarmful(id);
                    Effects.SendBoltEffect(id, true, 0);
                    if (m != null && SpellHelper.CheckReflectUOR(this, this.Caster, m, damage))
                    {
                        reflected = true;
                        continue;
                    }
                    if (m != null && this.CheckResisted(m))
                    {
                        damage *= 0.5;
                        m.SendMessage(0x22, "You resist the spell!");
                    }
                    if (m != null)
                        damage *= this.GetDamageScalar(m);
                    SpellHelper.Damage(this, id, damage, 0, 0, 0, 0, 100);
                }
                targets.Clear();
                targets.TrimExcess();
                this.FinishSequence();
                return;
            }
            // --- LOGICA ORIGINALE (AOS e altro) ---
            if (!this.Caster.CanSee(p))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
                this.FinishSequence();
                return;
            }
            if (!SpellHelper.CheckTown(p, this.Caster) || !this.CheckSequence())
            {
                this.FinishSequence();
                return;
            }
            SpellHelper.Turn(this.Caster, p);
            if (p is Item)
                p = ((Item)p).GetWorldLocation();
            List<IDamageable> targetsAos = new List<IDamageable>();
            Map mapAos = this.Caster.Map;
            if (mapAos != null)
            {
                IPooledEnumerable eable = mapAos.GetObjectsInRange(new Point3D(p), 2);
                foreach (object o in eable)
                {
                    IDamageable id = o as IDamageable;
                    if (id == null || (Core.AOS && id is Mobile && (Mobile)id == this.Caster))
                        continue;
                    if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(this.Caster, id as Mobile)) && this.Caster.CanBeHarmful(id, false))
                    {
                        if (Core.AOS && !this.Caster.InLOS(id))
                            continue;
                        targetsAos.Add(id);
                    }
                }
                eable.Free();
            }
            double damageAos;
            if (targetsAos.Count > 0)
            {
                for (int i = 0; i < targetsAos.Count; ++i)
                {
                    IDamageable id = targetsAos[i];
                    Mobile m = id as Mobile;
                    if (Core.AOS)
                        damageAos = this.GetNewAosDamage(51, 1, 5, id is PlayerMobile, id);
                    else
                        damageAos = Utility.Random(27, 22);
                    if (Core.AOS && targetsAos.Count > 2)
                        damageAos = (damageAos * 2) / targetsAos.Count;
                    else if (!Core.AOS)
                        damageAos /= targetsAos.Count;
                    if (!Core.AOS && m != null && this.CheckResisted(m))
                    {
                        damageAos *= 0.5;
                    }
                    if(m != null)
                        damageAos *= this.GetDamageScalar(m);
                    this.Caster.DoHarmful(id);
                    SpellHelper.Damage(this, id, damageAos, 0, 0, 0, 0, 100);
                    Effects.SendBoltEffect(id, true, 0);
                }
            }
            targetsAos.Clear();
            targetsAos.TrimExcess();
            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly ChainLightningSpell m_Owner;
            public InternalTarget(ChainLightningSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                {
                    if (Core.UOR)
                    {
                        // In UOR non chiamare StartSequence, targeting libero
                        m_Owner.Target((IPoint3D)o);
                    }
                    else
                    {
                        if (!this.m_Owner.StartSequence(o))
                        {
                            this.m_Owner.FinishSequence();
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1005213); // You can't do that
                }
            }
            protected override void OnTargetOutOfLOS(Mobile from, object o)
            {
                from.Target = new InternalTarget(m_Owner);
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237); // Target can not be seen.
            }
        }
    }
}