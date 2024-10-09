using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public int seed;
    
	public void GetRandom()
	{
		System.Random random = new System.Random(seed);
		
		
		Debug.Log(random.Next(-10000, 10000));
		Debug.Log(random.Next(-10000, 10000));
	}
}
