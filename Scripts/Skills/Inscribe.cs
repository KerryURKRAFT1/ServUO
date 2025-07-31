using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.SkillHandlers
{
    public class Inscribe
    {
        private static readonly Hashtable m_UseTable = new Hashtable();
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Inscribe].Callback = new SkillUseCallback(OnUse);
        }

    	public static TimeSpan OnUse(Mobile m)
        {
    		if (!SkillRegistry.Contains(m))
    		{
	    		SkillRegistry.Add(m);
	        	
	        	if (!TriggerSkill(m))
	        	{
		    		SkillRegistry.Remove(m);
	        	}
    		}
    		else if (SkillRegistry.WaitMsg)
    		{
                m.SendMessage("You must wait to perform another action.");
    		}
        	
        	return TimeSpan.Zero;
        }

        public static bool TriggerSkill(Mobile m)
        {
            Target target = new InternalTargetSrc();
            m.Target = target;
            m.SendLocalizedMessage(1046295); // Target the book you wish to copy.
            target.BeginTimeout(m, TimeSpan.FromSeconds(10.0));

            return true;
        }

        public static Mobile GetUser(BaseBook book)
        {
            return (Mobile)m_UseTable[book];
        }

        public static bool IsEmpty(BaseBook book)
        {
            foreach (BookPageInfo page in book.Pages)
            {
                foreach (string line in page.Lines)
                {
                    if (line.Trim().Length != 0)
                        return false;
                }
            }
            return true;
        }

        public static void Copy(BaseBook bookSrc, BaseBook bookDst)
        {
            bookDst.Title = bookSrc.Title;
            bookDst.Author = bookSrc.Author;

            BookPageInfo[] pagesSrc = bookSrc.Pages;
            BookPageInfo[] pagesDst = bookDst.Pages;
            for (int i = 0; i < pagesSrc.Length && i < pagesDst.Length; i++)
            {
                BookPageInfo pageSrc = pagesSrc[i];
                BookPageInfo pageDst = pagesDst[i];

                int length = pageSrc.Lines.Length;
                pageDst.Lines = new string[length];

                for (int j = 0; j < length; j++)
                    pageDst.Lines[j] = pageSrc.Lines[j];
            }
        }

        private static void SetUser(BaseBook book, Mobile mob)
        {
            m_UseTable[book] = mob;
        }

        private static void CancelUser(BaseBook book)
        {
            m_UseTable.Remove(book);
        }

        private class InternalTargetSrc : Target
        {
            public InternalTargetSrc()
                : base(3, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseBook book = targeted as BaseBook;
                if (book == null)
                    from.SendLocalizedMessage(1046296); // That is not a book
                else if (Inscribe.IsEmpty(book))
                    from.SendLocalizedMessage(501611); // Can't copy an empty book.
                else if (Inscribe.GetUser(book) != null)
                    from.SendLocalizedMessage(501621); // Someone else is inscribing that item.
                else
                {
                    Target target = new InternalTargetDst(book);
                    from.Target = target;
                    from.SendLocalizedMessage(501612); // Select a book to copy this to.
                    target.BeginTimeout(from, TimeSpan.FromSeconds(10.0));
                    
   		            new SkillTimer(from, book, SkillRegistry.Delay).Start();
   		            
   		            return;
                }

                SkillRegistry.Remove(from);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
				SkillRegistry.Remove(from);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1076203); // Target out of range.	
				SkillRegistry.Remove(from);
            }

	        protected override void OnTargetOutOfLOS(Mobile from, object o)
	        {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237);// Target can not be seen.
                SkillRegistry.Remove(from);
	        }
        }

        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly BaseBook m_Targ;

            public SkillTimer(Mobile owner, BaseBook targ, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
                Inscribe.SetUser(m_Targ, m_Owner);
                
                SkillRegistry.Remove(m_Owner);
			}
		}

        private class InternalTargetDst : Target
        {
            private readonly BaseBook m_BookSrc;
            public InternalTargetDst(BaseBook bookSrc)
                : base(3, false, TargetFlags.None)
            {
                this.m_BookSrc = bookSrc;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_BookSrc.Deleted)
                    return;

                BaseBook bookDst = targeted as BaseBook;

                if (bookDst == null)
                    from.SendLocalizedMessage(1046296); // That is not a book
                else if (Inscribe.IsEmpty(this.m_BookSrc))
                    from.SendLocalizedMessage(501611); // Can't copy an empty book.
                else if (bookDst == this.m_BookSrc)
                    from.SendLocalizedMessage(501616); // Cannot copy a book onto itself.
                else if (!bookDst.Writable)
                    from.SendLocalizedMessage(501614); // Cannot write into that book.
                else if (Inscribe.GetUser(bookDst) != null)
                    from.SendLocalizedMessage(501621); // Someone else is inscribing that item.
                else
                {
                    if (from.CheckTargetSkill(SkillName.Inscribe, bookDst, 0, 50))
                    {
                        Inscribe.Copy(this.m_BookSrc, bookDst);

                        from.SendLocalizedMessage(501618); // You make a copy of the book.
                        from.PlaySound(0x249);
                    }
                    else
                    {
                        from.SendLocalizedMessage(501617); // You fail to make a copy of the book.
                    }
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Timeout)
                    from.SendLocalizedMessage(501619); // You have waited too long to make your inscribe selection, your inscription attempt has timed out.
            }

            protected override void OnTargetFinish(Mobile from)
            {
                Inscribe.CancelUser(this.m_BookSrc);
            }
        }
    }
}