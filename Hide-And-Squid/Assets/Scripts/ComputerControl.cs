using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;


public class ComputerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed, turnSpeed;
    public Animator computer_animator;
    private float v, h;
    private int state;
    private float time, rotateTime, stopTime, m_currentV;
    private readonly float m_interpolation = 10;
    

    private PhotonView pv;

    void Start()
    {
        SetRandom();
        state = UnityEngine.Random.Range(0, 3);
        pv = GetComponent<PhotonView>();
       
    }

    void SetRandom()
    {
        time = UnityEngine.Random.Range(4.0f, 7.0f);
        rotateTime = UnityEngine.Random.Range(2.0f, 3.0f);
        stopTime = UnityEngine.Random.Range(1.0f, 3.0f);
        v = UnityEngine.Random.Range(-1.0f, 1.0f);
        h = UnityEngine.Random.Range(-1.0f, 1.0f);
    }

    void Update()
    {
        if (!pv.IsMine)   //로컬이 아니라면 진입불가
        {
            return;
        }

        /*Switch -> 0 이동
                 -> 1 회전
                 -> 2 정지*/
        switch (state)
        {
            case 0: //이동 -> 4초간 이동 후 , 2초 정지
                if (time > 0)
                {
                    if ( v < 0 ) //s일때 뒤로 걷는 속도 적용
                    {
                        v *= 0.9f;
                    }

                    m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); // 상하갱신

                    this.transform.Translate(Vector3.forward * v * speed * Time.deltaTime);
                    this.transform.Rotate(Vector3.up * h * turnSpeed * Time.deltaTime);
                    computer_animator.SetBool("Walk", true); //애니메이션 갱신                  
                   time -= Time.deltaTime;
                }

                else
                {
                    computer_animator.SetBool("Walk", false);
                    SetRandom();
                    state = 2;
                }
                break;

            case 1: // 회전 -> 2초간 회전 후, 이동 state
                if(rotateTime > 0)
                {
                    this.transform.Rotate(Vector3.up * h * turnSpeed * Time.deltaTime);
                    rotateTime -= Time.deltaTime;
                    /*computer_animator.SetBool("Walk", true);*/
                }
                else
                {
                    /*computer_animator.SetBool("Walk", false);*/
                    SetRandom();
                    state = 0;
                }
                break;

            case 2: // 2초간 정지 후, 0~1 state
                if (stopTime > 0)
                {
                    stopTime -= Time.deltaTime;
                    /*computer_animator.SetBool("Walk", false);*/

                }
                else
                {
                    SetRandom();
                    state = UnityEngine.Random.Range(0, 2);
                }
                break;
        }
    }

    [PunRPC]
    public void RPCDestroy() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
