using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour 
{
    public HexFieldPerlinCompute Terrain;

    private Vector2 _direction;

	// Update is called once per frame
	void Update () 
	{
	 	if(Input.GetKey(KeyCode.R))
	 	{
			Scene scene = SceneManager.GetActiveScene(); 
			SceneManager.LoadScene(scene.name);
	 	}

        //_direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //Debug.Log(_direction);
        //Terrain.UpdateDirection(_direction);
	}
}
