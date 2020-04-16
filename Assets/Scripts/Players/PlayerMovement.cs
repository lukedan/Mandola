using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView PV;
    private CharacterController myCC;

    public float xSpeed = 30f;
    public float zSpeed = 30f;

    public float rotationSpeed;

    int playerID;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            ProcessTranslation();
        }

    }

    private void ProcessTranslation()
    {
        float horizontal = Input.GetAxis("Horizontal"); //A D 左右
        float vertical = Input.GetAxis("Vertical"); //W S 上 下

        transform.Translate(Vector3.forward * vertical * zSpeed * Time.deltaTime);//W S 上 下
        transform.Translate(Vector3.right * horizontal * xSpeed * Time.deltaTime);//A D 左右
        //myCC.Move(Vector3.forward * vertical * zSpeed * Time.deltaTime);
        //myCC.Move(Vector3.right * horizontal * xSpeed * Time.deltaTime);
    }
}
