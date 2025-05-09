using System;
using Server.Items;

namespace Server.Engines.Craft
{


    public class DefClassicAlchemy : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Alchemy;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044001;
            }// <CENTER>ALCHEMY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefClassicAlchemy();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefClassicAlchemy()
            : base(1, 1, 1.25)// base( 1, 1, 3.1 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.


            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x242);
        }

        private static readonly Type typeofPotion = typeof(BasePotion);

        public static bool IsPotion(Type type)
        {
            return typeofPotion.IsAssignableFrom(type);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (IsPotion(item.ItemType))
                {
                    from.AddToBackpack(new Bottle());
                    return 500287; // You fail to create a useful potion.
                }
                else
                {
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                }
            }
            else
            {
                from.PlaySound(0x240); // Sound of a filling bottle

                if (IsPotion(item.ItemType))
                {
                    if (quality == -1)
                        return 1048136; // You create the potion and pour it into a keg.
                    else
                        return 500279; // You pour the potion into a bottle...
                }
                else
                {
                    return 1044154; // You create the item.
                }
            }
        }

/// <summary>
/// // OVERRIDE CRAFT METHOD
/// </summary>
/*
            public override void Craft(Mobile from, CraftItem item, BaseTool tool)
            {
                // Controlla se il giocatore ha tutti i materiali necessari
                foreach (CraftRes res in item.Resources)
                {
                    int quantity = from.Backpack.GetAmount(res.ItemType);

                    if (quantity < res.Amount)
                    {
                        from.SendMessage("You do not have the necessary materials to craft any items");
                        return;
                    }
                }

                bool firstReagent = true; // Flag per il primo messaggio

                // Consuma i materiali uno alla volta e mostra il messaggio
                foreach (CraftRes res in item.Resources)
                {
                    for (int i = 0; i < res.Amount; i++)
                    {
                        // Rimuove un'unità del materiale
                        Item consumed = from.Backpack.ConsumeTotal(res.ItemType, 1);

                        if (consumed != null)
                        {
                            if (firstReagent)
                            {
                                from.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "You start grinding some " + res.ItemType.Name + " in the Mortar");
                                firstReagent = false; // Cambia il flag dopo il primo messaggio
                            }
                            else
                            {
                                from.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "You add some " + res.ItemType.Name + " and continue grinding");
                            }
                        }
                    }
                }

                // Completa il crafting
                base.Craft(from, item, tool);

                // Messaggio di successo finale
                from.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "You pour the completed potion into a bottle");
            }

*/
/// <summary>
/// //
/// </summary>

        public override void InitCraftList()
        {
            int index = -1;

            // Healing and Curative
            index = AddCraft(typeof(RefreshPotion), 1116348, 1044538, -25, 25.0, typeof(BlackPearl), 1044353, 1, 1044361);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(TotalRefreshPotion), 1116348, 1044539, 25.0, 75.0, typeof(BlackPearl), 1044353, 5, 1044361);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(LesserHealPotion), 1116348, 1044543, -25.0, 25.0, typeof(Ginseng), 1044356, 1, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(HealPotion), 1116348, 1044544, 15.0, 65.0, typeof(Ginseng), 1044356, 3, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(GreaterHealPotion), 1116348, 1044545, 55.0, 105.0, typeof(Ginseng), 1044356, 7, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(LesserCurePotion), 1116348, 1044552, -10.0, 40.0, typeof(Garlic), 1044355, 1, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(CurePotion), 1116348, 1044553, 25.0, 75.0, typeof(Garlic), 1044355, 3, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(GreaterCurePotion), 1116348, 1044554, 65.0, 115.0, typeof(Garlic), 1044355, 6, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);



            // Enhancement
            index = AddCraft(typeof(AgilityPotion), 1116349, 1044540, 15.0, 65.0, typeof(Bloodmoss), 1044354, 1, 1044362);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(GreaterAgilityPotion), 1116349, 1044541, 35.0, 85.0, typeof(Bloodmoss), 1044354, 3, 1044362);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(NightSightPotion), 1116349, 1044542, -25.0, 25.0, typeof(SpidersSilk), 1044360, 1, 1044368);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(StrengthPotion), 1116349, 1044546, 25.0, 75.0, typeof(MandrakeRoot), 1044357, 2, 1044365);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(GreaterStrengthPotion), 1116349, 1044547, 45.0, 95.0, typeof(MandrakeRoot), 1044357, 5, 1044365);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);


            // Toxic
            index = this.AddCraft(typeof(LesserPoisonPotion), 1116350, 1044548, -5.0, 45.0, typeof(Nightshade), 1044358, 1, 1044366);
            this.AddSkill(index, SkillName.Poisoning, -1.0, 60.0);
            this.AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = this.AddCraft(typeof(PoisonPotion), 1116350, 1044549, 15.0, 65.0, typeof(Nightshade), 1044358, 2, 1044366);
            this.AddSkill(index, SkillName.Poisoning, 30.0, 70.0);
            this.AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = this.AddCraft(typeof(GreaterPoisonPotion), 1116350, 1044550, 55.0, 100.0, typeof(Nightshade), 1044358, 4, 1044366);
            this.AddSkill(index, SkillName.Poisoning, 60.0, 100.0);
            this.AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = this.AddCraft(typeof(DeadlyPoisonPotion), 1116350, 1044551, 90.0, 110.0, typeof(Nightshade), 1044358, 8, 1044366);
            this.AddSkill(index, SkillName.Poisoning, 95.0, 100.0);
            this.AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            // Explosive
            index = AddCraft(typeof(LesserExplosionPotion), 1116351, 1044555, 5.0, 55.0, typeof(SulfurousAsh), 1044359, 3, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(ExplosionPotion), 1116351, 1044556, 35.0, 85.0, typeof(SulfurousAsh), 1044359, 5, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(typeof(GreaterExplosionPotion), 1116351, 1044557, 65.0, 115.0, typeof(SulfurousAsh), 1044359, 10, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);



            if (Core.ML)
            {
                // Enhancement
                index = AddCraft(typeof(InvisibilityPotion), 1116349, 1074860, 65.0, 115.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(Bloodmoss), 1044354, 4, 1044362);
                AddRes(index, typeof(Nightshade), 1044358, 4, 1044366);
                AddRecipe(index, (int)TinkerRecipes.InvisibilityPotion);
                SetNeededExpansion(index, Expansion.ML);

                // Toxic
                index = AddCraft(typeof(ParasiticPotion), 1116350, 1072942, 65.0, 115.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(ParasiticPlant), 1073474, 5, 1044253);
                AddRecipe(index, (int)TinkerRecipes.ParasiticPotion);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkglowPotion), 1116350, 1072943, 65.0, 115.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(LuminescentFungi), 1073475, 5, 1044253);
                AddRecipe(index, (int)TinkerRecipes.DarkglowPotion);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ScouringToxin), 1116350, 1112292, 75.0, 100.0, typeof(ToxicVenomSac), 1112291, 1, 1044253);
                AddRes(index, typeof(Bottle), 1044529, 1, 500315);

                // Explosive
                index = AddCraft(typeof(ConflagrationPotion), 1116351, 1072096, 55.0, 105.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(GraveDust), 1023983, 5, 1044253);

                index = AddCraft(typeof(GreaterConflagrationPotion), 1116351, 1072099, 70.0, 120.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(GraveDust), 1023983, 10, 1044253);

                index = AddCraft(typeof(ConfusionBlastPotion), 1116351, 1072106, 55.0, 105.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(PigIron), 1023978, 5, 1044253);

                index = AddCraft(typeof(GreaterConfusionBlastPotion), 1116351, 1072109, 70.0, 120.0, typeof(Bottle), 1044529, 1, 500315);
                AddRes(index, typeof(PigIron), 1023978, 10, 1044253);
                
            }

            


            
        }
    }
}