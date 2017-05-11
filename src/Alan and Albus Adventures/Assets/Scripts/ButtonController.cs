using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonController : MonoBehaviour {

	public void LoadFreePlay()
	{
		SceneManager.LoadScene("Main");
	}
	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
