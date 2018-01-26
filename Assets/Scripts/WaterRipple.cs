using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterRipple : MonoBehaviour
{
    private ParticleSystem _waterParticleSystem;
    ParticleSystem.Particle[] _waterParticles;
    public Mesh WaterCubeMesh;
    public Material WaterMaterial;
    
    public List<AudioSource> Clips = new List<AudioSource>();
    private int _clipIndex = 0;

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
    public float SplashForceDrip = 0.5f;
    public float SplashForceMouse = 3.0f;
    public float MaxSplashHeight = 5.0f;
    public float SplashRadius = 1.0f;
    
    private FieldItem[,] fieldItems;
	//private FieldItem[,] fieldItemLocations;
    private Vector3 fieldCenter = new Vector3();
    private float timePassed = 0.0f;
    private BoxCollider surfaceBoxCollider;
    private float _buttonDownStartTime;

    float level = 0.0f;

	public Mesh mesh;

    void Awake()
    {
        Application.targetFrameRate = 60;
        _waterParticles = new ParticleSystem.Particle[XSize * ZSize];
        var go = new GameObject("Particle System");
        go.transform.SetParent(transform, false);
        _waterParticleSystem = go.AddComponent<ParticleSystem>();
        surfaceBoxCollider = go.AddComponent<BoxCollider>();
        surfaceBoxCollider.size = new Vector3(XSize, 0.1f, ZSize);

        ParticleSystem.MainModule main =  _waterParticleSystem.main;
        main.maxParticles = XSize * ZSize;
        main.startSpeed = 0.0f;
        main.startLifetime = Mathf.Infinity;
        main.startSize = 1.5f;

        ParticleSystem.EmissionModule emission = _waterParticleSystem.emission;
        emission.rateOverTime = XSize * ZSize;

        ParticleSystemRenderer renderer = _waterParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Mesh;
        renderer.mesh = WaterCubeMesh;
        renderer.material = WaterMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.alignment = ParticleSystemRenderSpace.World;
        
        _waterParticleSystem.Emit(XSize * ZSize);


        fieldItems = new FieldItem[XSize, ZSize];
		//fieldItemLocations = new FieldItem[XSize, ZSize];
        MakeField();

        //InvokeRepeating("RandomSplash", 0.0f, 0.05f);
    }

    void Update()
    {
		Fluid();
        Draw();
        timePassed = timePassed + Time.deltaTime;

		if(Input.GetMouseButton(0))
		{
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Bounds b = new Bounds(hit.point, new Vector3(SplashRadius, SplashRadius, SplashRadius));

                for (int z = 1; z < ZSize - 1; z++)
                {
                    for (int x = 1; x < XSize - 1; x++)
                    {
                        if (b.Contains(new Vector3(fieldItems[x, z].Position.x + transform.position.x, 0.0f, fieldItems[x, z].Position.z + transform.position.z)))
                        {
                            fieldItems[x, z].AddForce(SplashForceMouse);
                        }
                    }
                }
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    _buttonDownStartTime = Time.time;
        //    Clips[_clipIndex].Play();
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    Clips[_clipIndex].Stop();
        //    if ((Time.time - _buttonDownStartTime) > Clips[_clipIndex].clip.length)
        //        NextClip();
        //}
    }

    void NextClip()
    {
        _clipIndex = _clipIndex + 1;
        if (_clipIndex >= Clips.Count)
            _clipIndex = 0;
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
                hDiff = fieldItems[x - 1, z].Height - fieldItems[x, z].Height;
                hForce += Damping * hDiff;
                hDiff = fieldItems[x + 1, z].Height - fieldItems[x, z].Height;
                hForce += Damping * hDiff;
                hDiff = fieldItems[x, z - 1].Height - fieldItems[x, z].Height;
                hForce += Damping * hDiff;
                hDiff = fieldItems[x, z + 1].Height - fieldItems[x, z].Height;
                hForce += Damping * hDiff;
                // influence of normal waterlevel
                hDiff = level - fieldItems[x, z].Height;
                hForce += Damping * hDiff;
                // apply force and update
                fieldItems[x, z].AddForce(hForce);
                fieldItems[x, z].UpdateFieldItem();
            }
        }
    }

    void Draw()
    {
        int fieldSize = _waterParticleSystem.GetParticles(_waterParticles);
        int particleIndex = 0;
        int x;
        int y;

        for (x = 0; x < XSize; x++)
        {
            for (y = 0; y < ZSize; y++)
            {
            	
                Vector3 newPosition = new Vector3(fieldItems[x, y].Position.x,
                                                  fieldItems[x, y].Height,
					                              fieldItems[x, y].Position.z);

                fieldItems[x, y].Position = newPosition;

                
                _waterParticles[particleIndex].position = fieldItems[x, y].Position;
                particleIndex = particleIndex + 1;
            }
        }

        _waterParticleSystem.SetParticles(_waterParticles, fieldSize);
    }

    void RandomSplash()
    {
        int randomX = Random.Range(1, XSize - 1);
        int randomY = Random.Range(1, ZSize - 1);

        fieldItems[randomX, randomY].AddForce(SplashForceDrip);   
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

                FieldItem fieldItem = new FieldItem(position, MaxSplashHeight);

                fieldItems[x, z] = fieldItem;
                fieldItems[x, z].Index = new Vector2(x, z);

                if ((x == XSize / 2 || XSize == 1) && (z == ZSize / 2 || ZSize == 1))
                {
                    fieldCenter = fieldItems[x, z].Position;
                }
            }
        }

        for (int x = 0; x < XSize; x++)
        {
            for (int z = 0; z < ZSize; z++)
            {
                fieldItems[x, z].Position = new Vector3(fieldItems[x, z].Position.x - fieldCenter.x,
                                                             fieldItems[x, z].Position.y - fieldCenter.y,
                                                             fieldItems[x, z].Position.z - fieldCenter.z);
            }
        }
    }
}