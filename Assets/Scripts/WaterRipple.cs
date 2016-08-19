using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterRipple : MonoBehaviour
{

    [Header("X")]
    public int XSize = 5;
    public float XItemOffset = 0;
    public float XLineOffset = 0;
    [Header("Z")]
    public int ZSize = 5;
    public float ZItemOffset = 0;
    public float ZLineOffset = 0;

    [Header("Water")]
    public float Damping = 0.07f;
    public float SplashForce = 0.5f;
    public float SplashInterval = 0.2f;

    public GameObject MyFieldItem;
    private FieldItem[,] fieldItems;
	private FieldItem[,] fieldItemLocations;
    private Vector3 fieldCenter = new Vector3();
    private float timePassed = 0.0f;

    float level = 0.0f;



	public Mesh mesh;
    public Material waterMaterial;

    void Awake()
    {
        fieldItems = new FieldItem[XSize, ZSize];
		fieldItemLocations = new FieldItem[XSize, ZSize];
		for (int x = 0; x < XSize; x++)
        {
            for (int z = 0; z < ZSize; z++)
            {
				fieldItemLocations[x,z] = new FieldItem();
            }
      	}
        MakeField();
    }

    void Update()
    {
		Fluid();
        Draw();
        timePassed = timePassed + Time.deltaTime;

		if(Input.touches.Length > 0)
        {
        	foreach(Touch touch in Input.touches)
        	{
				RaycastHit hit;
        		Ray ray = Camera.main.ScreenPointToRay(touch.position);
        
		        if (Physics.Raycast(ray, out hit)) {
		            Transform objectHit = hit.transform;
		            
		            // Do something with the object that was hit by the raycast.
		        }
        	}
        }

		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
        		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
		        if (Physics.Raycast(ray, out hit)) 
		        {
		            Transform objectHit = hit.transform;
		            FieldItem fieldItemHit = hit.transform.GetComponent<FieldItem>();
					if((int)fieldItemHit.index.x != 0 && (int)fieldItemHit.index.x != XSize - 1 && (int)fieldItemHit.index.y != 0 && (int)fieldItemHit.index.y != ZSize - 1)
					{
						fieldItems[(int)fieldItemHit.index.x, (int)fieldItemHit.index.y].AddForce(SplashForce);
					}
		        }
		}
    }

    void Fluid()
    {
        for (int z = 1; z < ZSize - 1; z++)
        {
            for (int x = 1; x < XSize - 1; x++)
            {
                float hDiff = 0;
                float hForce = 0;
                // influences of neighbours
                hDiff = fieldItems[x - 1, z].height - fieldItems[x, z].height;
                hForce += Damping * hDiff;
                hDiff = fieldItems[x + 1, z].height - fieldItems[x, z].height;
                hForce += Damping * hDiff;
                hDiff = fieldItems[x, z - 1].height - fieldItems[x, z].height;
                hForce += Damping * hDiff;
                hDiff = fieldItems[x, z + 1].height - fieldItems[x, z].height;
                hForce += Damping * hDiff;
                // influence of normal waterlevel
                hDiff = level - fieldItems[x, z].height;
                hForce += Damping * hDiff;
                // apply force and update

                fieldItems[x, z].AddForce(hForce);
                fieldItems[x, z].Update();
            }
        }
    }

	void FluidFieldItems()
    {
        for (int z = 1; z < ZSize - 1; z++)
        {
            for (int x = 1; x < XSize - 1; x++)
            {
                float hDiff = 0;
                float hForce = 0;
                // influences of neighbours
				hDiff = fieldItemLocations[x - 1, z].height - fieldItemLocations[x, z].height;
                hForce += Damping * hDiff;
				hDiff = fieldItemLocations[x + 1, z].height - fieldItemLocations[x, z].height;
                hForce += Damping * hDiff;
				hDiff = fieldItemLocations[x, z - 1].height - fieldItemLocations[x, z].height;
                hForce += Damping * hDiff;
				hDiff = fieldItemLocations[x, z + 1].height - fieldItemLocations[x, z].height;
                hForce += Damping * hDiff;
                // influence of normal waterlevel
				hDiff = level - fieldItemLocations[x, z].height;
                hForce += Damping * hDiff;
                // apply force and update
				fieldItemLocations[x, z].AddForce(hForce);
				fieldItemLocations[x, z].Update();
            }
        }
    }


    void Draw()
    {
        int x;
        int y;

        for (x = 0; x < XSize; x++)
        {
            for (y = 0; y < ZSize; y++)
            {
            	
                Vector3 newPosition = new Vector3(fieldItems[x, y].transform.localPosition.x,
                                                    fieldItems[x, y].height,
					fieldItems[x, y].transform.localPosition.z);
					fieldItems[x, y].transform.localPosition = newPosition;


//				Vector3 newPosition = new Vector3(fieldItemLocations[x, y].location.x,
//                                                  fieldItemLocations[x, y].height,
//												  fieldItemLocations[x, y].location.z);
//				Graphics.DrawMesh(mesh, newPosition, Quaternion.identity, material, 0);
            }
        }

    }

    void RandomSplash()
    {
        int randomX = Random.Range(1, XSize - 1);
        int randomY = Random.Range(1, ZSize - 1);

        fieldItems[randomX, randomY].AddForce(SplashForce);

//		fieldItemLocations[randomX, randomY].AddForce(SplashForce);    
	}

    private void MakeField()
    {
        float currentXOffset = 0;
        float currentZOffset = 0;

        float currentXLineOffset = 0;
        float currentZLineOffset = 0;

        Vector3 position;
        for (int x = 0; x < XSize; x++)
        {
            for (int z = 0; z < ZSize; z++)
            {
                currentXOffset = XItemOffset * x;
                currentZOffset = ZItemOffset * z;

                if (x % 2 == 0)
                {
                    currentXLineOffset = 0;
                }
                else
                {
                    currentXLineOffset = XLineOffset;
                }

                if (z % 2 == 0)
                {
                    currentZLineOffset = 0;
                }

                else
                {
                    currentZLineOffset = ZLineOffset;
                }

                position = new Vector3(x + transform.localScale.x + currentXOffset + currentZLineOffset + transform.position.x,
                                        1.0f,
                                        z + transform.localScale.z + currentZOffset + currentXLineOffset + transform.position.z);

                fieldItemLocations[x, z].location = position ;

                GameObject fieldItem = (GameObject)Instantiate(MyFieldItem, position, transform.rotation);
               	fieldItem.GetComponent<Renderer>().material = waterMaterial;

                fieldItems[x, z] = fieldItem.GetComponent<FieldItem>();
                fieldItems[x, z].index = new Vector2(x, z);
                fieldItems[x, z].transform.parent = gameObject.transform;

                if ((x == XSize / 2 || XSize == 1) && (z == ZSize / 2 || ZSize == 1))
                {
                    fieldCenter = fieldItems[x, z].transform.position;
//					fieldCenter = fieldItemLocations[x, z].location;
                }
            }
        }

        for (int x = 0; x < XSize; x++)
        {
            for (int z = 0; z < ZSize; z++)
            {
					fieldItems[x, z].transform.position = new Vector3(fieldItems[x, z].transform.position.x - fieldCenter.x,
                                                                 fieldItems[x, z].transform.position.y - fieldCenter.y,
                                                                 fieldItems[x, z].transform.position.z - fieldCenter.z);
            	
//				fieldItemLocations[x, z].location = new Vector3(fieldItemLocations[x, z].location.x - fieldCenter.x,
//																  fieldItemLocations[x, z].location.y - fieldCenter.y,
//																  fieldItemLocations[x, z].location.z - fieldCenter.z);
            }
        }

        fieldCenter = new Vector3(0, 0, 0);
        MyFieldItem.SetActive(false);
    }
}