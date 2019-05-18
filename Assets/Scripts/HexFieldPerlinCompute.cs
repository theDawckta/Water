using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HexFieldPerlinCompute : MonoBehaviour {
    public float yOffset = 0.0f;
	[Header("X")]
	public int xSize = 20;
	public float xItemOffset = 0;
	public float xLineOffset = 0;
	[Header("Z")]
	public int zSize = 20;
	public float zItemOffset = 0;
	public float zLineOffset = 0;
	
	private Vector3[] fieldItemPosition;
	private bool playerFlattened = false;
	Quaternion zeroRotation = Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
	private Vector3 fieldCenter;
	private int vertexCount;
	private int fieldCount;
	public Mesh aMesh;
	public Material aMaterial;
	public float scale1 = 1.0f;
	public float heightScale1 = 1.0f;
	public float timeScale1 = 1.0f;
	public float scale2 = 1.0f;
	public float heightScale2 = 1.0f;
	public float timeScale2 = 1.0f;
	private Vector3 meshPosition = new Vector3(0.0f, 0.0f, 0.0f);

	private ComputeBuffer bufferPoints;
	private ComputeBuffer bufferNormals;
	private ComputeBuffer bufferPos;
    private Vector2 direction;

	void Awake () 
	{
		fieldItemPosition = new Vector3[xSize * zSize];
		
		MakeField ();
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            direction = direction + Vector2.up;
        float y;

        for (int i = 0; i < fieldCount; i++)
        {
            y = heightScale1 * Mathf.PerlinNoise(Math.Sign(direction.x) * (Time.time * timeScale1) + (fieldItemPosition[i].x * scale1), Math.Sign(direction.y) * (Time.time * timeScale1) + (fieldItemPosition[i].z * scale1));
            //y = y + heightScale2 * Mathf.PerlinNoise(Time.time * timeScale2 + (fieldItemPosition[i].x * scale2), Time.time * timeScale2 + (fieldItemPosition[i].z * scale2));
            fieldItemPosition[i] = new Vector3(fieldItemPosition[i].x, y + yOffset, fieldItemPosition[i].z);
        }
    }

    public void UpdateDirection(Vector2 newDirection)
    {
        direction = direction + newDirection;
    }

    private void MakeField()
	{
		var verts = GetTriangleVertices(aMesh);
		vertexCount = verts.Length;

		ReleaseBuffers ();
		
		bufferPoints = new ComputeBuffer (vertexCount, 12);
		bufferPoints.SetData (verts);
		aMaterial.SetBuffer ("buf_Points", bufferPoints);
		
		var normals = GetTriangleNormals (aMesh);
		bufferNormals = new ComputeBuffer (vertexCount, 12);
		bufferNormals.SetData(normals);
		aMaterial.SetBuffer ("buf_Normals", bufferNormals);

		bufferPos = new ComputeBuffer (xSize * zSize, 12);
		aMaterial.SetBuffer ("buf_Positions", bufferPos);

		float currentXOffset = 0;
		float currentZOffset = 0;
		
		float currentXLineOffset = 0;
		float currentZLineOffset = 0;

		Vector3 position;
		
		Vector3[] tempFieldItemPosition = new Vector3[xSize * zSize];
		
		int count = 0;
		for (int x = 1; x <= xSize; x++) 
		{
			for (int z = 1; z <= zSize; z++) 
			{
				currentXOffset = xItemOffset * x;
				currentZOffset = zItemOffset * z;
					
				if(x % 2 == 0)
				{
					currentXLineOffset = 0;
				}
				else
				{
					currentXLineOffset = xLineOffset;
				}
					
				if(z % 2 == 0)
				{
					currentZLineOffset = 0;
				}
					
				else
				{
					currentZLineOffset = zLineOffset;
				}
					
				position = new Vector3 (x + transform.localScale.x + currentXOffset + currentZLineOffset + transform.position.x, 
					                    yOffset, 
					                    z + transform.localScale.z + currentZOffset + currentXLineOffset + transform.position.z);
					
				tempFieldItemPosition[count] = position;
				count++;
					
				if((x == xSize / 2 || xSize == 1)
					&& (z == zSize / 2 || zSize == 1))
				{
					fieldCenter = position;
				}
			}
		}
		
		fieldItemPosition = new Vector3[count];
		
		for(int i = 0; i < count; i++)
		{
			fieldItemPosition[i] = tempFieldItemPosition[i];
		}
		
		for (int i = 0; i < fieldItemPosition.Length; i++)
		{
			fieldItemPosition[i] = new Vector3(fieldItemPosition[i].x - fieldCenter.x,
			                                   fieldItemPosition[i].y - fieldCenter.y,
			                                   fieldItemPosition[i].z - fieldCenter.z);
		}
		
		fieldCenter = new Vector3(0, 0, 0);
		fieldCount = fieldItemPosition.Length;
	}

	// called if script attached to the camera, after all regular rendering is done
	void OnPostRender () {
		bufferPos.SetData (fieldItemPosition);
		aMaterial.SetPass (0);
		Graphics.DrawProceduralNow (MeshTopology.Triangles, vertexCount, xSize * zSize);
	}

	Vector3[] GetTriangleVertices(Mesh aMesh)
	{
		Vector3[] vertices = new Vector3[aMesh.triangles.Length];
		for (int i = 0; i < aMesh.triangles.Length; i += 3) {
			vertices[i + 0] = aMesh.vertices [aMesh.triangles [i + 0]];
			vertices[i + 1] = aMesh.vertices [aMesh.triangles [i + 1]];
			vertices[i + 2] = aMesh.vertices [aMesh.triangles [i + 2]];
		}
		
		return vertices;
	}
	
	Vector3[] GetTriangleNormals(Mesh aMesh)
	{
		Vector3[] normals = new Vector3[aMesh.triangles.Length];
		for (int i = 0; i < aMesh.triangles.Length; i += 3) {
			Vector3 a = aMesh.vertices [aMesh.triangles [i + 0]];
			Vector3 b = aMesh.vertices [aMesh.triangles [i + 1]];
			Vector3 c = aMesh.vertices [aMesh.triangles [i + 2]];
			Vector3 normal = Vector3.Cross(b - a, c - a);
			normal.Normalize();
			normals[i + 0] = normal;
			normals[i + 1] = normal;
			normals[i + 2] = normal;
		}
		
		return normals;
	}
	
	private void ReleaseBuffers() {
		if (bufferPoints != null) bufferPoints.Release();
		bufferPoints = null;
		if (bufferPos != null) bufferPos.Release();
		bufferPos = null;
		if (bufferNormals != null) bufferNormals.Release();
		bufferNormals = null;
	}
	
	void OnDisable() {
		ReleaseBuffers ();
	}
}
