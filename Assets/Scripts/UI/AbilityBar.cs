using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    public void setAbilityCooldown(float cooldown)
    {
        slider.value = cooldown;
    }
}
