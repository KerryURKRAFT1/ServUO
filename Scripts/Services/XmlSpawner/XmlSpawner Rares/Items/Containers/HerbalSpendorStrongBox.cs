using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public class HSMetalBox : RaresLockableContainer
    {
        public override bool Decays{ get{ return false; } } 

        public override Rectangle2D Bounds
        {
            get{ return new Rectangle2D( 18, 105, 144, 73 ); }
        }

        [Constructable]
        public HSMetalBox() : base( 0x9A8 )
        {
			Name = "strong box";
        	Movable = false;
            Locked = true;
            RequiredSkill = 22;
            LockLevel = this.RequiredSkill - Utility.Random( 1, 10 );
            MaxLockLevel = this.RequiredSkill;

            DropItem( new Gold( Utility.Random( 10, 50 ) ) );
            
            if( Utility.RandomBool() == true )
            {
				switch( Utility.Random( 4 ) )
            	{
					case 0: DropItem( new StealableWhiteDriedFlowers() ); break;
					case 1: DropItem( new StealableGreenDriedFlowers() ); break;
					case 2: DropItem( new StealableDriedHerbs() ); break;
					case 3: DropItem( new StealableDriedOnions() ); break;
					default: break;
            	}
            }
        }

        public HSMetalBox( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}