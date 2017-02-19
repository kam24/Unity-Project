using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

    // Use this for initialization
    public byte dir=4;
    public bool pressed = false;

    void Awake()
    {
        switch ((int)transform.eulerAngles.z)
        {
            case 0: dir = 0; break;           
            case 270: dir = 1; break;
            case 180: dir = 2; break;
            case 90: dir = 3; break;
        }
    }

    void OnMouseUp()
    {
        pressed = true;
        Debug.Log(pressed);
    }
    void OnMouseExit()
    {
        pressed = false;
    }
}
