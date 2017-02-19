using UnityEngine;
using System;
using System.Collections;
[ExecuteInEditMode]

public class Block : MonoBehaviour {
    private Manager mgr;    //Ссылка на скрипт Manager

    void Awake()
    {
        mgr = GameObject.Find("Manager").GetComponent<Manager>();
        transform.position = mgr.Equalizer(transform.position); //выравнивание позиции блока
    }

    void Start()
    {

    }
    void Update()
    {
        if (transform.position != mgr.Equalizer(transform.position))
            transform.position = mgr.Equalizer(transform.position);//выравнивание позиции блока
    }
}



