using System;
using Server.Items;
using Server.Targeting;
using Server.Gumps;

namespace Server.Engines.Craft
{
    public enum SmeltResult
    {
        Success,
        Invalid,
        NoSkill
    }

    public class Resmelt
    {
        public Resmelt()
        {
        }

        public static void Do(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
        {
            int num = craftSystem.CanCraft(from, tool, null);

            if (num > 0 && num != 1044267)
            {
                from.SendGump(new CraftGump(from, craftSystem, tool, num, CraftGump.CraftPage.None, isPreAoS));
            }
            else
            {
                from.Target = new InternalTarget(craftSystem, tool, isPreAoS);
                from.SendLocalizedMessage(1044273); // Target an item to recycle.
            }
        }

        private class InternalTarget : Target
        {
            private readonly CraftSystem m_CraftSystem;
            private BaseTool m_Tool;
            private readonly bool m_IsPreAoS;

            public InternalTarget(CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base(2, false, TargetFlags.None)
            {
                this.m_CraftSystem = craftSystem;
                this.m_Tool = tool;
                this.m_IsPreAoS = isPreAoS;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int num = this.m_CraftSystem.CanCraft(from, this.m_Tool, null);

                if (num > 0)
                {
                    if (num == 1044267)
                    {
                        bool anvil, forge;

                        DefBlacksmithy.CheckAnvilAndForge(from, 2, out anvil, out forge);

                        if (!anvil)
                            num = 1044266; // You must be near an anvil
                        else if (!forge)
                            num = 1044265; // You must be near a forge.
                    }

                    from.SendGump(new CraftGump(from, this.m_CraftSystem, this.m_Tool, num, CraftGump.CraftPage.None, m_IsPreAoS));
                }
                else
                {
                    SmeltResult result = SmeltResult.Invalid;
                    bool isStoreBought = false;
                    int message;

                    if (targeted is BaseArmor)
                    {
                        result = this.Resmelt(from, (BaseArmor)targeted, ((BaseArmor)targeted).Resource);
                        isStoreBought = !((BaseArmor)targeted).PlayerConstructed;
                    }
                    else if (targeted is BaseWeapon)
                    {
                        result = this.Resmelt(from, (BaseWeapon)targeted, ((BaseWeapon)targeted).Resource);
                        isStoreBought = !((BaseWeapon)targeted).PlayerConstructed;
                    }
                    else if (targeted is DragonBardingDeed)
                    {
                        result = this.Resmelt(from, (DragonBardingDeed)targeted, ((DragonBardingDeed)targeted).Resource);
                        isStoreBought = false;
                    }

                    switch (result)
                    {
                        default:
                        case SmeltResult.Invalid:
                            message = 1044272;
                            break; // You can't melt that down into ingots.
                        case SmeltResult.NoSkill:
                            message = 1044269;
                            break; // You have no idea how to work this metal.
                        case SmeltResult.Success:
                            message = isStoreBought ? 500418 : 1044270;
                            break; // You melt the item down into ingots.
                    }

                    // Dopo lo smelting, aprire il menu corretto (Pre-AoS o AoS)
                    if (m_Tool != null && m_Tool.Deleted)
                    {
                        m_Tool = null;
                    }

                    if (m_Tool == null)
                    {
                        from.SendLocalizedMessage(1044038); // You have worn out your tool!
                        return;
                    }

                    if (m_IsPreAoS)
                    {
                        from.SendMenu(new NewCraftingMenu(from, m_CraftSystem, m_Tool, message, m_IsPreAoS));
                    }
                    else
                    {
                        from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, message, CraftGump.CraftPage.None, m_IsPreAoS));
                    }
                }
            }

            private SmeltResult Resmelt(Mobile from, Item item, CraftResource resource)
            {
                try
                {
                    if (Ethics.Ethic.IsImbued(item))
                        return SmeltResult.Invalid;

                    if (CraftResources.GetType(resource) != CraftResourceType.Metal)
                        return SmeltResult.Invalid;

                    CraftResourceInfo info = CraftResources.GetInfo(resource);

                    if (info == null || info.ResourceTypes.Length == 0)
                        return SmeltResult.Invalid;

                    CraftItem craftItem = this.m_CraftSystem.CraftItems.SearchFor(item.GetType());

                    if (craftItem == null || craftItem.Resources.Count == 0)
                        return SmeltResult.Invalid;

                    CraftRes craftResource = craftItem.Resources.GetAt(0);

                    if (craftResource.Amount < 2)
                        return SmeltResult.Invalid; // Not enough metal to resmelt

                    double difficulty = 0.0;

                    switch (resource)
                    {
                        case CraftResource.DullCopper:
                            difficulty = 65.0;
                            break;
                        case CraftResource.ShadowIron:
                            difficulty = 70.0;
                            break;
                        case CraftResource.Copper:
                            difficulty = 75.0;
                            break;
                        case CraftResource.Bronze:
                            difficulty = 80.0;
                            break;
                        case CraftResource.Gold:
                            difficulty = 85.0;
                            break;
                        case CraftResource.Agapite:
                            difficulty = 90.0;
                            break;
                        case CraftResource.Verite:
                            difficulty = 95.0;
                            break;
                        case CraftResource.Valorite:
                            difficulty = 99.0;
                            break;
                    }

                    if (difficulty > from.Skills[SkillName.Mining].Value)
                        return SmeltResult.NoSkill;

                    Type resourceType = info.ResourceTypes[0];
                    Item ingot = (Item)Activator.CreateInstance(resourceType);

                    if (item is DragonBardingDeed || (item is BaseArmor && ((BaseArmor)item).PlayerConstructed) || (item is BaseWeapon && ((BaseWeapon)item).PlayerConstructed) || (item is BaseClothing && ((BaseClothing)item).PlayerConstructed))
                        ingot.Amount = craftResource.Amount / 2;
                    else
                        ingot.Amount = 1;

                    item.Delete();
                    from.AddToBackpack(ingot);

                    from.PlaySound(0x2A);
                    from.PlaySound(0x240);
                    return SmeltResult.Success;
                }
                catch
                {
                }

                return SmeltResult.Invalid;
            }
        }
    }
}