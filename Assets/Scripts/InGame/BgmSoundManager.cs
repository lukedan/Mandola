using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmSoundManager : MonoBehaviour
{
    private static BgmSoundManager bgmSoundInstance = null;
    public static BgmSoundManager Instance
    {
        get { return bgmSoundInstance; }
    }

    void Awake()
    {
        if (bgmSoundInstance != null && bgmSoundInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            bgmSoundInstance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
