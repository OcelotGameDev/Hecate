using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class HecateSounds : MonoBehaviour
{
    [EventRef] public string SoundEvent;


    public void PlayStep()
    {
        FMODUnity.RuntimeManager.PlayOneShot(SoundEvent);
    }

}
