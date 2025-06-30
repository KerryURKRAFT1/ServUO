using System;

namespace Server.Items
{
	public class SpellbookFull : Spellbook
	{
		[Constructable]
		public SpellbookFull()
		{
			this.Content = ulong.MaxValue;
		}
		
		public SpellbookFull( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}