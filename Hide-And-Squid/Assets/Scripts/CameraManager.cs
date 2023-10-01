using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviourPun
{
    float mouseSpeed = 4;
    float mouseY = 0;
    public GameObject newcamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            newcamera.SetActive(false);  // 이즈마인이 아닐경우 카메라 삭제
        }

        mouseY += Input.GetAxis("Mouse Y") * mouseSpeed;

        mouseY = Mathf.Clamp(mouseY, -55.0f, 55.0f);

        transform.localEulerAngles = new Vector3(-mouseY, 0, 0);

    }
}

