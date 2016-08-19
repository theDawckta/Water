using UnityEngine;
using System.Collections;

public class FieldItem : MonoBehaviour {

    [HideInInspector]
	public float height; 
	[HideInInspector]
    public Vector3 location = Vector3.zero;
	[HideInInspector]
    public Vector2 index;
    public float MaxHeight;
    public float SpeedDampening;
    float speed;

    public void AddForce(float force)
    {
        speed += force;
    }

    public void Update()
    {
        height = Mathf.Clamp(height + speed, -MaxHeight, MaxHeight);
        location = new Vector3(location.x, height, location.z);
		speed *= SpeedDampening;
    }
}
