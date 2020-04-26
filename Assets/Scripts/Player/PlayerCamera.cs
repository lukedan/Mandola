using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Player transform
    public Transform playerTransform;

    //Resolution of game view
    private Vector2 screenResolution;

    //Camera offset
    private Vector3 offsetValue;
    private float offsetScaleX;
    private float offsetScaleZ;

    //Initial offset between camera and avatar
    private Vector3 currentOffset;

    //When avatar dies, equals false
    private bool onAvatar;

    void Start()
    {
        screenResolution = new Vector2(1024, 768);
        offsetValue = new Vector3(13, 0, 10);
        onAvatar = true;
        currentOffset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CameraShift();
    }


    //Camera shifts according to the mouse position
    private void CameraShift()
    {
        if (onAvatar)
        {
            offsetScaleX = ((Input.mousePosition.x - screenResolution.x * 0.5f) / (screenResolution.x * 0.5f));
            offsetScaleZ = ((Input.mousePosition.y - screenResolution.y * 0.5f) / (screenResolution.y * 0.5f));

            Vector3 targetOffset = new Vector3(offsetScaleX * offsetValue.x, 0, offsetScaleZ * offsetValue.z);

            Vector3 velocity = (targetOffset - currentOffset) * 0.5f;

            transform.position += velocity * Time.deltaTime;
            currentOffset += velocity * Time.deltaTime;
        }
    }

    // When avatar dies, set the camera static
    public void DetachCameraFromAvatar()
    {
        onAvatar = false;
        transform.parent = null;
        Debug.Log("Detach camera");

        //Destroy camera 3 scends after the avatar dies
        Invoke("DestroyCamera", 3);
    }

    //Destroy camera
    private void DestroyCamera()
    {
        Debug.Log("Destroy camera");
        Destroy(gameObject);
        // TO DO : transfer the view to another camera or load scene
    }
}