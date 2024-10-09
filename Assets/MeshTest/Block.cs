using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
	public short index;
	public byte state;
    
    public Block(short index, byte state) 
    {
        this.index = index;
        this.state = state;
    }
}
