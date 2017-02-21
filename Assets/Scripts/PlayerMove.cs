using UnityEngine;
using System;
using System.Collections;
public class PlayerMove : MonoBehaviour
{ 

    //Для импорта значений
    private Manager mgr;    //Ссылка на скрипт Manager
    private BlockMove bm;   //Ссылка на скрипт BlockMove текущего объекта
    private float delta;

    //Установка целей
    public byte dir; //Направление движения (↑ = 0, → = 1, ↓ = 2, ← = 3)
    public bool flowTargetX = false, flowTargetY = false, directFlowMoving = false, targetAccess=true;
    public Vector2 target;
    public byte anotherDir;
    private GameObject[] buttons;
    private Button[] bts = new Button[4];

    //Синхронизированное движение
    private GameObject syncBlock, go;
    private bool syncEstablishment = false;
    public byte syncDir = 4;
    private BlockMove bmSync;
    public Vector2 lastTarget;
    byte choosedDir=4;
    int blocksSum = 0;
    bool choosingSyncBlock = false;
    GameObject[] syncBlocks = new GameObject[4];


    //Остальное

    void Awake()
    {
        gameObject.GetComponent<Block>().enabled = false;
        mgr = GameObject.Find("Manager").GetComponent<Manager>();
        bm = gameObject.GetComponent<BlockMove>();
        delta = mgr.delta;
    }
    void Start()
    {
        lastTarget = bm.target  = transform.position;        
    }

    void Update()
    {         
        
        BlockSyncronisation();
         
        ChooseSyncBlockOfFour();

        MotionControl(); //Функция, реагирующая на вводимые значения клавиш, и, перемещающая игрока в соответствующем направлении
        SyncBlockControl(); //Функция, управляющая синхронизируемым блоком
                
        mgr.wrongDir = 4;
        lastTarget = bm.target;        
    }

    void SynchronizationEstablishment() //Метод, создающий синхронизацию движения рядом стоящего блока с движением игрока
    {        
        Vector2 tp = mgr.Equalizer(transform.position);
        syncBlocks = new GameObject[4];
        blocksSum = 0;
        choosedDir = 4;
        foreach (GameObject block in mgr.AllRelocatableBlocks) {            
            Vector2 posBlock = block.transform.position;
            for (byte i = 0; i < 4; i++)
            {
                float x = 0, y = 0;
                switch (i)
                {
                    case 0: y = delta; break;
                    case 1: x = delta; break;
                    case 2: y = -delta; break;
                    case 3: x = -delta; break;
                }
                Vector2 direction = new Vector2(x, y);
                if (tp + direction == posBlock) //Если положение игрока совпадает с положением передвигаемого блока
                {
                    syncBlocks[i] = block; //Добавляем ссылку на выбранный рядом стоящий блок в переменную syncBlock
                    ++blocksSum;
                }                
            }
            if (blocksSum == 4)
                break;
        }
        if (blocksSum > 1)
            choosingSyncBlock = true;
        else
            for (byte i = 0; i < 4; i++)
                if (syncBlocks[i] != null)
                {
                    CreateSynchronization(syncBlocks[i], i);
                }
    }

    void ChooseSyncBlockOfFour()
    {
        if (choosingSyncBlock == true)
        {
            for (byte i = 0; i < 4; i++)
                if (syncBlocks[i] != null)
                {
                    bmSync = syncBlocks[i].GetComponent<BlockMove>();
                    bmSync.enableLabel = 2;
                }
            targetAccess = false;
            if (Input.GetAxisRaw("Horizontal") > 0) //Движение вправо
            {
                choosedDir = 1;
                goto CreateLabel;
            }
            if (Input.GetAxisRaw("Horizontal") < 0) //Движение влево
            {
                choosedDir = 3;
                goto CreateLabel;
            }
            if (Input.GetAxisRaw("Vertical") > 0)   //Движение вверх
            {
                choosedDir = 0;
                goto CreateLabel;
            }
            if (Input.GetAxisRaw("Vertical") < 0)   //Движение вниз
            {
                choosedDir = 2;
                goto CreateLabel;
            }
            CreateLabel:
            if (choosedDir < 4)
                if (syncBlocks[choosedDir] != null)
                {                    
                    CreateSynchronization(syncBlocks[choosedDir], choosedDir);
                    SwitchOffBlockChoose();
                }
                else
                {
                    targetAccess = true;
                    CheckTarget(choosedDir);
                    SwitchOffBlockChoose();
                }
        }
    }

    void SwitchOffBlockChoose()
    {
        choosingSyncBlock = false;
        for (byte i = 0; i < 4; i++)
            if (syncBlocks[i] != null && i != choosedDir)
            {
                BlockMove bmSyncBlocks = syncBlocks[i].GetComponent<BlockMove>();
                bmSyncBlocks.enableLabel = 0;
            }
        targetAccess = true;
    }

    void DisableSyncBlock()
    {
        bmSync.enableLabel = 0;
        syncBlock = null;
    }

    void BlockSyncronisation()
    {
        //Если нажата кнопка E, включается или отключается синхронизация движения с рядом стоящим блоком
        if (Input.GetButtonDown("BlockSyncronisation"))
        {
            if (syncBlock != null || choosingSyncBlock == true)
            {
                DisableSyncBlock();
                SwitchOffBlockChoose();
            }
            else
                SynchronizationEstablishment();
        }
    }

    void CreateSynchronization(GameObject go, byte direction)
    {
        bmSync = go.GetComponent<BlockMove>();
        bmSync.enableLabel = 1;
        syncBlock = go;
        syncDir = direction;
    }

    void CheckTarget(byte dir)
    {
        /*
        foreach (GameObject block in mgr.RelocatableBlocks)
        {
            BlockMove bms = block.GetComponent<BlockMove>();
            if (bm.target.x != bms.target.x && bm.target.y != bms.target.x)
                bm.target = mgr.Target(gameObject, dir);
        }*/
        Vector2 lastTargetPos = bm.target;
        /*
        bool fromDirectFlowMoving = false;
        foreach (GameObject block in mgr.DirectFlow)
        {
            Vector2 posB = block.transform.position;
            if (previousTarget == posB)
                fromDirectFlowMoving = true;
            if (mgr.TargetValue(gameObject, dir) == posB && fromDirectFlowMoving == true)
            {
                Debug.Log(1);
                targetAccess = false;
                break;
            }
        }*/
        targetAccess = false;
        foreach (GameObject floor in mgr.Floor)
        {
            Vector2 floorPos = floor.transform.position;
            Vector2 tp = transform.position;
            if (tp + mgr.Direction(dir) == floorPos)
            {
                targetAccess = true;
                break;
            }
        }
        if (targetAccess == true)
            bm.target = mgr.Target(gameObject, dir);
        //if (lastTargetPos != target)
        //    previousTarget = lastTargetPos;
        targetAccess = true;
    }

    void MotionControl() //Функция, реагирующая на вводимые значения клавиш перемещения, и, перемещающая его в соответствующем направлении
    {

        //Если координаты объекта равны координатам своей цели, то проверяется входная ось и в зависимости от нее задается новая цель, которая будет проверена
        if (Input.GetAxisRaw("Horizontal") > 0 && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true) //Движение вправо
        {
            dir = 1;
            CheckTarget(dir);
        }
        if (Input.GetAxisRaw("Horizontal") < 0 && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true) //Движение влево
        {
            dir = 3;
            CheckTarget(dir);
        }
        if (Input.GetAxisRaw("Vertical") > 0 && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true)   //Движение вверх
        {
            dir = 0;
            CheckTarget(dir);
        }
        if (Input.GetAxisRaw("Vertical") < 0 && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true)   //Движение вниз
        {
            dir = 2;
            CheckTarget(dir);
        }
    }

    void SyncBlockControl() //Функция, управляющая синхронизируемым блоком
    {
        if (syncBlock != null)
        {
            Vector2 tp = transform.position;
            Vector2 posB = syncBlock.transform.position;            
            foreach(GameObject pit in mgr.Pits)
                if (posB == (Vector2)pit.transform.position && pit.GetComponent<Pit>().filled==false)
                    DisableSyncBlock();
            float x = 0, y = 0;
            switch (syncDir)
            {
                case 0:
                    y = delta;
                    if (posB.y - tp.y <= 1.1 * y && posB.y - tp.y > 0 && tp.x == posB.x) { }
                    else
                    {
                        DisableSyncBlock();
                    }
                    break;
                case 1:
                    x = delta;
                    if (posB.x - tp.x <= 1.1 * x && posB.x - tp.x > 0 && tp.y == posB.y) { }
                    else
                    {
                        DisableSyncBlock();
                    }
                    break;
                case 2:
                    y = -delta;
                    if (posB.y - tp.y < 0 && posB.y - tp.y >= 1.1 * y && tp.x == posB.x) { }
                    else
                    {
                        DisableSyncBlock();
                    }
                    break;
                case 3:
                    x = -delta;
                    if (posB.x - tp.x < 0 && posB.x - tp.x >= 1.1 * x && tp.y == posB.y) { }
                    else
                    {
                        DisableSyncBlock();
                    }
                    break;
            }
            if (posB == bmSync.target && lastTarget != bm.target && syncBlock != null)
                bmSync.target = mgr.Target(syncBlock, dir);
        }
    }

    void CreateButtonControl()
    {
        buttons = GameObject.FindGameObjectsWithTag("Button");
        for (int i = 0; i < 4; i++)
        {
            foreach (GameObject button in buttons)
            {
                Button bt = button.GetComponent<Button>();
                if (bt.dir == (byte)i)
                {
                    bts[i] = button.GetComponent<Button>();
                }
            }
        }
    }

    void ButtonMotionControl() //Функция, реагирующая на вводимые значения клавиш перемещения, и, перемещающая его в соответствующем направлении
    {

        //Если координаты объекта равны координатам своей цели, то проверяется входная ось и в зависимости от нее задается новая цель, которая будет проверена
        if (bts[1].pressed == true && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true) //Движение вправо
        {
            dir = 1;
            CheckTarget(dir);
        }
        if (bts[3].pressed == true && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true) //Движение влево
        {
            dir = 3;
            CheckTarget(dir);
        }
        if (bts[0].pressed == true && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true)   //Движение вверх
        {
            dir = 0;
            CheckTarget(dir);
        }
        if (bts[2].pressed == true && (Vector2)transform.position == bm.target && directFlowMoving == false && targetAccess == true)   //Движение вниз
        {
            dir = 2;
            CheckTarget(dir);
        }
    }

    /*Update()
     * if (directFlowMoving == true)
            foreach (GameObject flow in mgr.DirectFlow)
                if ((Vector2)transform.position == bm.target && bm.target != (Vector2)flow.transform.position)
                    directFlowMoving = false;
     */
}

