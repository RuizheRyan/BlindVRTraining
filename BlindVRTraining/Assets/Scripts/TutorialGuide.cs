﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGuide : MonoBehaviour
{
    public enum State { Guide_Direction, Push_To_Walk, Wait, Win };

    public GameObject guideManager;

    public Vector3 direction;
    public GameObject Singnal;
    private SignalController sc;

    public State state = State.Wait;
    private float span = 5.0f;
    private float duration = 0.0f;
    private bool istriggered = false;
    private bool hint = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!guideManager.GetComponent<AudioSource>().isPlaying)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            duration = span;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !istriggered)
        {
            other.gameObject.GetComponent<player>().stop();

            if (other.gameObject.GetComponent<player>().isCollected)
            {
                state = State.Win;
                guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Tutorial_Congratuate);
            }
            else
            {
                state = State.Guide_Direction;
                sc = Singnal.GetComponent<SignalController>();

                //determine which direction to turn
                guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._XStreet_Direction);
                Vector3 playerFaceDir = other.gameObject.GetComponent<player>().getUnitFacingDirection();
                float dot = Vector3.Dot(direction, playerFaceDir);

                if (dot >= 0.7f)
                {
                    guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Direction_Front);
                }
                else if (dot <= -0.7f)
                {
                    guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Direction_Back);
                }
                else
                {
                    Vector3 cross = Vector3.Cross(playerFaceDir, direction);
                    if (cross.y > 0)
                    {
                        guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Direction_Right);
                    }
                    else if (cross.y < 0)
                    {
                        guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Direction_Left);
                    }
                }

            }

            istriggered = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (state)
            {
                case State.Guide_Direction:
                    {
                        Vector3 playerFaceDir = other.gameObject.GetComponent<player>().getUnitFacingDirection();
                        float dot = Vector3.Dot(direction, playerFaceDir);
                        if (dot >= 0.7f)
                        {
                            guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Tutorial_XStreet);
                            guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Tutorial_PushButton);
                            state = State.Push_To_Walk;
                            duration = 0.0f;
                        }
                    }
                    break;
                case State.Push_To_Walk:
                    {
                        if (duration <= 0.0f)
                        {
                            guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Xstreet_PushButton);
                        }

                        if (Input.GetKey(KeyCode.Space))
                        {
                            state = State.Wait;
                            other.gameObject.GetComponent<player>().move();
                            duration = 0.0f;
                        }
                    }
                    break;
                case State.Wait:
                    {
                        if (duration <= 0.0f && !sc.AllowGoStraight)
                        {
                            guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Xstreet_Wait);
                            hint = false;
                        }
                        else if (!hint && sc.AllowGoStraight)
                        {
                            guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Tutorial_Beep);
                            hint = true;
                        }
                    }
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        istriggered = false;
        if (!sc.AllowGoStraight)
        {
            guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Error_HurtByCar);
            other.gameObject.GetComponent<player>().resetLocation(transform.position);
        }
    }
}