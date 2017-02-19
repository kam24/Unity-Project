using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour {

    private Vector2 tp;
    private Manager mgr;    //Ссылка на скрипт Manager
    public bool filled = false;

    void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 2);
        mgr = GameObject.Find("Manager").GetComponent<Manager>();
    }

    void Start()
    {
        tp = transform.position;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (filled == false)
        {
            Vector2 colPos = col.transform.position;
            if (tp == colPos)
            {
                if (col.tag == "Relocatable")
                {
                    GameObject go = col.gameObject;
                    for (int i = 0; i < mgr.AllRelocatableBlocks.Count; i++)
                        if (go == mgr.AllRelocatableBlocks[i])
                            mgr.AllRelocatableBlocks.RemoveAt(i);
                    col.tag = "Floor";
                    mgr.Floor.Add(go);
                    go.GetComponent<BoxCollider2D>().enabled = false;
                    go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, 1);
                    filled = true;
                }
            }
        }
    }
}
