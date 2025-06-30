using System;
using Server;
using Server.Spells;

namespace Server.Custom
{
    public class SphereStyleHelper
    {
        public delegate void FinalEffectDelegate();
        public delegate bool FinalChecksDelegate();

        public static void DoSphereStyleCast(
            Spell spell,
            object target,
            TimeSpan delay,
            FinalEffectDelegate finalEffect,
            FinalChecksDelegate finalChecks)
        {
            // If NextSpellTime is a long, set it using Core.TickCount
            if (spell.Caster != null)
            {
                spell.Caster.NextSpellTime = Core.TickCount + (long)(delay.TotalSeconds * 1000);
            }

            Timer.DelayCall(delay, new TimerStateCallback(SphereDelayCallback),
                new SphereDelayState(spell, target, finalEffect, finalChecks));
        }

        private class SphereDelayState
        {
            public Spell Spell;
            public object Target;
            public FinalEffectDelegate FinalEffect;
            public FinalChecksDelegate FinalChecks;

            public SphereDelayState(Spell spell, object target, FinalEffectDelegate finalEffect, FinalChecksDelegate finalChecks)
            {
                this.Spell = spell;
                this.Target = target;
                this.FinalEffect = finalEffect;
                this.FinalChecks = finalChecks;
            }
        }

        private static void SphereDelayCallback(object state)
        {
            SphereDelayState s = (SphereDelayState)state;

            if (s.Spell.Caster == null || s.Spell.Caster.Deleted || !s.Spell.Caster.Alive || s.Target == null)
            {
                s.Spell.DoFizzle();
                s.Spell.FinishSequence();
                return;
            }

            if (s.FinalChecks != null && !s.FinalChecks())
            {
                s.Spell.DoFizzle();
                s.Spell.FinishSequence();
                return;
            }

            if (s.FinalEffect != null)
            {
                s.FinalEffect();
            }
        }
    }
}