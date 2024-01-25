using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHead : MonoBehaviour {
    public GameObject head;
    public GameObject tail;
    public GameObject bodyStraightPrefab;
    public GameObject body90Prefab;
    public GameObject foodPrefab;
    public GameObject lastSpawnedBody;
    public Vector3 rotAxis;
    public Vector3 tailRotAxis;
    public Vector3 tailRotDir;
    public Vector3 rotDir;
    public float corgiSpeed = 1;
    public float rotSpeed = 90f; // Phase this out and just use corgiSpeed*90f in its place.
    public Vector3 corgiMoveDirection;
    public Vector3 nextMoveDirection;
    public Vector3 lastNode;
    public Vector3 nextNode;
    public bool controlsDisabled=false;
    public bool isTurning;
    public bool isTailTurning;
    public bool tailFollowing;
    public Vector3[] corgiPath;
    public int maxCorgiLength = 5;
    public int corgiLengthArrayCounter=1;
    public Vector3 tailMoveDirection;


	// Use this for initialization
	void Start () {
        corgiMoveDirection = Vector3.up;
        nextMoveDirection = Vector3.up;
        tailMoveDirection = Vector3.up;
        lastNode = head.transform.position;
        nextNode = lastNode + corgiMoveDirection;
        corgiPath = new Vector3[100];
        corgiPath[0] = head.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        //if (!controlsDisabled)
        //{
            if (Input.GetKeyDown(KeyCode.LeftArrow)&&corgiMoveDirection!=Vector3.right)
            {
                nextMoveDirection = Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)&&corgiMoveDirection != Vector3.left)
            {
                nextMoveDirection = Vector3.right;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && corgiMoveDirection != Vector3.down)
            {
                nextMoveDirection = Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && corgiMoveDirection!= Vector3.up)
            {
                nextMoveDirection = Vector3.down;
            }
        //}

        float distOfMove = (corgiMoveDirection * corgiSpeed * Time.deltaTime).magnitude;
        float distToNextNode = Vector3.Distance(nextNode, transform.position);
        
        if (distOfMove > distToNextNode)
        {
            //Debug.Log(tail.transform.position + "TailPos");
        }

        if (distOfMove > distToNextNode) //Crossing into new square FIX THIS!1
        {
            transform.position = nextNode;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Round(transform.rotation.eulerAngles.z / 90) * 90);
                      
            //Debug.Log("Crossed Line");
            isTurning = false;
            isTailTurning = false;
            lastNode = nextNode;
            AddCorgiNode();
            nextNode = lastNode + nextMoveDirection;

            //Set Turn parameters
            if (nextMoveDirection != corgiMoveDirection)
            {

                //Spawn Corgi 90, then rotate
                lastSpawnedBody = Instantiate(body90Prefab, lastNode+Vector3.forward*.2f, Quaternion.identity);
                if ((corgiMoveDirection == Vector3.up && nextMoveDirection == Vector3.left)
                          || (corgiMoveDirection == Vector3.right && nextMoveDirection == Vector3.down))
                {
                    //Debug.Log("Rotate 0");
                    lastSpawnedBody.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if ((corgiMoveDirection == Vector3.up && nextMoveDirection == Vector3.right)
                          || (corgiMoveDirection == Vector3.left && nextMoveDirection == Vector3.down))
                {
                    //Debug.Log("Rotate 90");
                    lastSpawnedBody.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if ((corgiMoveDirection == Vector3.down && nextMoveDirection == Vector3.right)
                          || (corgiMoveDirection == Vector3.left && nextMoveDirection == Vector3.up))
                {
                    //Debug.Log("Rotate 180");
                    lastSpawnedBody.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if ((corgiMoveDirection == Vector3.down && nextMoveDirection == Vector3.left)
                          || (corgiMoveDirection == Vector3.right && nextMoveDirection == Vector3.up))
                {
                   // Debug.Log("Rotate 270");
                    lastSpawnedBody.transform.rotation = Quaternion.Euler(0, 0, 270);
                }

                rotAxis = transform.position - .5f * corgiMoveDirection + .5f * nextMoveDirection;
                rotDir = Vector3.Cross(corgiMoveDirection, nextMoveDirection);
                corgiMoveDirection = nextMoveDirection;
                isTurning = true;
            }
            else
            {
                //Spawn Corgi Straight, then rotate
                lastSpawnedBody = Instantiate(bodyStraightPrefab, lastNode+Vector3.forward*.2f, Quaternion.identity);
                //thisCorg.transform.position += Vector3.forward * .1f;
                if (corgiMoveDirection == Vector3.left || corgiMoveDirection == Vector3.right)
                {
                    lastSpawnedBody.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }

            //Set Tail Turn parameters
            Vector3 tailNextMoveDirection;
            if (corgiPath[1] == Vector3.zero)
            {
                tailNextMoveDirection = tailMoveDirection;
            }
            else
            {
                tailNextMoveDirection = corgiPath[1] - corgiPath[0];
            }
            if (tailMoveDirection != tailNextMoveDirection)
            {
                tailRotAxis = tail.transform.position + .5f * tailMoveDirection + .5f * tailNextMoveDirection;
                tailRotDir = Vector3.Cross(tailMoveDirection, tailNextMoveDirection);
                tailMoveDirection = tailNextMoveDirection;
                isTailTurning = true;
            }
        }

        if (isTurning)
        {
            head.transform.RotateAround(rotAxis, rotDir, corgiSpeed*90f * Time.deltaTime);
        }
        else
        {
            transform.position = transform.position + corgiMoveDirection * corgiSpeed * Time.deltaTime;
        }


        if (corgiLengthArrayCounter == maxCorgiLength || isTailTurning)
        {
            if (isTailTurning)
            {
                tail.transform.RotateAround(tailRotAxis, tailRotDir, corgiSpeed*90f * Time.deltaTime);
            }
            else
            {
                tail.transform.position = Vector3.MoveTowards(tail.transform.position, corgiPath[0], corgiSpeed * Time.deltaTime);
            }
        }

    }

    void AddCorgiNode()
    {
        corgiPath[corgiLengthArrayCounter] = lastNode;
        corgiLengthArrayCounter++;
        if (corgiLengthArrayCounter > maxCorgiLength)
        {
            tail.transform.position = corgiPath[0];
            tail.transform.rotation = Quaternion.Euler(tail.transform.rotation.eulerAngles.x, tail.transform.rotation.eulerAngles.y, Mathf.Round(tail.transform.rotation.eulerAngles.z / 90) * 90);
            TruncPath();

            tailFollowing = true;

        }

    }

    void TruncPath()
    {
        //Debug.Log("Trunking");
        for (int i = 0; i < corgiLengthArrayCounter; i++)
        {
            corgiPath[i] = corgiPath[i + 1];
        }
        corgiLengthArrayCounter--;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("FOOD"))
        {
            Debug.Log("Eating Noms" + col.gameObject.name);
            Destroy(col.gameObject);
            maxCorgiLength++;
            corgiSpeed += .1f;
            SpawnHotdog();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("FOOD") && col.gameObject != lastSpawnedBody)
        {
            Debug.Log("Your Corgi Got too hungry and ate itself a little bit.");
            Debug.Log("Your Corgi was running his tiny legs off at "+corgiSpeed+"m/s");
            Debug.Log("YOU HIT" + col.name+ "YOU LOSE!");
            corgiSpeed = 0;
        }
    }

    void SpawnHotdog()
    {
        Vector3 randomLocation = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), .05f);
        Instantiate(foodPrefab, randomLocation, Quaternion.identity);
    }
}
