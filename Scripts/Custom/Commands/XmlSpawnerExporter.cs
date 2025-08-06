using System;
using Server.Commands;
using Server.Mobiles;
using System.IO;
using System.Collections.Generic;
using Server.Regions;
using Server.Engines.XmlSpawner2;
using Server.Engines.Quests;

namespace Server.Misc
{
	class XmlSpawnerExporter
	{
		public List<(string maps, string regs, XmlSpawner spawners)> SpawnerEntry = new List<(string, string, XmlSpawner)>();
				
		private static bool m_Enabled = false; //true: On WorldSave
		
		public static string RootPath = XmlSpawner.XmlSpawnDir;
		
		private static List<SpawnerEntries> m_SpawnList;

		public static void Initialize()
		{
			if( m_Enabled )
			{
				EventSink.AfterWorldSave += new AfterWorldSaveEventHandler( WorldSave_OnEvent );
			}

			CommandSystem.Register( "XmlSpawnerExporter", AccessLevel.Developer, new CommandEventHandler( XmlSpawnerExporter_OnCommand ) );
    		CommandSystem.Register( "XSE", AccessLevel.Developer, new CommandEventHandler( XmlSpawnerExporter_OnCommand ) );

            if (!Directory.Exists(RootPath))
            {
            	Directory.CreateDirectory(RootPath);
            }
		}

		[Usage( "WorldBackup" )]
		[Description( "Performs World Backup." )]
		private static void XmlSpawnerExporter_OnCommand( CommandEventArgs e )
		{
			WorldSave( e.Mobile );
		}

		private static void WorldSave_OnEvent( AfterWorldSaveEventArgs e )
		{
			WorldSave( null );
		}
		
		private static void WorldSave( Mobile m )
		{
            m_SpawnList = new List<SpawnerEntries>();

            string date = DateTime.Now.ToString ("[yyyy-MM-dd] [HH-mm-ss]");

			string datefolder = Path.Combine (RootPath, date);

			if (!Directory.Exists (datefolder))
			{
            	Directory.CreateDirectory (datefolder);
            }
								
            CollectSpawns ();
            
            ExportSpawns (datefolder);

			if ( m != null )
			{
				m.SendMessage( "World Backup Complete" );
			}
		}

 		public static void CollectSpawns() //crappy method - nevermind
		{
            List<XmlSpawner> xml = new List<XmlSpawner>();
            List<Item> items = new List<Item>(World.Items.Values);
 				
            for (int i = 0; i < items.Count; i++)
			{
            	if (items[i] is XmlSpawner)
            	{
	            	XmlSpawner sp = items[i] as XmlSpawner;
	 				
					if (sp != null && !sp.Deleted && !(sp.RootParent is Mobile))
					{
						if (DoRegionFix(sp))
						{
							xml.Add(sp);
						}
					}
            	}
 			}
 			           	
            for (int i = 0; i < xml.Count; i++)
            {            		
            	string region = GetRegionName(xml[i]);
                
            	xml[i].Name = String.Format ("[{0}] {1}",i ,TypeList(xml[i]));
				
				m_SpawnList.Add (new SpawnerEntries( xml[i].Map.ToString(), region, xml[i]));
			}
		}
		
		private static string TypeList(XmlSpawner spawner)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			int max = 2; //truncate name at <max> entries
						
			if (spawner.SpawnObjects.Length > 0)
			{
				for (int i = 0; i < spawner.SpawnObjects.Length; i++)
				{
					XmlSpawner.SpawnObject so = spawner.SpawnObjects[i];
											
					if (sb.Length > 0 && i < sb.Length-1)
					{
						sb.Append("-");
					}
					
					sb.Append($"{so.TypeName[0].ToString().ToUpper()}{so.TypeName.Substring(1)}");
					
					Type type = SpawnerType.GetType(so.TypeName);
					
					if(type != null && type.IsSubclassOf(typeof(MondainQuester)))
				    {
						sb.Append("-MLQuester");
				    }
					
					if (i == max-1)
					{
						sb.Append("-Etc");

						break;
					}
				}
			}

			return sb.ToString();
		}
				
		private static bool DoRegionFix(XmlSpawner spawner)
		{
			if (spawner.RootParent == null)
			{
				Region reg = Region.Find (spawner.Location, spawner.Map);

				GuardedRegion region = reg.GetRegion(typeof(GuardedRegion)) as GuardedRegion;
		   	
			   	if (region != null && !spawner.IsGuardExempt)
		   		{
		   			spawner.IsGuardExempt = true;
		   		}
			}
			
		   	return true;
		}
		
 		private static string GetRegionName(XmlSpawner spawner)
 		{			
			var loc = spawner.Location;
			var map = spawner.Map;

			if (spawner.RootParent != null)
			{
				if (spawner.RootParent is Mobile m)				
				{
					loc = m.Location;
					map = m.Map;
				}
				else if (spawner.RootParent is Item i)				
				{
					loc = i.Location;
					map = i.Map;
				}
			}

 			Region reg = Region.Find (loc, map);
				
			string region = "Wilderness";

			if (reg.ToString() != "Region") //omg jumping through hoops to get region name
			{
				region = reg.ToString();
			
				if (!reg.IsDefault)
                {
                    reg = reg.Parent;

                    while (reg != null)
                    {
                    	region = reg.ToString();
                        reg = reg.Parent;
                    }
                }
			}
			
			return region;
 		}
 		
 		public static void ExportSpawns(string path)
		{
 			foreach (SpawnerEntries entry in m_SpawnList)
			{
				string mapfolder = Path.Combine (path, entry.Maps);

				if (!Directory.Exists(mapfolder))
				{
	            	Directory.CreateDirectory (mapfolder);
	            }

				string regfolder = Path.Combine (mapfolder, entry.Regs);
 
				if (!Directory.Exists (regfolder))
				{
	            	Directory.CreateDirectory (regfolder);
	            }
				
				List<XmlSpawner> xml = new List<XmlSpawner>();
				
				xml.Add (entry.Spawners);
				
				XmlSpawner.SaveSpawnList (null, xml, Path.Combine( regfolder, String.Format("{0}.xml", entry.Spawners.Name )), false, false);
 			}
		}
	}
	
	public class SpawnerEntries
	{
		private string m_Maps;
		private string m_Regs;
		private XmlSpawner m_Spawners;

		public SpawnerEntries (string map, string reg, XmlSpawner spawner)
		{
			m_Maps = map;
			m_Regs = reg;
			m_Spawners = spawner;
		}

		public string Maps { get {return m_Maps;} set {m_Maps = value; } }
		public string Regs { get {return m_Regs;} set {m_Regs = value; } }
		public XmlSpawner Spawners { get {return m_Spawners;} set {m_Spawners = value; } }
	}
}