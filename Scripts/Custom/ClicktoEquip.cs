//
// Original Script by DeusX
//

using Server.Mobiles;

namespace Server.Items
{
	public static class ClickToEquip
    {
		private enum Action
	    {
	    	Equip,			//just equip
	        SwapDirect,		//swap like for like
	        SwapIndirect,	//swap left for right and vice versa
	        SwapDifferent	//swap 2 handed for 1 handed and vice versa
	    }
	
		public static void OnDoubleClick( Mobile from, Item item )
        {			
			if( item.RootParent != from || ( item.Parent == from && item.Movable && from.Backpack.TryDropItem( from, item, true )))
        	{
        		return;
        	}
        	
			if( !item.IsAccessibleTo( from ) || item.Layer == Layer.Invalid || item.Parent is Corpse || from.Backpack == null || !item.CanEquip(from))
            {
				from.SendLocalizedMessage(1071936); // You cannot equip that.
            }
			else if( !item.Movable && from.AccessLevel == AccessLevel.Player )
            {
                from.SendLocalizedMessage(1005213); // You can't do that
            }
        	else if( !from.InRange(item.GetWorldLocation(), 2 ))
            {
                from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
            }
			else
			{
	            Action action = Action.Equip;
	
	            Item item1h = from.FindItemOnLayer( Layer.OneHanded );
	            Item item2h = from.FindItemOnLayer( Layer.TwoHanded );
		        Item swapItem;
					
		        Container container = item.Parent as Container ?? from.Backpack;

		        switch( item.Layer )
	            {
	            	default:
	        		{
		                item1h = from.FindItemOnLayer( item.Layer );
		
		                if( item1h != null )
		                {
		                    action = Action.SwapDirect;
		                }
		                break;
	           		}
	            	case Layer.OneHanded:
	        		{
	                    if( item1h != null )
	                    {
	                        if( item2h != null && item2h is BaseShield )
	                        {
	                            item2h = null;
	                        }
	
	                        action = Action.SwapIndirect;
	                    }
	                    else if( item2h != null && !( item2h is BaseShield ))
	                    {
	                        action = Action.SwapIndirect;
	                    }
	                    break;
	        		}
	            	case Layer.TwoHanded:
	        		{
	                    if( item is BaseShield )
	                    {
	                        if( item2h != null )
	                        {
	                            action = Action.SwapIndirect;
	                        }
	                    }
	                    else
	                    {
	                        if( item2h != null )
	                        {
	                            if( item1h != null )
	                            {
	                                action = Action.SwapDifferent;
	                            }
	                            else
	                            {
	                                action = Action.SwapIndirect;
	                            }
	                        }
	                        else if( item1h != null )
	                        {
	                            action = Action.SwapIndirect;
	                        }
	                    }
		                break;
	        		}
	            }
	
	       		swapItem = item1h;
	
	       		switch (action)
	            {
	       			default:
					case Action.Equip:
	            	{
	                   	from.EquipItem( item );
	                    break;
	            	}
					case Action.SwapIndirect:
	            	{
	                    swapItem = item2h ?? item1h;
	                    goto case Action.SwapDirect;
	            	}
	                case Action.SwapDirect:
	            	{
	            		if( swapItem != null  ) // sanity
	            		{
		                    if( from is PlayerMobile ) 
		                    {
		                        if( container.TryDropItem( from, swapItem, true ))
		                        {
		                            if( from.EquipItem( item ))
		                            {
		                                swapItem.Location = item.Location;
		                            }
		                        }
		                    }
		                    else
		                    {
		                        swapItem.Delete();
		                        goto default;
		                    }
	            		}
	                    break;
	            	}
	                case Action.SwapDifferent:
	            	{
	            		if( item1h != null && item2h != null ) // sanity
	            		{
	            			if( from is PlayerMobile )
		                    {
		                        if( container.TryDropItem( from, item1h, true ) && container.TryDropItem( from, item2h, true ))
		                        {
		                            if( from.EquipItem( item ))
		                            {
		                                item1h.Location = item.Location;
		                                item2h.Location = item.Location;
		                            }
		                        }
		                    }
		                    else
		                    {
		                        item1h.Delete();
		                        item2h.Delete();
		                        goto default;
		                    }
	            		}
	                    break;
	        		}
	            }
	        }
	    }
	}
}	
