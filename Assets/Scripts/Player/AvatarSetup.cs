using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;

    public int characterValue;

    public Camera myCamera;
    public AudioListener myAL;

    public GameObject cameraPrefab;

    public Vector3 initialPositionOffset = new Vector3(0, 23, -15);
    public Vector3 initialRotation = new Vector3(53, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
            Instantiate(cameraPrefab, transform.position + initialPositionOffset, Quaternion.Euler(initialRotation));
            cameraPrefab.GetComponent<PlayerCamera>().playerTransform = transform;
        }
        else
        {
            Destroy(myCamera);
            Destroy(myAL);
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
