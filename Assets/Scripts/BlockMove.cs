using UnityEngine;
using System.Collections;

public class BlockMove : MonoBehaviour
{

    //Для импорта значений
    private Manager mgr;    //Ссылка на скрипт Manager
    private float speed, delta;   //Скорость передвижения

    //Установка целей
    public Vector2 target; //Цель, к которой будет двигаться блок
    public byte dir;       //Направление движения (↑ = 0, → = 1, ↓ = 2, ← = 3)
    private Vector2 lastTarget;
    public bool directFlowMoving = false;

    private GameObject syncLabel;
    public byte enableLabel = 0;
    [SerializeField]
    private Sprite image, image2,selfImage;

    void Awake()
    {
        gameObject.GetComponent<Block>().enabled = false;
        mgr = GameObject.Find("Manager").GetComponent<Manager>();

        speed = mgr.speed;  //импорт значения скорости из Manager 
        delta = mgr.delta;

        selfImage = GetComponent<SpriteRenderer>().sprite;
    }

    void Start()
    {
        lastTarget = target = transform.position;   //выставление значений по умолчанию
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
        ToTarget();
        SyncLabel(enableLabel);
    }

    void FixedUpdate()
    {

    }

    void ToTarget()//Перемещение к цели
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x,target.y,transform.position.z), Time.deltaTime * speed);
    }

    public void SyncLabel(byte enableLabel)
    {
        if (gameObject.tag != "Player")
        {
            switch (enableLabel)
            {
                case 1:
                    GetComponent<SpriteRenderer>().sprite = image;
                    goto Checking;
                case 2:
                    GetComponent<SpriteRenderer>().sprite = image2;
                    goto Checking;
                case 0:
                    syncLabel = null;
                    GetComponent<SpriteRenderer>().sprite = selfImage;
                    break;                   
            }

            Checking:
                foreach (GameObject player in mgr.Players)
                {
                    Vector2 pos = transform.position;
                    Vector2 posPlayer = transform.position;
                    PlayerMove pm = player.GetComponent<PlayerMove>();
                    float x = 0, y = 0;
                    switch (System.Math.Abs(pm.syncDir - 2))
                    {
                        case 0: y = delta; if (pos.y + y <= posPlayer.y) GetComponent<SpriteRenderer>().sprite = selfImage; break;
                        case 1: x = delta; if (pos.x + x <= posPlayer.x) GetComponent<SpriteRenderer>().sprite = selfImage; break;
                        case 2: y = -delta; if (pos.y + y >= posPlayer.y) GetComponent<SpriteRenderer>().sprite = selfImage; break;
                        case 3: x = -delta; if (pos.x + x <= posPlayer.x) GetComponent<SpriteRenderer>().sprite = selfImage; break;
                    }
                }
        }
    }
}

