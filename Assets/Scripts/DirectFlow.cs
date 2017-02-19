using UnityEngine;
using System.Collections;

public class DirectFlow : MonoBehaviour
{

    //Для импорта значений
    private Manager mgr;    //Ссылка на скрипт Manager

    public byte dir; //Направление движения (↑ = 0, → = 1, ↓ = 2, ← = 3)

    void Start()
    {
        mgr = GameObject.Find("Manager").GetComponent<Manager>();
        //transform.rotation = Quaternion.Set(0, 0, mgr.RotationEqualizer(transform.rotation.z));
        switch ((int)transform.eulerAngles.z)
        {
            case 0: dir = 0; break;
            case 90: dir = 3; break;
            case 180: dir = 2; break;
            case 270: dir = 1; break;
        }
    }

    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D col)
    {
        if ((Vector2)col.transform.position == (Vector2)transform.position)
            /*if (col.tag == "Player")
            {
                PlayerMove pm = col.GetComponent<PlayerMove>();
                pm.target = mgr.Target(col.gameObject, dir);
                pm.directFlowMoving = true;
            }
            else*/
            {                
                BlockMove bm = col.GetComponent<BlockMove>();
                bm.target = mgr.Target(col.gameObject, dir);
            }
    }
}
