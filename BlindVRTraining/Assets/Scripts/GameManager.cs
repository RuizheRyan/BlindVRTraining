﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public GameObject car;
    public int maxCarCount = 5;
    IntersectionController _ic;
    float timer;
    //NetworkManager networkManager;
    void Start()
    {
        _ic = IntersectionController.Instance;
//        networkManager = NetworkManager.singleton;
//#if UNITY_ANDROID
//        networkManager.StartHost();
//#elif UNITY_IOS
//        networkManager.StartHost();
//#elif Unity_Editor
//        networkManager.StartClient();
//#else
//        networkManager.StartClient();
//#endif
    }

    void Update()
    {
        if(!isServer)
        { return; }
        timer -= Time.deltaTime;
        if(timer <= 0 && CarController.carCount <= maxCarCount)
        {
            var tempCar = Instantiate(car);
            var tempCarController = tempCar.GetComponent<CarController>();
            //tempCarController.behavior = CarController.Behavior.GoStraight;
            //tempCarController.sourceDir = CarController.Direction.East;
            timer = Random.Range(2,6);
            NetworkServer.Spawn(tempCar);
        }
    }

}
