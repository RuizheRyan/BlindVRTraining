﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EncouragementGuide : NetworkBehaviour
{
    public GameObject guideManager;
    private float span = 6.0f;

    // Start is called before the first frame update
    void Start()
    {
        guideManager = GameObject.Find("GuideManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }
        if (guideManager.GetComponent<GuideManager>().span >= span && TurnHeadParallel.state == 3) 
        {
            if (GetComponent<player>().getSpeed() != 0)
            {
                float interval = Random.value;
                if (interval < 0.3f)
                {
                    guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Encouragement_1);
                }
                else if (interval < 0.6f)
                {
                    guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Encouragement_2);
                }
                else
                {
                    guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Encouragement_3);
                }
            }
            else
            {
                guideManager.GetComponent<GuideManager>().playList.Add((int)GuideManager.GuideDic._Encouragement_Hint);
            }

            guideManager.GetComponent<GuideManager>().span = 0.0f;
        }
    }
}
