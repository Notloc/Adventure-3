namespace Adventure.Engine.Navigation.Internal
{
    [System.Serializable]
    public struct Node
    {
        public byte data;

        /* 8 Bits
         *  
         *      Byte as bits -> [7 6 5 4 3 2 1 0]
         *      
         *      0 - 3   represent walls
         *      4       represent pathability
         *      5 - 7   unused
         */


        //Bit 0 - North Wall
        public bool HasWallNorth()
        {
            return (data & 1) == 1;
        }

        //Bit 1 - East Wall
        public bool HasWallEast()
        {
            return ((data >> 1) & 1) == 1;
        }

        //Bit 2
        public bool HasWallSouth()
        {
            return ((data >> 2) & 1) == 1;
        }

        //Bit 3
        public bool HasWallWest()
        {
            return ((data >> 3) & 1) == 1;
        }

        //Bit 4
        public bool IsPathable()
        {
            return ((data >> 4) & 1) == 1;
        }


        public void SetPathable(bool state)
        {
            //Get a 1 into the 4th position of update
            byte update = 1;
            update = (byte)(update << 4);

            //Use OR to ensure data's 4th position is a 1
            data = (byte)(data | update);

            //If 0 is desired
            if (!state)
            {
                //Use XOR to ensure data's 4th position becomes 0
                data = (byte)(data ^ update);
            }
        }
    }
}