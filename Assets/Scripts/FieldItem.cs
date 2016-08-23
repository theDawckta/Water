using UnityEngine;
using System.Collections;

public class FieldItem : MonoBehaviour {

    [HideInInspector]
	public float height;
    [HideInInspector]
    public float length;
    [HideInInspector]
    public float depth; 
	[HideInInspector]
    public Vector3 location = Vector3.zero;
    [HideInInspector]
    public Vector3 OriginalLocation = Vector3.zero;
	[HideInInspector]
    public Vector2 index;
    public float MaxHeight;
    public float SpeedDampening;
    float heightSpeed;
    float lengthSpeed;
    float depthSpeed;

    public void AddForce(Vector3 force)
    {
        lengthSpeed += force.x;
        heightSpeed += force.y;
        depthSpeed += force.z;
    }

    public void Update()
    { 
        length = Mathf.Clamp(length + lengthSpeed, -20, 20);
        height = Mathf.Clamp(height + heightSpeed, -MaxHeight, MaxHeight);
        depth = Mathf.Clamp(depth + depthSpeed, -20, 20);
        location = new Vector3(OriginalLocation.x + length, height, OriginalLocation.z + depth); 
        lengthSpeed *= SpeedDampening;
		heightSpeed *= SpeedDampening;
        depthSpeed *= SpeedDampening;
    }
}
