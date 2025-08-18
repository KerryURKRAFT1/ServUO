//create for purposes of homerange

using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Regions;
using Server.Targeting;
using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
	public class RaresBaseNPC : BaseCreature
	{			
		public override bool CanOpenDoors{ get{ return true; } }

		public RaresBaseNPC() : base( AIType.AI_Thief, FightMode.Aggressor, 15, 1, 0.2, 0.4 )
		{
		}

		public RaresBaseNPC(AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed): base (ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
		{
		}

		public void AddRares(Item item, int chance)
		{
			if (Utility.Random(chance) == 0)
			{
				PackItem( item );
				item.Movable = true; // override if in pack
			}
		}
		
		public RaresBaseNPC( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
    	     base.Deserialize( reader ); 
	
        	 int version = reader.ReadInt(); 
		}
	}
}
