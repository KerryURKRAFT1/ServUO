//modified Horse.cs

using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a horse corpse")]
    [TypeAlias("Server.Mobiles.BrownHorse", "Server.Mobiles.DirtyHorse", "Server.Mobiles.GrayHorse", "Server.Mobiles.TanHorse")]
    public class KillableGuardMount : BaseMount
    {
		private static readonly int[] m_IDs = new int[] { 0xC8, 0x3E9F, 0xE2, 0x3EA0, 0xE4, 0x3EA1, 0xCC, 0x3EA2 };
	    	    	
        [Constructable]        public KillableGuardMount() : base("a guard's horse", 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            int random = Utility.Random(4);

            Body = m_IDs[random * 2];
            ItemID = m_IDs[random * 2 + 1];
            BaseSoundID = 0xA8;

            SetStr(32, 98);
            SetDex(66, 75);
            SetInt(6, 10);

            SetHits(45, 60);
            SetMana(0);

            SetDamage(4, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);

            SetSkill(SkillName.MagicResist, 40.1, 50.0);
            SetSkill(SkillName.Tactics, 49.3, 64.0);  
            SetSkill(SkillName.Wrestling, 49.3, 64.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 35.1;
        }
        
		public override void OnAfterDelete()
		{
			if (m_MountTimer != null)
			{
				m_MountTimer.Stop();
				
				m_MountTimer = null;
			}

			base.OnAfterDelete();
		}

        public override int Meat { get { return 3; }}

        public override int Hides { get { return 10; }}

        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }}

		private Timer m_MountTimer;

		private DateTime m_End;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime IdleTime { get { return m_End; } set { m_End = value; MountIdleTimer(); }}

		public void MountIdleTimer()
    	{
            if (m_End > DateTime.UtcNow)
            {
	           	m_MountTimer = new KillableMountTimer(this, m_End);
				
				m_MountTimer.Start();
            }
    	}

        public KillableGuardMount(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			//version 0
            writer.Write(m_End);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            
            switch (version)
            {
            	case 0: m_End = reader.ReadDateTime(); break;
            }
            
            MountIdleTimer();
        }

    	public class KillableMountTimer : Timer
		{
			private readonly KillableGuardMount m_Horse;
			
			private readonly DateTime m_End;
						
			public KillableMountTimer(KillableGuardMount horse, DateTime end) : base(TimeSpan.FromSeconds(15.0), TimeSpan.FromSeconds(15.0))
			{
				m_Horse = horse;
				
				m_End = end;
			}

			protected override void OnTick()
			{
	            if (m_Horse.Deleted || m_Horse.Controlled || m_Horse.ControlMaster != null)
	            {
					Stop();
				}
	            else if (m_End < DateTime.UtcNow)
				{
					Effects.SendLocationParticles(EffectItem.Create(m_Horse.Location, m_Horse.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
				
					m_Horse.PlaySound(0xA8);

					m_Horse.Delete();
					
					Stop();
				}
			}
        }
	}
}