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
        if (!pv.IsMine)   //������ �ƴ϶�� ���ԺҰ�
        {
            return;
        }

        /*Switch -> 0 �̵�
                 -> 1 ȸ��
                 -> 2 ����*/
        switch (state)
        {
            case 0: //�̵� -> 4�ʰ� �̵� �� , 2�� ����
                if (time > 0)
                {
                    if ( v < 0 ) //s�϶� �ڷ� �ȴ� �ӵ� ����
                    {
                        v *= 0.9f;
                    }

                    m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation); // ���ϰ���

                    this.transform.Translate(Vector3.forward * v * speed * Time.deltaTime);
                    this.transform.Rotate(Vector3.up * h * turnSpeed * Time.deltaTime);
                    computer_animator.SetBool("Walk", true); //�ִϸ��̼� ����                  
                   time -= Time.deltaTime;
                }

                else
                {
                    computer_animator.SetBool("Walk", false);
                    SetRandom();
                    state = 2;
                }
                break;

            case 1: // ȸ�� -> 2�ʰ� ȸ�� ��, �̵� state
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

            case 2: // 2�ʰ� ���� ��, 0~1 state
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
