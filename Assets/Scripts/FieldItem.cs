using UnityEngine;
using System.Collections;

public class FieldItem
{
	public float Height; 
    public Vector3 Position = Vector3.zero;
    public Vector2 Index;
    public float MaxHeight = 50.0f;
    public float SpeedDampening = 0.85f;
    public float Speed;
    
    public FieldItem(Vector3 position)
    {
        Position = position;
    }

    public void AddForce(float force)
    {
        Speed += force;
    }

    public void UpdateFieldItem()
    {
        Height = Mathf.Clamp(Height + Speed, -MaxHeight, MaxHeight);
        Position = new Vector3(Position.x, Height, Position.z);
		Speed *= SpeedDampening;
    }
}
