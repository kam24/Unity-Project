using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {
    void Start()
    {
    }
    void OnMouseUp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}
