using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TPSControl : MonoBehaviourPun
{
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    Animator animator;

    public Transform LaserTransform; //레이저가 발사될 위치
    private float LaserDistance = 2f;
    private GameObject ChatInput2;
    private NetworkManager manager;
    public PhotonView pv;
    public GameObject newcamera;
    public string nickname;
    private Transform Tr;//트랜스폼을 저장할 함수
    // Start is called before the first frame update
    void Start()
    {
        Tr = GetComponent<Transform>();
        animator = characterBody.GetComponent<Animator>();
        pv = GetComponent<PhotonView>();

        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        ChatInput2 = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        ChatInput2.SetActive(false);
        nickname = pv.Owner.NickName;


    }

    // Update is called once per frame
    void Update()
    {

        LookAround();
        Move();
        if (!pv.IsMine)   //로컬이 아니라면 진입불가
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("attack");
            pv.RPC("Attack", RpcTarget.Others, null);

            Attack();
        }
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Return) && manager.isGameStart)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                if (ChatInput2.activeSelf == false)
                {
                    ChatInput2.SetActive(true);
                }
                else
                {
                    manager.Send();
                    ChatInput2.SetActive(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

    }
    private void Move()
    {
        if (!pv.IsMine)   //로컬이 아니라면 진입불가
        {
            return;
        }
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isMove", isMove);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = lookForward;
            transform.position += moveDir * Time.deltaTime * 5f;
        }
        // Debug.DrawLine(cameraArm.position, new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized, Color.red);
    }

    private void LookAround()
    {
        if (!pv.IsMine)   //로컬이 아니라면 진입불가
        {
            return;
        }
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    [PunRPC]
    void Attack()
    {
        //레이캐스트에 의한 충돌 정보를 저장하는 컨테이너
        RaycastHit hit;
        //공격 받은 곳을 저장할 변수
        Vector3 hitPosition = Vector3.zero;
        //레이캐스트(시작 지점, 방향, 충돌 정보 컨테이너, 사정거리)
        if (Physics.Raycast(LaserTransform.position, LaserTransform.forward, out hit, LaserDistance))
        {

            if (hit.transform.tag == "Player")
            {

                Destroy(hit.transform.gameObject, 1.0f);
            }
            else if (hit.transform.tag == "Computer")
            {
                // Vector3 Attackposition = hit.transform.GetComponent<Transform>().position;
                // Attackposition.y += 0.7f;
                // GameObject Effect = Instantiate(AttackParticle, Attackposition, Quaternion.identity);
                // Destroy(Effect, 1.0f);
                Destroy(hit.transform.gameObject, 1.0f);
            }

        }

    }
    void LateUpdate()
    {
        if (!pv.IsMine)
        {
            newcamera.SetActive(false);  // 이즈마인이 아닐경우 카메라 삭제
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
