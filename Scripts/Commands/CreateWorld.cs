using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Commands
{
	public class CreateWorld
	{
		public enum GumpType 
		{ 
			Create, Delete, Spawn
		}

		public struct CommandEntry
		{
			public string Name;
			public string CreateCommand;
			public string DeleteCommand;
			public int checkId;
			public bool tickId;
			
			public CommandEntry(int i) : this ("Space", "", "", i, false) 
			{ 
				checkId = i; 
			}
			
			public CommandEntry(string n, string c, int i, bool t) : this (n, c, "", i, t) 
			{ 
				Name = n; CreateCommand = c; checkId = i; tickId = t;
			}
			
			public CommandEntry(string n, string d, int i) : this (n, "", d, i, true) 
			{ 
				Name = n; DeleteCommand = d; checkId = i; 
			}
			
			public CommandEntry(string n, string c, string d, int i, bool t) 
			{ 
				Name = n; CreateCommand = c; DeleteCommand = d; checkId = i; tickId = t; 
			}
		}

		public static List<CommandEntry> Commands;
		                                                                         
		public static List<CommandEntry> CreateCommands = new List<CommandEntry>(new CommandEntry[]
		{
			new CommandEntry("Moongates",       	"Moongen",							101, 	true),
			new CommandEntry("Doors",           	"DoorGen",							102, 	true),
			new CommandEntry("Signs",           	"SignGen",							103, 	true),
			new CommandEntry("Teleporters",     	"TelGen",							104, 	true),
			new CommandEntry("Decorations",     	"Decorate",         				105, 	true),
			new CommandEntry("Factions",        	"GenerateFactions",					201, 	true),
			new CommandEntry(300), //Spacer			
			new CommandEntry("Regular Spawners",	"XmlLoad Spawns/Felucca/Spawns",	301, 	true),
			new CommandEntry("Reagent Spawners",	"XmlLoad Spawns/Felucca/Reagents",	302, 	false),
			new CommandEntry("Rares Spawners",		"XmlLoad Spawns/Felucca/Rares",		303, 	false),
			new CommandEntry("Stealable Spawners",	"XmlLoad Spawns/Felucca/Stealables",304, 	false),
			new CommandEntry(400), //Spacer
			new CommandEntry("Khaldun",     	 	"GenKhaldun",       		   	   	401, 	false),
			new CommandEntry("Khaldun Spawners", 	"XmlLoad Spawns/Felucca/Khaldun", 	402, 	false),
			new CommandEntry(500), //Spacer
			new CommandEntry("SmartSpawn",			"OptimalSmartSpawning 100",		   	501, 	false),
		});

		public static List<CommandEntry> DeleteCommands = new List<CommandEntry>(new CommandEntry[]
		{
			new CommandEntry("Moongates",       	"MoonGenDelete",		101),
			new CommandEntry("Doors",           	"DoorGenDelete",		102),
			new CommandEntry("Signs",           	"SignGenDelete",		103),
			new CommandEntry("Teleporters",     	"TelGenDelete",			104),
			new CommandEntry("Decorations",     	"DecorateDelete",		105),
			new CommandEntry("Factions",        	"DeleteFactions",		201),		
			new CommandEntry("Khaldun",     	 	"DeleteKhaldun",     	301),
			new CommandEntry(400), //Spacer
			new CommandEntry("Spawners",			"XmlSpawnerWipeAll", 	401),
		});

		public static List<CommandEntry> XmlSpawnersCommand = new List<CommandEntry>(new CommandEntry[]
		{
			new CommandEntry("Regular Spawners",	"XmlLoad Spawns/Felucca/Spawns",	101, 	true),
			new CommandEntry(200), //Spacer
			new CommandEntry("Khaldun Spawners", 	"XmlLoad Spawns/Felucca/Khaldun", 	302, 	false),
			new CommandEntry(400), //Spacer
			new CommandEntry("Reagent Spawners",	"XmlLoad Spawns/Felucca/Reagents",	501, 	false),
			new CommandEntry("Rares Spawners",		"XmlLoad Spawns/Felucca/Rares",		502, 	false),
			new CommandEntry("Stealable Spawners",	"XmlLoad Spawns/Felucca/Stealables",503, 	false),
			new CommandEntry(600), //Spacer
			new CommandEntry("SmartSpawn",			"OptimalSmartSpawning 100",		   	601, 	false),
		});

		public CreateWorld()
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("Createworld", AccessLevel.Administrator, new CommandEventHandler(Create_OnCommand));
			CommandSystem.Register("DeleteWorld", AccessLevel.Administrator, new CommandEventHandler(Delete_OnCommand));
			CommandSystem.Register("AddXmlSpawners", AccessLevel.Administrator, new CommandEventHandler(AddXmlSpawners_OnCommand));
		}

		[Usage("CreateWorld")]
		[Description("Generates the world with a menu.")]
		private static void Create_OnCommand(CommandEventArgs e)
		{
			if (String.IsNullOrEmpty(e.ArgString))
			{
				Commands = new List<CommandEntry>(CreateCommands);
				
				e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Create));
			}
			else
			{
				if (e.Mobile != null)
				{
					e.Mobile.SendMessage("Usage: CreateWorld");
				}
			}
		}

		[Usage("DeleteWorld [nogump]")]
		[Description("Undoes world generation with a menu.")]
		private static void Delete_OnCommand(CommandEventArgs e)
		{
			if (String.IsNullOrEmpty(e.ArgString))
			{
				Commands = new List<CommandEntry>(DeleteCommands);
				
				e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Delete));
			}
			else
			{
				if (e.Mobile != null)
				{
					e.Mobile.SendMessage("Usage: DeleteWorld");
				}
			}
		}

		[Usage("AddXmlSpawners")]
		[Description("Adds XmlSpawners to the world with a menu.")]
		private static void AddXmlSpawners_OnCommand(CommandEventArgs e)
		{
			if (String.IsNullOrEmpty(e.ArgString))
			{
				Commands = new List<CommandEntry>(XmlSpawnersCommand);
				
				e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Spawn));
			}
			else
			{
				if (e.Mobile != null)
				{
					e.Mobile.SendMessage("Usage: AddXmlSpawners");
				}
			}
		}
		
		
		public static void DoCommands(int[] selections, GumpType type, Mobile from)
		{
			World.Broadcast(0x35, false, "The dark side is generating. This may take some time...");
			
			string prefix = Server.Commands.CommandSystem.Prefix;
			
			switch (type)
			{
				case CreateWorld.GumpType.Create:
				case CreateWorld.GumpType.Spawn:
					CommandSystem.Handle(from, prefix + "XmlSpawnerWipeAll"); //dont dupe					
					break;
				default: break;
			}
			
			foreach (int sel in selections)
			{
				foreach (CreateWorld.CommandEntry entry in CreateWorld.Commands)
				{
					if (entry.checkId == sel)
					{
						switch (type)
						{
							case CreateWorld.GumpType.Create:
								from.Say("Generating " + entry.Name);
								CommandSystem.Handle(from, prefix + entry.CreateCommand);
								break;
							case CreateWorld.GumpType.Delete:
								if (!String.IsNullOrEmpty(entry.DeleteCommand))
								{
									from.Say("Deleting " + entry.Name);
									CommandSystem.Handle(from, prefix + entry.DeleteCommand);
								}
								break;
							case CreateWorld.GumpType.Spawn:
								from.Say("Spawning " + entry.Name);
								CommandSystem.Handle(from, prefix + entry.CreateCommand);
								break;
						}
					}
				}
			}
			
			World.Broadcast(0x35, false, "Dark side generation complete.");
		}
	}
}

namespace Server.Gumps
{
	public class CreateWorldGump : Gump
	{
		private readonly CommandEventArgs m_CommandEventArgs;
		private CreateWorld.GumpType m_Type;
		
		public CreateWorldGump(CommandEventArgs e, CreateWorld.GumpType type) : base(50,50)
		{
			m_Type = type;
			m_CommandEventArgs = e;

			AddPage(1);

			int items = CreateWorld.Commands.Count;

			if (!Server.Factions.Settings.Enabled)
			{
				items--;
			}

			CustomBackground(0, 0, 240, 85 + items * 25, 9270, 2624);
			
			switch (m_Type)
			{
				case CreateWorld.GumpType.Create:
					AddLabel(50, 7, 60, "CREATE WORLD GUMP");
					break;
				case CreateWorld.GumpType.Spawn:
					AddLabel(50, 7, 60, "SPAWN WORLD GUMP");
					break;
				case CreateWorld.GumpType.Delete:
					AddLabel(50, 7, 60, "DELETE WORLD GUMP");
					break;
			}
						
			int y = 35;
			
			foreach(CreateWorld.CommandEntry entry in CreateWorld.Commands)
			{
				if (entry.Name == "Space" || (entry.Name == "Factions" && !Server.Factions.Settings.Enabled))
				{
					if (entry.Name == "Space")
					{
						y += 25;
					}

					continue;
				}
				
				AddLabel(20, y+1, 2040, entry.Name);

				AddCheck(200, y-2, 210, 211, entry.tickId, entry.checkId);
				
				y += 25;
			}

			y = 25 + (items * 25);

			AddButton(50, y+20, 247, 249, 1, GumpButtonType.Reply, 0);
			AddButton(130, y+20, 241, 243, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 1:
					CreateWorld.DoCommands(info.Switches, m_Type, from);
					break;
				default: // Closed or Cancel
					return;
			}
		}

		public void CustomBackground( int x, int y, int width, int height, int bg, int it )
		{
			AddBackground (x, y, width, height, bg);
			AddImageTiled( x+2, y+2, width-4, height-4, it );
		}
	}
}
