using System;

namespace Server.Items
{


        
        [Flipable(0x1918, 0x1919, 0x191C, 0x191D)] // ID grafico per due versioni
        
        public class BarTablesWest : CraftableFurniture
        {
            [Constructable]
            public BarTablesWest() : base (0x1918)
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar Table West"; // Nome dell'oggetto
            }

            public BarTablesWest(Serial serial)
                : base(serial)
            {
            }


            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0); // Versione
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }

        }

    // BAR north

        [Flipable( 0x191A, 0x191B, 0x191E, 0x191F)] // ID grafico per due versioni
        public class BarTableNorth : CraftableFurniture
        {
                [Constructable]
                public BarTableNorth() : base (0x191A)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar Corner North"; // Nome dell'oggetto
                }

                public BarTableNorth(Serial serial)
                    : base(serial)
                {
                }


                public override void Serialize(GenericWriter writer)
                {
                    base.Serialize(writer);
                    writer.Write(0); // Versione
                }

                public override void Deserialize(GenericReader reader)
                {
                    base.Deserialize(reader);
                    int version = reader.ReadInt();
                }
        }
        

    // BAR CORNER
        public class BarCornerNorth : CraftableFurniture
        {
            [Constructable]
            public BarCornerNorth() : base (0x1913)
            
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar Table North"; // Nome dell'oggetto
            }

            public BarCornerNorth(Serial serial)
                : base(serial)
            {
            }


            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0); // Versione
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }

        // bar corner 2



       [Flipable( 0x1911, 0x1912, 0x1910)] 
        public class BarCorner : CraftableFurniture
        {

            [Constructable]
            public  BarCorner() : base (0x1911)
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar Corner"; // Nome dell'oggetto
            }

            public BarCorner(Serial serial)
                : base(serial)
            {
            }


            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0); // Versione
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
}





