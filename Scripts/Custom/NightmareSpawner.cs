using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Commands
{
	public class NightmareSpawnerCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register("GenNightmareSpawner", AccessLevel.GameMaster, new CommandEventHandler(OnGenNightmareSpawner));
			CommandSystem.Register("DelNightmareSpawner", AccessLevel.GameMaster, new CommandEventHandler(DeleteNightmareSpawner));
		}

		public static void OnGenNightmareSpawner(CommandEventArgs e)
		{
			NightmareSpawner ns = new NightmareSpawner();
								
			ns.Location = new Point3D(5566, 3938, 1);
			
			ns.MoveToWorld(ns.Location, Map.Felucca);
		}

		public static void DeleteNightmareSpawner(CommandEventArgs e)
		{
			foreach (Item item in new List<Item>(World.Items.Values))
			{
				if (item is	NightmareSpawner ns)
				{
					ns.Delete();
				}
			}			
		}
	}
}

namespace Server.Mobiles
{
    public class NightmareSpawner : Item
    {
    	public static readonly int MaxSpawns = 1;	
		public static readonly int SpawnRange = 5;    	
		public static readonly int HomeRange = 10;	
		public static readonly int MinTime = 60;
		public static readonly int MaxTime = 240;

		public static List<Mobile> Registry = new List<Mobile>();
		
		public static List<Point3D> Locations = new List<Point3D>()
		{
			new Point3D(5569, 4047, -1),
			new Point3D(5578, 3968, -1),
			new Point3D(5561, 3920, 1),
			new Point3D(5516, 3912, 29),			
		};

		[Constructable]
        public NightmareSpawner() : base(3797)
        {
            Movable = false;
            Visible = false;

            Start();
        }

        public override string DefaultName { get { return "Nightmare Spawner"; } }
        			
        public static void Initialize()
        {
            Restart();
        }        

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);
						
			for (int i = 0; i < Registry.Count; i++)
			{				
				LabelTo(from, Registry[i].Location.ToString());
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			Reset();
			
			Restart();
			
			OnSingleClick(from);
		}
		
		public override void OnDelete()
		{
			Reset();
		}						

		private static void Reset()
		{
			for (int i = 0; i < Registry.Count; i++)
			{				
				if (Registry[i] is BaseCreature bc)
				{
					if (bc.Owners.Count == 0)
					{
						bc.Delete();
					}
				}
				
				Registry.Clear();
			}
		}		
		
		private void Start()
		{
			foreach (Item item in new List<Item>(World.Items.Values))
			{
				if (item is	NightmareSpawner ns)
				{
					if (ns != this)
					{
						ns.Delete();
					}
				}
			}		

			Restart();
		}	

		private static void Restart()
		{
			if (Registry.Count == 0)
			{			
				for (int i = 0; i < MaxSpawns; i++)
				{
					NightmareTimer.AddTimer(Spawn(), SpawnDelay());
				}
			}			
			else if (Registry.Count <= MaxSpawns)
			{			
				foreach (Mobile m in Registry)
				{
					if (m is BaseCreature bc)
					{
						NightmareTimer.AddTimer(bc, SpawnDelay());
					}
				}
			}
		}

		public static void Sequence(BaseCreature bc)
		{
        	Registry.Remove(bc);

			if (bc.Owners.Count == 0)
			{
				bc.Delete();
			}
        	
			if (Registry.Count < MaxSpawns)
			{			
				for (int i = Registry.Count; i < MaxSpawns; i++)
				{
					NightmareTimer.AddTimer(Spawn(), SpawnDelay());
				}
			}
		}
		
		private static BaseCreature Spawn()
		{
			Nightmare mare = new Nightmare();
			
			mare.Home = Locations[Utility.Random(Locations.Count)];
			
			mare.RangeHome = HomeRange;
						
			mare.Location = SpawnLoc(mare.Home);
			
			mare.MoveToWorld(mare.Location, Map.Felucca);

			Registry.Add(mare);

			return mare;
		}

		private static Point3D SpawnLoc(Point3D p3d)
		{
			int x, y;
			Point3D newloc;
           	Map map = Map.Felucca;
			
           	for (int i = 0; i < 20; i++)
           	{           		
           		x = p3d.X + (int)Utility.Random(SpawnRange*2) - SpawnRange;
                y = p3d.Y + (int)Utility.Random(SpawnRange*2) - SpawnRange;

           		newloc = new Point3D (x, y, p3d.Z);

    	    	if (map.CanFit( newloc, 16, false, false))
    	    	{		
    	    		return newloc;
    	    	}
           	}
			
			return p3d;
		}

		private static DateTime SpawnDelay()
		{
			return DateTime.Now + TimeSpan.FromMinutes(Utility.RandomMinMax(MinTime, MaxTime));
		}

        public NightmareSpawner(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(Registry, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            
            switch ( version )
            {
                case 0:
                {
            		Registry = reader.ReadStrongMobileList();

					break;
        		}
            }
        }
    }

	public class NightmareTimer : Timer
	{
		private static NightmareTimer _Instance;

		public static NightmareTimer Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new NightmareTimer();

				return _Instance;
			}
		}

		public List<(BaseCreature bc, DateTime dt)> Registry { get; set; } = new List<(BaseCreature, DateTime)>();

		public NightmareTimer() : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
		{
		}

		public static void AddTimer(BaseCreature bc, DateTime ds)
		{		
			Instance.Registry.Insert(0, (bc, ds));

			if (!Instance.Running)
			{
				Instance.Start();
			}
		}

		protected override void OnTick()
		{
			var registry = Instance.Registry;

			if (registry.Count > 0)
			{
				for (int i = registry.Count - 1; i >= 0; i--)
				{
					var bc = registry[i].bc;
					var end = registry[i].dt;

					if (DateTime.Now > end)
					{
						NightmareSpawner.Sequence(bc);
						
						registry.RemoveAt(i);
					}
				}
			}
		}
	}
}