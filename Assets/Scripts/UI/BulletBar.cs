using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletBar : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    public void setBullet(float clipBullet)
    {
        slider.value = clipBullet;
    }
}
