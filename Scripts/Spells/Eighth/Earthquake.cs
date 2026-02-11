using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Spells.Eighth
{
    public class EarthquakeSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Earthquake", "In Vas Por",
            233,
            9012,
            Reagent.Bloodmoss,
            Reagent.Ginseng,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        
        public EarthquakeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
        }
        
        public override bool DelayedDamage
        {
            get
            {
                return !Core.AOS;
            }
        }

        public override void OnCast()
        {
            if (Core.UOR)
            {
                List<IDamageable> targets = new List<IDamageable>();
                Map map = this.Caster.Map;
                if (map != null)
                {
                    IPooledEnumerable eable = this.Caster.GetObjectsInRange(1 + (int)(this.Caster.Skills[SkillName.Magery].Value / 15.0));
                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;
                        if (id == null || (id is Mobile && (Mobile)id == this.Caster))
                            continue;
                        targets.Add(id);
                    }
                    eable.Free();
                }
                this.Caster.PlaySound(0x220); // Suono terremoto invariato
                for (int i = 0; i < targets.Count; ++i)
                {
                    IDamageable id = targets[i];
                    Mobile m = id as Mobile;
                    double damage = Utility.Random(43, 5); // 43-47
                    this.Caster.DoHarmful(id);
                    // Reflect AoE come ChainLightning/MeteorSwarm
                    if (m != null && SpellHelper.CheckReflectUOR(this, this.Caster, m, damage))
                        continue;
                    if (m != null && this.CheckResisted(m))
                    {
                        damage /= 2.0;
                        m.SendMessage(0x22, "You resist the spell!");
                    }
                    SpellHelper.Damage(TimeSpan.Zero, id, this.Caster, damage, 100, 0, 0, 0, 0);
                }
                targets.Clear();
                targets.TrimExcess();
                this.FinishSequence();
                return;
            }
            // Ripristino logica AOS/pre-AOS
            if (SpellHelper.CheckTown(this.Caster, this.Caster) && this.CheckSequence())
            {
                List<IDamageable> targets = new List<IDamageable>();
                Map map = this.Caster.Map;
                if (map != null)
                {
                    // Range dinamico basato su Magery: 1 + (Magery / 15)
                    // Con Magery 100 = 7 tile range
                    IPooledEnumerable eable = this.Caster.GetObjectsInRange(1 + (int)(this.Caster.Skills[SkillName.Magery].Value / 15.0));
                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;
                        if (id == null || id is Mobile && (Mobile)id == this.Caster)
                            continue;
                        if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(this.Caster, id as Mobile)) && this.Caster.CanBeHarmful(id, false))
                        {
                            if (Core.AOS && !this.Caster.InLOS(id))
                                continue;
                            targets.Add(id);
                        }
                    }
                    eable.Free();
                }
                // Suono terremoto
                this.Caster.PlaySound(0x220);
                // Loop attraverso tutti i target
                for (int i = 0; i < targets.Count; ++i)
                {
                    IDamageable id = targets[i];
                    Mobile m = id as Mobile;
                    // ⚠️ EARTHQUAKE NON È REFLECTABLE PER DESIGN OSI!
                    // Nessun CheckReflect o CheckReflectUOR
                    // Spell interruption
                    if (m != null && m.Spell != null)
                        m.Spell.OnCasterHurt();
                    double damage;
                    // AOS
                    if (Core.AOS)
                    {
                        damage = id.Hits / 2;
                        if (m == null || !m.Player)
                            damage = Math.Max(Math.Min(damage, 100), 15);
                        damage += Utility.RandomMinMax(0, 15);
                    }
                    // UOR
                    else if (Core.UOR)
                    {
                        damage = Utility.Random(43, 5); // 43-47
                        // Magic Resistance funziona (NO reflection!)
                        if (m != null && this.CheckResisted(m))
                        {
                            damage /= 2.0;
                            m.SendMessage(0x22, "You resist the spell!");
                        }
                    }
                    // Pre-AOS
                    else
                    {
                        damage = (id.Hits * 6) / 10;
                        if (m != null)
                        {
                            if (!m.Player && damage < 10)
                                damage = 10;
                            else if (damage > 75)
                                damage = 75;
                        }
                    }
                    // Applica danno (100% Physical)
                    this.Caster.DoHarmful(id);
                    SpellHelper.Damage(TimeSpan.Zero, id, this.Caster, damage, 100, 0, 0, 0, 0);
                }
                targets.Clear();
                targets.TrimExcess();
            }
            this.FinishSequence();
        }
    }
}