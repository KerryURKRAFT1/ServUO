using System;
using System.IO;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
	public class StaticContainer : LockableContainer, ILockpickable
	{
		#region Getters and Setters
		private int m_Count;
		private TimeSpan m_MinDelay;
		private TimeSpan m_MaxDelay;

		private bool m_Locked;
		private bool m_Trapped;
		private bool m_Steal;
		
		private List<Item> m_Items;
		private DateTime m_End;
		private bool m_Unlocked;
		private List<string> m_ItemsName;
		private bool m_Running;
		
		private InternalTimer m_Timer;

		[CommandProperty( AccessLevel.Administrator )]
		public int Count{ get { return m_Count; } set { m_Count = value; InvalidateProperties(); }}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Unlocked{ get { return m_Unlocked; } set { m_Unlocked = value; InvalidateProperties(); }}
		
		[CommandProperty( AccessLevel.Administrator )]
		public TimeSpan MinDelay{ get { return m_MinDelay; } set { m_MinDelay = value; InvalidateProperties(); }}
		
		[CommandProperty( AccessLevel.Administrator )]
		public TimeSpan MaxDelay{ get { return m_MaxDelay; } set { m_MaxDelay = value; InvalidateProperties(); }}
		
		[CommandProperty( AccessLevel.Administrator )]
		public bool isLocked{ get { return m_Locked; } set { m_Locked = value; InvalidateProperties(); }}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Trapped{ get { return m_Trapped; } set { m_Trapped = value; InvalidateProperties(); }}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Steal{ get { return m_Steal; } set { m_Steal = value; InvalidateProperties(); }}

		public List<string> ItemsName
		{
			get { return m_ItemsName; }
			set
			{
				m_ItemsName = value;

				if ( m_ItemsName.Count < 1 )
					Stop();

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Running
		{
			get { return m_Running; }
			set
			{
				if ( value )
					Start();
				else
					Stop();

				InvalidateProperties();
			}
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public TimeSpan NextSpawn
		{
			get
			{
				if ( m_Running )
					return m_End - DateTime.Now;
				else
					return TimeSpan.FromSeconds( 0 );
			}
			set
			{
				Start();
				DoTimer( value );
			}
		}
		#endregion
		
		#region Constructables
		[Constructable]
		public StaticContainer( int itemid, int amount, int minDelay, int maxDelay, List<string> itemsName, bool locked, bool trapped, bool steal ) : base( itemid )
		{
			InitSpawn( amount, TimeSpan.FromMinutes( minDelay ), TimeSpan.FromMinutes( maxDelay ), itemsName, locked, trapped, steal );
		}
		
		[Constructable]
		public StaticContainer( int itemid, int amount, List<string> itemsName, bool locked, bool trapped, bool steal ) : base( itemid )
		{
			InitSpawn( amount, TimeSpan.FromMinutes( 20 ), TimeSpan.FromMinutes( 60 ), itemsName, locked, trapped, steal );
		}

		[Constructable]
		public StaticContainer( int itemid ) : base( itemid )
		{
			List<string> itemsName = new List<string>();
			InitSpawn( 1, TimeSpan.FromMinutes( 20 ), TimeSpan.FromMinutes( 60 ), itemsName, false, false, false );
		}
		#endregion
		
		#region overrides
		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			LabelTo(from, "({0} items, {1} stones)", TotalItems, TotalWeight);
		}
		#endregion
		
		#region Functions		
		private bool isBarrel()
		{
			return ( ItemID == 0xE77 );
		}

		private bool isCrate()
		{
			return( ItemID == 0x9A9 || ItemID == 0xE7E || ItemID == 0xE3F || ItemID == 0xE3E || ItemID == 0xE3C || ItemID == 0xE3D );
		}

		private bool isMetalBox()
		{
			return( ItemID == 0x9A9 || ItemID == 0xE80 );
		}

		private bool isWoodenChest()
		{
			return( ItemID == 0xe42 || ItemID == 0xe43 || ItemID == 0x280B || ItemID == 0x280C || ItemID == 0x280D || ItemID == 0x280E || ItemID == 0x280F || ItemID == 0x28010 );
		}

		private bool isMetalChest()
		{
			return( ItemID == 0xE7C || ItemID == 0x9AB || ItemID == 0xE40 || ItemID == 0xE41);
		}

		public void InitSpawn( int amount, TimeSpan minDelay, TimeSpan maxDelay, List<string> itemsName, bool locked, bool trapped, bool steal )
		{
			Movable = false;
			LiftOverride = true;
			m_Running = true;
			m_Unlocked = false;
			m_MinDelay = minDelay;
			m_MaxDelay = maxDelay;
			m_Count = amount;
			m_ItemsName = itemsName;
			m_Locked = locked;
			m_Trapped = trapped;
			m_Steal = steal;

			Locked = m_Locked;
			
			m_Items = new List<Item>();
			DoTimer( TimeSpan.FromSeconds( 1 ) ); //spawn in 1 sec

			if( Locked )
			{
				if( isBarrel() )
					Locked = false;
				else
				{
					if( isWoodenChest() )
						RequiredSkill =( Utility.RandomMinMax( 40, 72 ));
					else if( isMetalChest() )
						RequiredSkill =( Utility.RandomMinMax( 50, 84 ));
					else if( isMetalBox() )
						RequiredSkill =( Utility.RandomMinMax( 60, 92 ));
					else
						RequiredSkill = Utility.RandomMinMax( 30, 52 );
					
					LockLevel = RequiredSkill - Utility.RandomMinMax( 1, 10 );
				}
			}

			TrapType = TrapType.None;

			if( m_Trapped )
			{
				if( isBarrel() )
					m_Trapped = false;
				else
				{
					int num = 3; //no explosion traps here

					if( isCrate() )
						num = 1;
					else if( isWoodenChest() || isMetalChest() )
						num =2;

					switch( Utility.Random(num) )
					{
						case 0:
							if(Utility.RandomBool() )
								TrapType = TrapType.DartTrap;
							else
								TrapType = TrapType.MagicTrap;
							break;

						case 1:
							TrapType = TrapType.MagicTrap;
							break;

						case 2:
							TrapType = TrapType.PoisonTrap;
							break;

						case 3:
							TrapType = TrapType.ExplosionTrap;
							break;
					}

					TrapLevel = num;
				}
			}
		}

		public void Start()
		{
			if ( !m_Running )
			{
				if ( m_ItemsName.Count > 0 )
				{
					m_Running = true;
					DoTimer();
				}
			}
		}
		
		public void Stop()
		{
			if ( m_Running )
			{
				m_Timer.Stop();
				m_Running = false;
			}
		}
		
		public void Defrag()
		{
			bool removed = false;
			
			List<Item> defragger = new List<Item>(m_Items);

			foreach (Item item in defragger)
			{
				if(item.Deleted)
				{
					m_Items.Remove( item );
					removed = true;
				}
				else if (item.Parent != this )
				{
					m_Items.Remove( item );
					removed = true;
				}
			}

			if ( removed )
			{
				InvalidateProperties();
			}
		}
		
		public void OnTick()
		{
			DoTimer();
			Spawn();
			
			if(!Unlocked)
			{
				Locked = m_Locked;

				if( Locked )
				{
					if( isBarrel() )
						Locked = false;
					else
					{
						if( isWoodenChest() )
							RequiredSkill =( Utility.RandomMinMax( 40,72 ));
						else if( isMetalChest() )
							RequiredSkill =( Utility.RandomMinMax( 50,84 ));
						else if( isMetalBox() )
							RequiredSkill =( Utility.RandomMinMax( 60,92 ));
						else
							RequiredSkill = Utility.RandomMinMax( 30, 52 );
						
						LockLevel = RequiredSkill - Utility.RandomMinMax( 1, 10 );
					}
				}
				
				TrapType = TrapType.None;

				if( m_Trapped )
				{
					if( isBarrel() )
						m_Trapped = false;
					else
					{
						int num = 3; //no explosion traps here

						if( isCrate() )
							num = 1;
						else if( isWoodenChest() || isMetalChest() )
							num =2;

						switch( Utility.Random(num) )
						{
							case 0:
								if(Utility.RandomBool() )
									TrapType = TrapType.DartTrap;
								else
									TrapType = TrapType.MagicTrap;
								break;

							case 1:
								TrapType = TrapType.MagicTrap;
								break;

							case 2:
								TrapType = TrapType.PoisonTrap;
								break;

							case 3:
								TrapType = TrapType.ExplosionTrap;
								break;
						}

						TrapLevel = num;
					}
				}
			}
		}
		
		public void Respawn()
		{
			RemoveItems();
			for ( int i = 0; i < m_Count; i++ )
				Spawn();

			if(!Unlocked)
			{
				Locked = m_Locked;

				if( Locked )
				{
					if( isBarrel() )
						Locked = false;
					else
					{
						if( isWoodenChest() )
							RequiredSkill =( Utility.RandomMinMax( 40,72 ));
						else if( isMetalChest() )
							RequiredSkill =( Utility.RandomMinMax( 50,84 ));
						else if( isMetalBox() )
							RequiredSkill =( Utility.RandomMinMax( 60,92 ));
						else
							RequiredSkill = Utility.RandomMinMax( 30, 52 );
						
						LockLevel = RequiredSkill - Utility.RandomMinMax( 1, 10 );
					}
				}

				TrapType = TrapType.None;
				
				if( m_Trapped )
				{
					if( isBarrel() )
						m_Trapped = false;
					else
					{
						int num = 3; //no explosion traps here

						if( isCrate() )
							num = 1;
						else if( isWoodenChest() || isMetalChest() )
							num =2;

						switch( Utility.Random(num) )
						{
							case 0:
								if(Utility.RandomBool() )
									TrapType = TrapType.DartTrap;
								else
									TrapType = TrapType.MagicTrap;
								break;

							case 1:
								TrapType = TrapType.MagicTrap;
								break;

							case 2:
								TrapType = TrapType.PoisonTrap;
								break;

							case 3:
								TrapType = TrapType.ExplosionTrap;
								break;
						}

						TrapLevel = num;
					}
				}
			}
		}
		
		public void Spawn()
		{
			if ( m_ItemsName.Count > 0 )
			{
				Spawn( Utility.Random( m_ItemsName.Count ) ); //spawn on of them index
			}
		}

		public void Spawn( string itemName )
		{
			for ( int i = 0; i < m_ItemsName.Count; i++ )
			{
				if ( (string)m_ItemsName[i] == itemName )
				{
					Spawn( i );
					break;
				}
			}
		}
		
		public void Spawn( int index )
		{
			if ( m_ItemsName.Count == 0 || index >= m_ItemsName.Count )
				return;

			Defrag();

			//limit already at
			if ( m_Items.Count >= m_Count )
				return;

			Type type = SpawnerType.GetType( (string)m_ItemsName[index] );

			if ( type != null )
			{
				try
				{
					object o = Activator.CreateInstance( type );

					if ( o is Item item)
					{
						m_Items.Add( item );

						if( m_Steal )
						{
							item.Movable = false;
							
							ItemFlags.SetStealable( item, true );
						}
						
						InvalidateProperties();

						this.DropItem( item );
					}
				}
				catch{}
			}
		}
		
		public void DoTimer()
		{
			if ( !m_Running )
				return;

			int minSeconds = (int)m_MinDelay.TotalSeconds;
			int maxSeconds = (int)m_MaxDelay.TotalSeconds;

			TimeSpan delay = TimeSpan.FromSeconds( Utility.RandomMinMax( minSeconds, maxSeconds ) );
			DoTimer( delay );
		}
		
		public void DoTimer( TimeSpan delay )
		{
			if ( !m_Running )
				return;

			m_End = DateTime.Now + delay;

			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = new InternalTimer( this, delay );
			m_Timer.Start();
		}
		
		public int CountItems( string itemName )
		{
			Defrag();

			int count = 0;

			for ( int i = 0; i < m_Items.Count; ++i )
				if ( Insensitive.Equals( itemName, m_Items[i].GetType().Name ) )
					++count;

			return count;
		}
		
		public void RemoveItems( string itemName )
		{
			Console.WriteLine( "defrag from removeitems" );
			Defrag();

			itemName = itemName.ToLower();

			for ( int i = 0; i < m_Items.Count; ++i )
			{
				Item item = m_Items[i];

				if ( Insensitive.Equals( itemName, item.GetType().Name ) )
				{
					item.Delete();
				}
			}

			InvalidateProperties();
		}
		
		public void RemoveItems()
		{
			Defrag();
			
			for ( int i = 0; i < m_Items.Count; ++i )
			{
				m_Items[i].Delete();
			}

			InvalidateProperties();
		}
		
		public override void OnDelete()
		{
			base.OnDelete();
			RemoveItems();

			if ( m_Timer != null )
				m_Timer.Stop();
		}
		#endregion

		#region Serialize
		public StaticContainer( Serial serial ) : base( serial )
		{
		}

		public StaticContainer( int itemid, int amount, TimeSpan minDelay, TimeSpan maxDelay, List<string> itemsName, bool locked, bool trapped, bool steal ) : base( itemid )
		{
			InitSpawn( amount, minDelay, maxDelay, itemsName, locked, trapped, steal );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			//version 2
			writer.Write( m_Locked );
			writer.Write( m_Trapped );
			writer.Write( m_Steal );

			//version 1
			writer.Write( m_Unlocked );

			//version 0
			writer.Write( m_MinDelay );
			writer.Write( m_MaxDelay );
			writer.Write( m_Count );
			writer.Write( m_Running );
			
			if ( m_Running )
			{
				writer.Write( m_End - DateTime.Now );
			}

			writer.Write( m_Items );

			writer.Write( m_ItemsName.Count );

			for ( int i = 0; i < m_ItemsName.Count; ++i )
			{
				writer.Write( m_ItemsName[i] );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			switch (version)
			{
				case 2:
					{
						m_Locked = reader.ReadBool();
						m_Trapped = reader.ReadBool();
						m_Steal = reader.ReadBool();
						goto case 1;
					}
				case 1:
					{
						m_Unlocked = reader.ReadBool();
						goto case 0;
					}
				case 0:
					{
						m_MinDelay = reader.ReadTimeSpan();
						m_MaxDelay = reader.ReadTimeSpan();
						m_Count = reader.ReadInt();
						m_Running = reader.ReadBool();

						if ( m_Running )
						{		
							DoTimer( reader.ReadTimeSpan() );							
						}
						
						m_Items = reader.ReadStrongItemList();
						
						m_ItemsName = new List<string>();
			
						int items = reader.ReadInt();
			
						for ( int i = 0; i < items; ++i )
						{
							m_ItemsName.Add( reader.ReadString() );
						}

						break;
					}
			}
						
			if( m_Unlocked )
			{
				Locked = false;
				TrapType=TrapType.None;
			}
						
			if( LiftOverride == false )
			{
				LiftOverride = true;
			}
		}
		#endregion
		
		#region Timers
		private class InternalTimer : Timer
		{
			private StaticContainer m_Spawner;

			public InternalTimer( StaticContainer spawner, TimeSpan delay ) : base( delay )
			{
				Priority = TimerPriority.OneSecond;
				m_Spawner = spawner;
			}

			protected override void OnTick()
			{
				if ( m_Spawner != null )
					if ( !m_Spawner.Deleted )
						m_Spawner.Respawn();
			}
		}
		#endregion
	}
}
