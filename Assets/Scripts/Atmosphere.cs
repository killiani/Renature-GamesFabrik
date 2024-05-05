using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ZoneEffects : MonoBehaviour
{
    public PostProcessVolume defaultVolume;
    public PostProcessVolume zone1Volume;
    public PostProcessVolume zone2Volume;
    public PostProcessVolume zone3Volume;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("atmo-gray"))
        {
            //defaultVolume.weight = 0;
            //zone1Volume.weight = 1;
            print("is in gray");
        }
        else if (other.CompareTag("Zone2"))
        {
            //defaultVolume.weight = 0;
            //zone2Volume.weight = 1;
        }
        else if (other.CompareTag("Zone3"))
        {
            //defaultVolume.weight = 0;
            //zone3Volume.weight = 1;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Zone1") || other.CompareTag("Zone2") || other.CompareTag("Zone3"))
    //    {
    //        defaultVolume.weight = 1;
    //        zone1Volume.weight = 0;
    //        zone2Volume.weight = 0;
    //        zone3Volume.weight = 0;
    //    }
    //}
}

