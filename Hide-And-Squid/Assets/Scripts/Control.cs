//20180972 ������
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Utility;
public class Control : MonoBehaviourPun
{
    private float h = 0.0f;
    private float v = 0.0f;   // �÷��̾� ������Ʈ�� �̵��ϱ� ���� h,v 2���� ������ ���
    private float r = 0.0f;
    private float moveSpeed = 5.0f;  //�÷��̾��� ���ǵ�
    private float rotationSpeed = 200.0f;
    public float turnSpeed = 5.0f; //���콺 ȸ�� �ӵ�
    private Transform Tr;//Ʈ�������� ������ �Լ�
    private Animator animator;
    public GameObject avata;
    public Transform LaserTransform; //�������� �߻�� ��ġ
    private float LaserDistance = 2f;

    
    public AudioClip PortalSound, SoundAttack, FistAttack;
    AudioSource audiosource;
   

    public GameObject AttackParticle;
    public bool isControl;

    public PhotonView pv;
    public string nickname;
    private GameObject ChatInput;
    private NetworkManager manager;

    // Start is called before the first frame update
    private void Start()
    {
        Tr = GetComponent<Transform>();
        animator = avata.GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
        isControl = true;

        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        ChatInput = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        ChatInput.SetActive(false);
        nickname = pv.Owner.NickName;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PlaySoundAttack()
    {
        if (SoundAttack)
            if (GetComponent<AudioSource>().isPlaying) return;
            else GetComponent<AudioSource>().PlayOneShot(SoundAttack);
    }

    public void PlayFistAttack()
    {
        GetComponent<AudioSource>().PlayOneShot(FistAttack);
    }

    private void OnTriggerStay(Collider collision) // ������������ �Ҹ�����
    {
        if (collision.CompareTag("Portal") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetComponent<AudioSource>().PlayOneShot(PortalSound);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine)   //������ �ƴ϶�� ���ԺҰ�
        {
            return;
        }
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            r = Input.GetAxis("Mouse X");

            Tr.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);
            Tr.Rotate(new Vector3(0, r, 0) * rotationSpeed * Time.deltaTime);
            if (v != 0.0f)
            {
                animator.SetBool("Walk", true);
            }
            if (v == 0.0f)
            {
                animator.SetBool("Walk", false);
            }
            float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("attack");
                pv.RPC("Attack", RpcTarget.Others, null);
                PlaySoundAttack();
                Attack();
            }
            if (pv.IsMine)
             {
            if (Input.GetKeyDown(KeyCode.Return) && manager.isGameStart)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                if (ChatInput.activeSelf == false)
                    ChatInput.SetActive(true);
                else
                {
                    manager.Send();
                    ChatInput.SetActive(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        }
     
    [PunRPC]
    void Attack()
    {
        //����ĳ��Ʈ�� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit hit;
        //���� ���� ���� ������ ����
        Vector3 hitPosition = Vector3.zero;
        //����ĳ��Ʈ(���� ����, ����, �浹 ���� �����̳�, �����Ÿ�)
        if (Physics.Raycast(LaserTransform.position, LaserTransform.forward, out hit, LaserDistance))
        {

            if (hit.transform.tag == "Player")
            {
                Vector3 Attackposition = hit.transform.GetComponent<Transform>().position;
                Attackposition.y += 0.7f;
                GameObject Effect = Instantiate(AttackParticle, Attackposition, Quaternion.identity);
                Destroy(Effect, 1.0f);
                PlayFistAttack();

                GameObject player = hit.transform.gameObject;
                GameManager.instance.Dead(player);
                
                /*Destroy(hit.transform.gameObject, 1.0f);*/
            }
            else if (hit.transform.tag == "Computer")
            {
                Vector3 Attackposition = hit.transform.GetComponent<Transform>().position;
                Attackposition.y += 0.7f;
                GameObject Effect = Instantiate(AttackParticle, Attackposition, Quaternion.identity);
                Destroy(Effect, 1.0f);
                PlayFistAttack();

                GameObject computer = hit.transform.gameObject;
                GameManager.instance.Dead(computer);
                /*Destroy(hit.transform.gameObject, 1.0f);*/
            }

        }

    }

    [PunRPC]
    public void RPCDestroy() => Destroy(gameObject);

    private Vector3 currPos;
    private Quaternion currRot;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Tr.position);
            stream.SendNext(Tr.rotation);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }


}
