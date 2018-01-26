using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour 
{

	// Update is called once per frame
	void Update () 
	{
	 	if(Input.GetKey(KeyCode.R))
	 	{
			Scene scene = SceneManager.GetActiveScene(); 
			SceneManager.LoadScene(scene.name);
	 	}
	}
}
