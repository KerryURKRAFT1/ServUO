using System;

namespace Server.Items
{


        
    // BAR EST

        [Flipable(0x1918, 0x1919, 0x191C, 0x191D)] // ID grafico per due versioni
        
        public class BarTableEast : CraftableFurniture
        {
            [Constructable]
            public BarTableEast() : base (0x191C)
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar"; // Nome dell'oggetto
            }

            public BarTableEast(Serial serial)
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


        public class BarTableEast2 : CraftableFurniture
        {
            [Constructable]
            public BarTableEast2() : base (0x191D)
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar"; // Nome dell'oggetto
            }

            public BarTableEast2(Serial serial)
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



        // BAR WEST


        [Flipable( 0x1919, 0x191B)] // ID grafico per due versioni
        public class BarTableWest : CraftableFurniture
        {
                [Constructable]
                public BarTableWest() : base (0x1918)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar"; // Nome dell'oggetto
                }

                public BarTableWest(Serial serial)
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


        public class BarTableWest2 : CraftableFurniture
        {
                [Constructable]
                public BarTableWest2() : base (0x1919)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar"; // Nome dell'oggetto
                }

                public BarTableWest2(Serial serial)
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



        

    // BAR CORNER NORTH

        [Flipable( 0x1913, 0x1911)] 
        public class BarCornerSouthE : CraftableFurniture
        {
            [Constructable]
            public BarCornerSouthE() : base (0x1913)
            
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar"; // Nome dell'oggetto
            }

            public BarCornerSouthE(Serial serial)
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


        public class BarCornerNorthE : CraftableFurniture
        {
            [Constructable]
            public BarCornerNorthE() : base (0x1911)
            
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar"; // Nome dell'oggetto
            }

            public BarCornerNorthE(Serial serial)
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

        // bar corner south 



       [Flipable( 0x1911, 0x1912)] 
        public class BarCornerSouthW : CraftableFurniture
        {

            [Constructable]
            public  BarCornerSouthW() : base (0x1912)
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar"; // Nome dell'oggetto
            }

            public BarCornerSouthW(Serial serial)
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


        public class BarCornernNorthW : CraftableFurniture
        {

            [Constructable]
            public  BarCornernNorthW() : base (0x1910)
            {
                Weight = 10.0; // Peso dell'oggetto
                Name = "Bar"; // Nome dell'oggetto
            }

            public BarCornernNorthW(Serial serial)
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




        // BAR NORTH
        [Flipable( 0x191E, 0x191F)] // ID grafico per due versioni
        public class BarTableNorth : CraftableFurniture
        {
                [Constructable]
                public BarTableNorth() : base (0x191E)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar"; // Nome dell'oggetto
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


        public class BarTableNorth2 : CraftableFurniture
        {
                [Constructable]
                public BarTableNorth2() : base (0x191F)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar"; // Nome dell'oggetto
                }

                public BarTableNorth2(Serial serial)
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


        // BAR SOUTH

        [Flipable( 0x191A, 0x191B)] // ID grafico per due versioni
        public class BarTableSouth : CraftableFurniture
        {
                [Constructable]
                public BarTableSouth() : base (0x191A)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar"; // Nome dell'oggetto
                }

                public BarTableSouth(Serial serial)
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


        public class BarTableSouth2 : CraftableFurniture
        {
                [Constructable]
                public BarTableSouth2() : base (0x191B)
                {
                    Weight = 10.0; // Peso dell'oggetto
                    Name = "Bar"; // Nome dell'oggetto
                }

                public BarTableSouth2(Serial serial)
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





