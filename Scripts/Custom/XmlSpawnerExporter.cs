using System;
using Server.Commands;
using Server.Mobiles;
using System.IO;
using System.Collections.Generic;

namespace Server.Misc
{
	class XmlSpawnerExporter
	{
		private static bool m_Enabled = true; //true: On WorldSave
		
		public static string RootPath = XmlSpawner.XmlSpawnDir;
		
		private static List<SpawnerEntries> m_SpawnList;

		public static void Initialize()
		{
			if( m_Enabled )
			{
				EventSink.AfterWorldSave += new AfterWorldSaveEventHandler( WorldSave_OnEvent );
			}

			CommandSystem.Register( "XmlSpawnerExporter", AccessLevel.Developer, new CommandEventHandler( XmlSpawnerExporter_OnCommand ) );

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

            string date = DateTime.UtcNow.ToString ("[yyyy-MM-dd] [HH-mm-ss]");

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
						xml.Add(sp);
					}
            	}
 			}
 			
            for (int i = 0; i < xml.Count; i++)
            {
				string reg = Region.Find (xml[i].Location, xml[i].Map).ToString();
				
				xml[i].Name = String.Format ("{0} [{1}]", reg, i);
				
				m_SpawnList.Add (new SpawnerEntries( xml[i].Map.ToString(), reg, xml[i]));
			}
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