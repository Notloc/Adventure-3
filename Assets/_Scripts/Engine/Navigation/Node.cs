[System.Serializable]
public struct Node
{
    public byte data;

    /* 8 Bits
     *  
     *      7 6 5 4 3 2 1 0
     *      
     *      0 - 3   represent if walls are present
     *      4       represents if the node is pathable
     *      5 - 7   currently unused
     */
    

    //Bit 0

    //Bit 1

    //Bit 2



    //Bit 4
    public bool IsPathable()
    {
        //Bit-shift bit 4 into position 0
        //Use '& 1' to get rid of the rest of the data
        //Return result
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
        if(!state)
        {
            //Use XOR to ensure data's 4th position becomes 0
            data = (byte)(data ^ update);
        }
    }

}
