using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource cameraShot;
    public AudioSource cameraCam1;
    public AudioSource cameraCam2;
    

    public void CameraShot()
    {
        cameraShot.Play();
    }
    public void CameraCam1()
    {
        cameraCam1.Play();
    }
    public void CameraCam2()
    {
        cameraCam2.Play();
    }
}
