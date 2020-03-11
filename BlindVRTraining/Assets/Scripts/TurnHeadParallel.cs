﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHeadParallel : MonoBehaviour
{
    player _player;
    GameObject _startPosition;
    IEnumerator coroutine;
    bool isTrackingCar;
    bool isplayed = false;
    int winCondition1, winCondition2, yesCount, noCount;
    GameObject guideManager;
    GameManager gameManager;
    public AudioClip[] audios;
    AudioSource audioSource;
    static public bool isCarInTrackZone;
    static public bool isCarComing;
    static public Vector3 targetPosition;
    static public int state;
    // Start is call`ed before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        guideManager = GameObject.Find("GuideManager");
        _player = GetComponent<player>();
        _startPosition = GameObject.FindGameObjectWithTag("StartPosition");
        isCarInTrackZone = false;
        winCondition1 = winCondition2 = 0;
        yesCount = noCount = 0;
        state = 0;
        StartCoroutine(checkTracking());
    }

    private void FixedUpdate()
    {
        if (GameManager.isStart == false)
        {
            switch (state)
            {
                case 0:
                    //turn head to track the car sound
                    if (!audioSource.isPlaying && !isplayed)
                    {
                        print("hi");
                        guideManager.GetComponent<GuideManager>().playOnce(21);
                        isplayed = true;
                    }
                    //else isplayed = false;
                    turnHead2TrackSound();
                    break;
                case 1:
                    //turn left side parallel to the traffic
                    //if(!isplayed)
                    guideManager.GetComponent<GuideManager>().playOnce(26);
                    comfirmPosition();
                    break;
            }
        }
    }
    public bool isLeftSideParallel()
    {
        Vector2 v1 = new Vector2(_player.getUnitFacingDirection().x, _player.getUnitFacingDirection().z);
        Vector2 v2 = new Vector2(0f, 1f);
        return getAngle(v1, v2) < 15f;
    }

    public float getAngle(Vector2 v1, Vector2 v2)
    {
        //angle = x1x2+y1y2 / sqr(x1*x1 + y1*y1) * sqr(x2*x2 + y2*y2)
        //forward (0,1)
        v1.Normalize();
        v2.Normalize();
        float cosAngle = v1.x * v2.x + v1.y * v2.y / (Mathf.Sqrt(v1.x * v1.x + v1.y * v1.y) * Mathf.Sqrt(v2.x * v2.x + v2.y * v2.y));
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        return angle;
    }

    public void comfirmPosition()
    {
        if (winCondition2 < 4)
        {
            if (Input.GetButtonDown("Confirm"))
            {
                //check if the player is in the right position
                if (isLeftSideParallel())
                {
                    //TODO: ADD SOUND
                    guideManager.GetComponent<GuideManager>().playOnce(Random.Range(17, 19));
                    //print("you got it!");
                    this.transform.rotation = Quaternion.Euler(this.transform.rotation.y, Random.Range(-180f, 180f), this.transform.rotation.y);
                    winCondition2++;
                }
                else
                {
                    //TODO: ADD SOUND
                    guideManager.GetComponent<GuideManager>().playOnce(30);
                    this.transform.rotation = Quaternion.Euler(this.transform.rotation.y, Random.Range(-180f, 180f), this.transform.rotation.y);
                    //print("try again");
                }
                //print("confirm");
            }
        }
        else
        {
            //print("Pass!");
            //todo: play sound
            guideManager.GetComponent<GuideManager>().playOnce(27);
            state = 3;
        }
    }

    public void turnHead2TrackSound()
    {
        if (winCondition1 < 4)
        {
            if (isCarComing)
            {
                //print("there is a car coming");
                if(targetPosition.x < 0){ //coming from left
                    //todo: play sound
                    print("there is a car coming left");
                    guideManager.GetComponent<GuideManager>().playOnce(22);
                }
                else if(targetPosition.x > 0){ //coming from right
                print("there is a car coming right");
                    guideManager.GetComponent<GuideManager>().playOnce(23);
                }
                //if the car enter the tracking zone, check if the player is looking at the car
            }
            //print("winCondition1: " + winCondition1);     
        }
        else
        {
            winCondition1 = 0;
            print(winCondition1);
            print("Pass");
            //todo: play sound
            guideManager.GetComponent<GuideManager>().playOnce(25);
            GameManager.isTrackState = false;
            state = 1;
        }

    }

    private IEnumerator checkTracking()
    {
        yield return new WaitForSeconds(0.5f);
        if (isCarInTrackZone)
        {
            Vector2 v1, v2;
            v1 = new Vector2(targetPosition.x - transform.position.x, targetPosition.z - transform.position.z);
            v2 = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.y);
            if (getAngle(v1, v2) < 20)
            {
                yesCount++;
            }
            else
            {
                noCount++;
            }
        }
        else if (!isCarInTrackZone)
        { // the target car is out of the track zone
            //compare these count
            if (yesCount > noCount)
            {
                winCondition1++;
                guideManager.GetComponent<GuideManager>().playOnce(Random.Range(17, 19));
                yesCount = noCount = 0;
            }
            else if (yesCount != 0 || noCount != 0)
            {
                guideManager.GetComponent<GuideManager>().playOnce(29);
                yesCount = noCount = 0;
            }
        }
        StartCoroutine(checkTracking());
    }

    public bool isLookingAtCar()
    {
        //get the position of the car, and the position of the player
        //compare with the player facing direction
        // < 20 degrees, return true else false
        print(targetPosition);
        Vector2 v1, v2;
        v1 = new Vector2(targetPosition.x - transform.position.x, targetPosition.z - transform.position.z);
        v2 = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.y);
        print(isLookingAtCar());
        return getAngle(v1, v2) < 20;
    }
}
