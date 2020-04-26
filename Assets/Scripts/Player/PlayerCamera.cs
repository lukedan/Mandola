using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Resolution of game view
    private Vector2 screenResolution;

    //Camera offset
    private Vector3 offsetValue;
    private float offsetScaleX;
    private float offsetScaleZ;

    //Initial offset between camera and avatar
    private Vector3 currentOffset;

    public Transform playerTransform;
    
    public void setPlayer(Transform T)
    {
        playerTransform = T;
    }

    void Start()
    {
        screenResolution = new Vector2(Screen.width, Screen.height);
        offsetValue = new Vector3(13, 0, 10);
        currentOffset = new Vector3(0, 0, 0);
    }

    public void UpdateCamera(Transform playerT, Vector3 initPosOffset, Vector3 initRotation)
    {
        if (Time.timeScale == 0)
            return;


        //Clamp the mouse position within the resolution of current window
        float mousePosX = Mathf.Clamp(Input.mousePosition.x , - screenResolution.x, screenResolution.x);
        float mousePosY = Mathf.Clamp(Input.mousePosition.y, -screenResolution.y, screenResolution.y);

        offsetScaleX = ((mousePosX - screenResolution.x * 0.5f) / (screenResolution.x * 0.5f));
        offsetScaleZ = ((mousePosY - screenResolution.y * 0.5f) / (screenResolution.y * 0.5f));

        Vector3 targetOffset = new Vector3(offsetScaleX * offsetValue.x, 0, offsetScaleZ * offsetValue.z);

        Vector3 velocity = (targetOffset - currentOffset) * 0.5f;

        currentOffset += velocity * Time.deltaTime;

        transform.position = playerT.position + initPosOffset + currentOffset;
        transform.rotation = Quaternion.Euler(initRotation);
    }
}