using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailCleanup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerExit2D(Collider2D col)
    {
        //Debug.Log("Destroying Last Body Section");
        if (col.CompareTag("Body"))
        {
            Destroy(col.gameObject);
        }
    }
}
