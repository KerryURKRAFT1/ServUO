/**************************************
*Script Name: Static Exporter
*Author: Nerun
*Rewritten by: Joeku
*Revised by: Nerun
**************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Targeting;

namespace Joeku.SE
{
	public class SE_Main
	{
		public const int Version = 200; // Script version (do not change)
		
		private static string rootfolder = "StaticExport";

		public static void Initialize()
		{
			CommandSystem.Register("StaticExport" , AccessLevel.Developer, new CommandEventHandler(StaticExport_OnCommand));
			CommandSystem.Register("StaEx" , AccessLevel.Developer, new CommandEventHandler(StaticExport_OnCommand));
		}

		[Usage( "StaticExport <filename> with or without <x1> <y1> <x2> <y2>" )]
		[Aliases( "StaEx" )]
		[Description( "Exports statics to a cfg decoration file." )]
		public static void StaticExport_OnCommand(CommandEventArgs e )
		{
			if( e.Arguments.Length == 1 )
			{
				BeginStaEx(e.Mobile, e.ArgString );
			}
			else if( e.Arguments.Length == 5 )
			{
				int x1 = Utility.ToInt32( e.Arguments[1] );
				int y1 = Utility.ToInt32( e.Arguments[2] );
				int x2 = Utility.ToInt32( e.Arguments[3] );
				int y2 = Utility.ToInt32( e.Arguments[4] );
				ExportXY( e.Mobile, e.Arguments[0], x1, y1, x2, y2 );
			}
			else
				e.Mobile.SendMessage("Format: StaticExport <filename>" );
		}

		public static void BeginStaEx(Mobile mob, string file )
		{
			BoundingBoxPicker.Begin(mob, new BoundingBoxCallback(StaExBox_Callback), new object[]{ file });
		}

		private static void StaExBox_Callback(Mobile mob, Map map, Point3D start, Point3D end, object state)
		{
			object[] states = (object[])state;
			string file = (string)states[0];

			StaticExport( mob.Map, file, new Rectangle2D(new Point2D(start.X, start.Y), new Point2D(end.X+1, end.Y+1) ));
		}
		
		private static void ExportXY(Mobile mob, string file, int ax, int ay, int bx, int by)
		{
			StaticExport( mob.Map, file, new Rectangle2D( new Point2D(ax, ay), new Point2D(bx, by) ) );
		}

		public static void StaticExport(Map map, string folder, Rectangle2D rect)
		{
			if ( !Directory.Exists( rootfolder ) )
				Directory.CreateDirectory( rootfolder );
			
			string path = Path.Combine( rootfolder, folder );

			if ( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );

			List<Item> items = new List<Item>();
			List<Item> signs = new List<Item>();
			List<Item> doors = new List<Item>();
			List<Item> teleporters = new List<Item>();
			List<Item> statics = new List<Item>();
			List<Item> custom = new List<Item>();
			
			IPooledEnumerable eable = map.GetItemsInBounds( rect );

			try
			{
				foreach( Item item in eable )
				{
					if( item == null || item.Deleted || item.Movable || item.Decays )
						continue;
					else if( item is AddonComponent || item is XmlSpawner || item is Spawner || item is Corpse )
						continue;
					
					else if( item is BaseDoor || item is PublicMoongate || item is Teleporter )
					{
						if( item is BaseDoor )
							doors.Add( item );
						else
							teleporters.Add( item );						
					}
					else if( item is BaseSign || item is Static )
					{				        
						if( item is BaseSign || (( item.ItemID >= 2979 && item.ItemID <=3086 ) || ( item.ItemID >= 3139 && item.ItemID <= 3140 )))							
							signs.Add( item );
						else
							statics.Add( item );							
					}
					else
					{					
						items.Add( item );
					}
				}
			}
			catch{}
			
			eable.Free();
			
			if( signs.Count > 0 )
				WriteList( signs, Path.Combine( path, "signs.cfg" ) );

			if( doors.Count > 0 )
				WriteList( doors, Path.Combine( path, "doors.cfg" ) );
					
			if( teleporters.Count > 0 )
				WriteList( teleporters, Path.Combine( path, "teleporters.cfg" ) );
					
			if( statics.Count > 0 )
				WriteList( statics, Path.Combine( path, "statics.cfg" ) );

			if( items.Count > 0 )
				WriteList( items, Path.Combine( path, "sundry.cfg" ) );

			if( items.Count > 0 )
				WriteList( custom, Path.Combine( path, "custom.cfg" ) );
		}

		private static void WriteList( List<Item> items, string path )
		{
			using ( StreamWriter op = new StreamWriter( path ) )
			{
				foreach( Item item in items )
				{
					string s = Construct( item );
	
					if( !s.Substring(0, s.IndexOf(' ')+1).Contains("+") ) // Make sure this isn't an InternalItem of a class...
					{
						op.WriteLine(s);
						op.WriteLine("{0} {1} {2}", item.X, item.Y, item.Z);
						op.WriteLine();
					}
				}
			}
		}

		public static List<string[]> List = new List<string[]>();

		public static void Add(string s)
		{
			Add(s, "");
		}
		
		public static void Add(string s1, string s2)
		{
			List.Add(new string[]{s1, s2});
		}

		public static string Construct(Item item)
		{
			string s;

			int itemID = item.ItemID;

			if( item is BaseAddon )
				for( int i = 0; i < ((BaseAddon)item).Components.Count; i++ )
					if( ((BaseAddon)item).Components[i].Offset == Point3D.Zero )
			{
				itemID = ((BaseAddon)item).Components[i].ItemID;
				break;
			}

			if( item is LocalizedStatic )
				Add("LabelNumber", ((LocalizedStatic)item).Number.ToString());
			else if( item is LocalizedSign )
				Add("LabelNumber", ((LocalizedSign)item).Number.ToString());
			else if( item is AnkhWest )
				Add("Bloodied", (item.ItemID == 0x1D98).ToString());
			else if( item is AnkhNorth )
				Add("Bloodied", (item.ItemID == 0x1E5D).ToString());
			else if( item is MarkContainer )
			{
				Add("Bone", ((MarkContainer)item).Bone.ToString());
				Add("Locked", ((MarkContainer)item).AutoLock.ToString());
				if( ((MarkContainer)item).TargetMap != null )
					Add("TargetMap", ((MarkContainer)item).TargetMap.ToString());
			}
			else if( item is WarningItem )
			{
				Add("Range", ((WarningItem)item).Range.ToString());
				if( VS(((WarningItem)item).WarningString) )
					Add("WarningString", ((WarningItem)item).WarningString);
				Add("WarningNumber", ((WarningItem)item).WarningNumber.ToString());
				if( item is HintItem )
				{
					if( VS(((HintItem)item).HintString) )
						Add("HintString", ((HintItem)item).HintString);
					Add("HintNumber", ((HintItem)item).HintNumber.ToString());
				}
				Add("Range", ((WarningItem)item).ResetDelay.ToString());
			}
			else if( item is SerpentPillar )
			{
				if( VS(((SerpentPillar)item).Word) )
					Add("Word", ((SerpentPillar)item).Word);
				Add("DestStart", ((SerpentPillar)item).Destination.Start.ToString());
				Add("DestEnd", ((SerpentPillar)item).Destination.End.ToString());
			}
			else if( item.GetType().IsSubclassOf(typeof(BaseBeverage)) )
				Add("Content", ((BaseBeverage)item).Content.ToString());
			else if( item.GetType().IsSubclassOf(typeof(BaseDoor)) )
				Add("Facing", GetFacing(((BaseDoor)item).Offset).ToString());

			else if( item is BaseLight )
			{
				if( !((BaseLight)item).Burning )
					Add("Unlit", String.Empty);
				if( !((BaseLight)item).Protected )
					Add("Unprotected", String.Empty);
			}
			else if( item is RecallRune )
			{
				RecallRune rune = (RecallRune)item;

				if( VS(rune.Description) )
					Add("Description", rune.Description);
				Add("Marked", rune.Marked.ToString());
				if( rune.TargetMap != null )
					Add("TargetMap", rune.TargetMap.ToString());
				Add("Target", rune.Target.ToString());
			}
			else if( item is Teleporter )
			{
				Teleporter tp = (Teleporter)item;

				if( item is SkillTeleporter )
				{
					SkillTeleporter st = (SkillTeleporter)item;

					Add("Skill", st.Skill.ToString());
					// "RequiredFixedPoint" == Required * 0.1 ?
					Add("Required", st.Required.ToString());
					if( VS(st.MessageString) )
						Add("MessageString", st.MessageString);
					Add("MessageNumber", st.MessageNumber.ToString());
				}
				else if( item is KeywordTeleporter )
				{
					KeywordTeleporter kt = (KeywordTeleporter)item;

					if( VS(kt.Substring) )
						Add("Substring", kt.Substring);
					Add("Keyword", kt.Keyword.ToString());
					Add("Range", kt.Range.ToString());
				}
				Add("PointDest", tp.PointDest.ToString());
				if( tp.MapDest != null )
					Add("MapDest", tp.MapDest.ToString());
				Add("Creatures", tp.Creatures.ToString());
				Add("SourceEffect", tp.SourceEffect.ToString());
				Add("DestEffect", tp.DestEffect.ToString());
				Add("SoundID", tp.SoundID.ToString());
				Add("Delay", tp.Delay.ToString());
			}
			else if( item is FillableContainer )
				Add("ContentType", ((FillableContainer)item).ContentType.ToString());

			if( item.Light != LightType.ArchedWindowEast )
				Add("Light", item.Light.ToString());
			if( item.Hue > 0 )
				Add("Hue", item.Hue.ToString());
			if( VS(item.Name) )
				Add("Name", item.Name);
			if( item.Amount > 1 )
				Add("Amount", item.Amount.ToString());
			
			s = String.Format("{0} {1}", ConstructType(item), itemID);

			if( item is Sign )
				s = String.Format("Static {0}", itemID);

			if( item is LocalizedSign )
				s = String.Format("LocalizedSign {0}", itemID);
			
			if( List.Count > 0 )
			{
				s += " (";
				for( int i = 0; i < List.Count; i++ )
				{
					if( List[i][1] == String.Empty )
						s += String.Format("{0}{1}", List[i][0], (i < List.Count-1 ? "; " : String.Empty));
					else
						s += String.Format("{0}={1}{2}", List[i][0], List[i][1], (i < List.Count-1 ? "; " : String.Empty));
				}
				s += ")";
			}

			List.Clear();
			return s;
		}

		public static bool VS(string s)
		{
			if( s == null || s == String.Empty )
				return false;
			return true;
		}

		public static string ConstructType(Item item)
		{
			string s = item.GetType().ToString();

			if( s.LastIndexOf('.') > -1 )
				s = s.Remove(0, s.LastIndexOf('.')+1);

			return s;
		}

		public static DoorFacing GetFacing(Point3D p)
		{
			DoorFacing facing = DoorFacing.WestCW;
			for(int i = 0; i < m_Offsets.Length; i++)
			{
				if( p == m_Offsets[i] )
				{
					facing = (DoorFacing)i;
					break;
				}
			}
			
			return facing;
		}
		private static Point3D[] m_Offsets = new Point3D[]
		{
			new Point3D(-1, 1, 0 ),
			new Point3D( 1, 1, 0 ),
			new Point3D(-1, 0, 0 ),
			new Point3D( 1,-1, 0 ),
			new Point3D( 1, 1, 0 ),
			new Point3D( 1,-1, 0 ),
			new Point3D( 0, 0, 0 ),
			new Point3D( 0,-1, 0 ),

			new Point3D( 0, 0, 0 ),
			new Point3D( 0, 0, 0 ),
			new Point3D( 0, 0, 0 ),
			new Point3D( 0, 0, 0 )
		};
	}
}