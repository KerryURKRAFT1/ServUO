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
    public class PDMetalChest : RaresLockableContainer
    {
        private const int m_Level = 1;

        public override bool Decays{ get{ return false; } }

        public override Rectangle2D Bounds
        {
            get{ return new Rectangle2D( 18, 105, 144, 73 ); }
        }

        [Constructable]
        public PDMetalChest() : base( 0x9AB )
        {
			Name = "strong box";
        	Movable = false;

        	TrapType = TrapType.PoisonTrap;
            TrapPower = m_Level * Utility.Random( 1, 10 );

            Locked = true;
            RequiredSkill = 22;
            LockLevel = this.RequiredSkill - Utility.Random( 1, 10 );
            MaxLockLevel = this.RequiredSkill;
            
			DropItem( new Bananas() );
			
            if( Utility.RandomBool() == true )
            {
				switch( Utility.Random( 5 ) )
            	{
					case 0: DropItem( new StealableCheeseWedge() ); break;
					case 1: DropItem( new StealableLemons() ); break;
					case 2: DropItem( new StealableLimes() ); break;
					case 3: DropItem( new StealableLimes() ); break;
					case 4: DropItem( new StealableLimes() ); break;
					default: break;
            	}
            }
        }

        public PDMetalChest( Serial serial ) : base( serial )
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