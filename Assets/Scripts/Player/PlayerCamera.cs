using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    //Avatar's transform 
    private Transform avatarTransform;

    //Resolution of game view
    private Vector2 screenResolution;

    //Camera offset
    private Vector3 offsetValue;
    private float offsetScaleX;
    private float offsetScaleZ;

    //Initial offset between camera and avatar
    private Vector3 initialOffset;

    void Start()
    {
        avatarTransform = transform.parent.transform;
        initialOffset = transform.position - avatarTransform.position;
        screenResolution = new Vector2(1024, 768);
        offsetValue = new Vector3(13, 0, 10);

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        offsetScaleX = ((Input.mousePosition.x - screenResolution.x * 0.5f) / (screenResolution.x * 0.5f));
        offsetScaleZ = ((Input.mousePosition.y - screenResolution.y * 0.5f) / (screenResolution.y * 0.5f));


        transform.position = new Vector3(avatarTransform.position.x + offsetScaleX * offsetValue.x,
                                         avatarTransform.position.y,
                                         avatarTransform.position.z + offsetScaleZ * offsetValue.z) + initialOffset;
    }
}
