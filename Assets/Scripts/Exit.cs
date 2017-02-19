using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour {

    [SerializeField]private int level;
    private string levelName;
    Vector3 zero = new Vector2(0, 0);
    bool reductionObject = false;
    GameObject Object;

    void Start () {
		
	}
	
	// Update is calledy once per frame
	void Update () {
        ReductionObject();
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            reductionObject = true;
            Object = col.gameObject;
            Vector2 tp = transform.position;
            Vector2 blockPos = col.gameObject.transform.position;                        
            if (tp == blockPos && col.gameObject.transform.localScale == zero)
            {
                Object = null;
                reductionObject = false;
                levelName = SceneManager.GetActiveScene().name;
                level = int.Parse(levelName.Substring(6));
                levelName = levelName.Replace(level.ToString(), (level + 1).ToString());
                SceneManager.LoadScene(levelName);
            }
        }
    }

    void ReductionObject()
    {
        if (reductionObject == true)
        {
            zero.z = Object.transform.localScale.z;
            Object.transform.localScale = Vector3.MoveTowards(Object.transform.localScale, zero, 5 * Time.deltaTime);
        }
    }
}
