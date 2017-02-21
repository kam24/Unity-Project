using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        if (gameObject.name == "Play")
            SceneManager.LoadScene("level01");
        if (gameObject.name == "Select level")
            SceneManager.LoadScene("Select level");
        if (gameObject.name == "Controls")
            SceneManager.LoadScene("Controls");
        if (gameObject.name == "Help")
            SceneManager.LoadScene("Help");
        if (gameObject.name == "Exit")
            Application.Quit();
        if (gameObject.name == "Back")
            SceneManager.LoadScene("Main Menu");
    }
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 250, 25), "Made By Kamil Zalyalov");
    }
}
