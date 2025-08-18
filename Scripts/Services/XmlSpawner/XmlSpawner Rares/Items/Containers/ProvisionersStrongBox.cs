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
    public class ProvisionersMetalBox : RaresLockableContainer
    {
        public override bool Decays{ get{ return false; } } 

        public override Rectangle2D Bounds
        {
            get{ return new Rectangle2D( 18, 105, 144, 73 ); }
        }

        [Constructable]
        public ProvisionersMetalBox() : base( 0x9A8 )
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
            	switch( Utility.Random( 8 ) )
            	{
            		case 0: DropItem( new StealableDates() ); break;
            		case 1: DropItem( new StealableBananas() ); break;
            		case 2: DropItem( new StealableCheeseWedge() ); break;
            		case 3: DropItem( new StealableLemons() ); break;
            		case 4: DropItem( new StealableLimes() ); break;
            		case 5: DropItem( new StealableBaconSlab() ); break;
            		case 6: DropItem( new StealableCoconutHalf() ); break;
            		case 7: DropItem( new StealableCoconutHalf() ); break;
            		default : break;           		
            	}
            }
        }

        public ProvisionersMetalBox( Serial serial ) : base( serial )
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