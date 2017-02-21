using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{

    //Постоянные
    public float delta = 2.42f;  //Расстояние между соседними неподвижными объектами
    [SerializeField]public float speed; //Скорость перемещения блоков
    public int sizeField = 64;

    //Генерация блоков
    public bool generation; //Переменная для подтверждения функции генерации поля
    public GameObject floor, block;
    public float k; //Размер генерируемых блоков
    public bool Equalize = true;

    //Создание целей
    public GameObject exitBlock;
    public GameObject[]  FixedBlocks, Blocks, DirectFlow, Players;
    public List<GameObject> RelocatableBlocks, Floor, AllRelocatableBlocks, Pits,Tiles;
    public GameObject[,] fixedBlocks,tiles/*, allRelocatableBlocks*/;
    public byte wrongDir = 4; //Направление, по которому не разрешается двигаться подвижным блокам

    //Остальное
    private float cameraScale, cameraScaleDefault;
    public List<Scene> levels;

    //public PlayerInteractEvent OnPlayerInteract;
    //[Serializable]
    //public class PlayerInteractEvent: UnityEvent { };

    void Awake()
    {       
        if (generation == true)
            for (float y = -delta * k; y <= delta * k; y += delta) //Заполнение поля блоками             
                for (float x = -delta * k; x <= delta * k; x += delta)
                    if (Math.Abs(y) == delta * k)
                        Instantiate(block, new Vector2(x, y), Quaternion.identity);
                    else
                        if (Math.Abs(x) == delta * k)
                        Instantiate(block, new Vector2(x, y), Quaternion.identity);
                    else
                        Instantiate(floor, new Vector2(x, y), Quaternion.identity);        
    }

    void Start()
    {
        exitBlock = GameObject.Find("Exit");
        RelocatableBlocks = new List<GameObject>(GameObject.FindGameObjectsWithTag("Relocatable"));
        Players = GameObject.FindGameObjectsWithTag("Player");
        Floor = new List<GameObject>(GameObject.FindGameObjectsWithTag("Floor"));
        Pits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pit"));
        AllRecotableBlocksFilling();
        TilesFilling();
        /*
        allRelocatableBlocks s= new GameObject[2 * size, 2 * size];
        foreach (GameObject block in AllRelocatableBlocks)
        {
            Vector2 posB = block.transform.position;
            allRelocatableBlocks[(int)(posB.x / delta) + size, (int)(posB.y / delta) + size] = block;
        }
        */
        DirectFlow = GameObject.FindGameObjectsWithTag("Direct Flow");

        Blocks = GameObject.FindGameObjectsWithTag("Block");
        fixedBlocks = new GameObject[2 * sizeField, 2 * sizeField];
        foreach (GameObject block in Blocks)
        {
            Vector2 posB = block.transform.position;
            fixedBlocks[(int)(posB.x / delta) + sizeField, (int)(posB.y / delta) + sizeField] = block;
        }
        cameraScale = cameraScaleDefault = GameObject.Find("Camera").GetComponent<Camera>().orthographicSize;
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu") == true)
            SceneManager.LoadScene("Main Menu");
        CameraScaleControl();
    }

    void CameraScaleControl()
    {
        if (Input.GetAxisRaw("CameraScale") > 0)
            if (cameraScale+1 < 30)
                cameraScale+=0.25f;
        if (Input.GetAxisRaw("CameraScale") < 0)
            if (cameraScale-1 > 5)
                cameraScale -= 0.25f;
        if (Input.GetButtonDown("CameraScaleDefault") == true)
            cameraScale = cameraScaleDefault;
        GameObject.Find("Camera").GetComponent<Camera>().orthographicSize = cameraScale;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 200, 25), "Escape - Go to Main Menu");
        GUI.Box(new Rect(0, 25, 50, 25), SceneManager.GetActiveScene().name);
    }

    public Vector2 Target(GameObject go, byte dir)
    {
        Vector2 posGO = go.transform.position;
        Vector2 target = posGO;
        float x = Direction(dir).x;
        float y = Direction(dir).y;
        //Vector2 direction = new Vector2(x, y);
        if (fixedBlocks[(int)((posGO.x + x) / delta) + sizeField, (int)((posGO.y + y) / delta) + sizeField] != null)
        {
            wrongDir = dir;
            return (target);
        }
        bool empty = true;
        foreach(GameObject tile in Tiles)
            if (posGO + Direction(dir) == (Vector2)tile.transform.position)
                empty = false;
        if (empty == true)
        {
            wrongDir = dir;
            return (target);
        }
        if (go.tag== "Relocatable"&& TargetValue(go,dir)==(Vector2)exitBlock.transform.position)
        {
            wrongDir = dir;
            return (target);
        }
        foreach (GameObject block in AllRelocatableBlocks)
        {
            Vector2 posB = block.transform.position;
            Vector2 lastB = posB;
            Vector2 targetB = posB;
            if (dir == 0 && posB.y - posGO.y <= y && posB.y - posGO.y > 0 && Math.Abs(posB.x - posGO.x) <= 0.5 * delta)
            {
                BlockMove bm = block.GetComponent<BlockMove>();
                targetB = Equalizer(Target(block, dir));
                if (dir == wrongDir)
                    targetB = lastB;
                bm.target = targetB;
                break;
            }
            if (dir == 2 && posB.y - posGO.y >= y && posB.y - posGO.y < 0 && Math.Abs(posB.x - posGO.x) <= 0.5 * delta)
            {
                BlockMove bm = block.GetComponent<BlockMove>();
                targetB = Equalizer(Target(block, dir));
                if (dir == wrongDir)
                    targetB = lastB;
                bm.target = targetB;
                break;
            }
            if (dir == 1 && posB.x - posGO.x <= x && posB.x - posGO.x > 0 && Math.Abs(posB.y - posGO.y) <= 0.5 * delta)
            {
                BlockMove bm = block.GetComponent<BlockMove>();
                targetB = Equalizer(Target(block, dir));
                if (dir == wrongDir)
                    targetB = lastB;
                bm.target = targetB;
                break;
            }
            if (dir == 3 && posB.x - posGO.x >= x && posB.x - posGO.x < 0 && Math.Abs(posB.y - posGO.y) <= 0.5 * delta)
            {
                BlockMove bm = block.GetComponent<BlockMove>();
                targetB = Equalizer(Target(block, dir));
                if (dir == wrongDir)
                    targetB = lastB;
                bm.target = targetB;
                break;
            }
        }
        Vector2 pos = new Vector2(posGO.x + x, posGO.y + y);
        Vector2 last = posGO;
        target = Equalizer(pos);
        if (dir == wrongDir)
            target = last;
        return (target);
    }

    public Vector2 TargetValue(GameObject go, byte dir)
    {
        Vector2 posGO = go.transform.position;
        return Equalizer(posGO + Direction(dir));
    }

    public Vector2 Direction(byte dir)
    {
        float x = 0, y = 0;
        switch (dir)
        {
            case 0: y = delta; break;
            case 1: x = delta; break;
            case 2: y = -delta; break;
            case 3: x = -delta; break;
        }
        return new Vector2(x, y);
    }

    public Vector3 Equalizer(Vector3 pos) //Возвращает округленные до delta * k значения координат
    {
        if (Equalize == true)
        {
            float z = pos.z;
            if (pos.x % delta != 0 || pos.y % delta != 0)
            {
                float ostX, ostY, x, y; //ost - остаток для проверки, x/y - позиция блока, деленная на delta, округленная до целыв
                x = (float)Math.Round(pos.x / delta) * delta;
                y = (float)Math.Round(pos.y / delta) * delta;
                ostX = pos.x - x; //вычисление остатка
                ostY = pos.y - y; //вычисление остатка
                if (ostX < 0.5 * delta)  //остаток по оси x меньше половины delta?            
                    if (ostY < 0.5 * delta)
                        pos = new Vector2(x, y);
                    else
                        pos = new Vector2(x, y + delta);
                else
                if (ostY < 0.5 * delta) //остаток по оси y меньше половины delta?
                    pos = new Vector2(x + delta, pos.y);
                else
                    pos = new Vector2(x + delta, y + delta);
            }
            pos.z = z;
        }
        return (pos);        
    }
    
    public float RotationEqualizer(float rotation) //Возвращает округленные до 90 градусов значение угла поворота
    {
        for (int i = 0; i <= rotation; i += 90)
            if (rotation < i)
                rotation = i - 90;
        return (rotation);
    }

    void AllRecotableBlocksFilling()
    {
        int sum = RelocatableBlocks.Count + Players.Length;
        AllRelocatableBlocks = new List<GameObject>();
        for (int i = 0; i < sum; i++)
        {
            if (i >= RelocatableBlocks.Count)
                AllRelocatableBlocks.Add(Players[i - RelocatableBlocks.Count]);
            else
                AllRelocatableBlocks.Add(RelocatableBlocks[i]);
        }
    }

    void TilesFilling()
    {
        int sum = Floor.Count + Pits.Count;
        Tiles = new List<GameObject>();
        for (int i = 0; i < sum; i++)
        {
            if (i >= Floor.Count)
                Tiles.Add(Pits[i - Floor.Count]);
            else
                Tiles.Add(Floor[i]);
        }
    }
}