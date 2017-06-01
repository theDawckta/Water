using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterRipple2 : MonoBehaviour {
	
	[Header("X")]
	public int xSize = 5;
	[Header("Z")]
	public int zSize = 5;

	public float damping = 0.999f;
	public float maxWaveHeight = 2.0f;
    public int Height;
	public int splashForce = 1000;

	public GameObject fieldItem;
	public GameObject[,] fieldItems;

	private int[,] buffer1;
	private int[,] buffer2;
	private int[,] currentBuffer;
	private bool swapCurrent;
	private int count = 0;
	void Awake () {
		buffer1 = new int[xSize,zSize];
		buffer2 = new int[xSize,zSize];
		fieldItems = new GameObject[xSize,zSize];
		MakeField ();
	}
	
	private void MakeField()
	{
		Vector3 position;
		for (int x = 0; x < xSize; x++)
		{
			for (int z = 0; z < zSize; z++)
			{
				position = new Vector3 (x,0,z);
				
				GameObject newFieldItem = (GameObject)Instantiate(fieldItem, position, Quaternion.identity);
				newFieldItem.transform.parent = gameObject.transform;
				fieldItems[x,z] = newFieldItem;
			}
		}
	}

	void FixedUpdate()
	{
		checkInput ();

		if (swapCurrent) 
		{
			ProcessWater (ref buffer1, ref buffer2);
			currentBuffer = buffer2;
		} 
		else 
		{
			ProcessWater (ref buffer2, ref buffer1);		
			currentBuffer = buffer1;
		}
		swapCurrent = !swapCurrent;
		RenderWater (currentBuffer);
	}

	void ProcessWater(ref int[,] source, ref int[,] dest)
	{
		int x;
		int y;
		
		for(x = 1; x < xSize - 1; x++)
		{
			for(y = 1; y < zSize - 1; y++)
			{
				dest[x, y] = (source[x-1,y] +
				              source[x+1,y] +
				              source[x,y+1] +
				              source[x,y-1]) / 2 - dest[x,y];
				
				dest[x,y] = (int)(dest[x,y] * damping);
			}
		}
	}

	void RenderWater(int[,] currentBuffer)
	{
		int x;
		int y;

		for(x = 0; x < xSize; x++)
		{
			for(y = 0; y < zSize; y++)
			{
				float newY = (currentBuffer[x,y] * 1.0f/splashForce) * maxWaveHeight;
				Vector3 newPosition = new Vector3(fieldItems[x,y].transform.localPosition.x,
				                               newY,
				                               fieldItems[x,y].transform.localPosition.z);
				fieldItems[x,y].transform.localPosition = newPosition;
			}
		}
	}

	void checkInput()
    {	
		if (Input.GetMouseButton (0))
        {
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
				if(hit.transform.parent.name == "WaterField")
				{
                    splashAtPoint((int)hit.transform.position.x, (int)hit.transform.position.z);
                }
			}
		}
	}

	void splashAtPoint(int x, int y)
    {
        try
        {
            buffer1[x, y] = splashForce;
            buffer1[x, y - 1] = splashForce;
            buffer1[x, y + 1] = splashForce;
            buffer1[x - 1, y] = splashForce;
            buffer1[x + 1, y] = splashForce;
        }
        catch
        {
        }
	}
}
