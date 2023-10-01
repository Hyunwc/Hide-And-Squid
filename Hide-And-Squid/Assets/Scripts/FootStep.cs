using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FootStep : MonoBehaviourPunCallbacks
{
    public AudioSource source;

    public AudioClip footstep;

    public  PhotonView pv;
    public void footsound()
    {
        if (!pv.IsMine) 
        {
            return;
        }
        source.clip = footstep;
        source.Play();
        
    }
}
