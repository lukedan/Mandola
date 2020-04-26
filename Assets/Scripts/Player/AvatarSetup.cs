using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;

    public int characterValue;

    //Camera prefab
    public GameObject cameraPrefab;

    //Player camera
    private GameObject playerCamera;

    //Initial position offset between avatar and camera
    public Vector3 initialPositionOffset = new Vector3(0, 23, -15);
    public Vector3 initialRotation = new Vector3(53, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
            playerCamera = Instantiate(cameraPrefab, transform.position + initialPositionOffset, Quaternion.Euler(initialRotation));
            playerCamera.GetComponent<PlayerCamera>().setPlayer(transform);
        }
        else
        {
            Destroy(playerCamera);
        }
    }

    private void Update()
    {
        if(PV.IsMine)
        {
            //Update player camera's transformation
            playerCamera.GetComponent<PlayerCamera>().UpdateCamera(transform, initialPositionOffset, initialRotation);
        }
    }


    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        characterValue = whichCharacter;
        myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter],
            transform.position, transform.rotation, transform);
    }
}
