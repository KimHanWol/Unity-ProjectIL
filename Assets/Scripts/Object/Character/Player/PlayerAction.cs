using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAction : MonoBehaviour
{
    public float Speed;
    public GameManager manager;

    Rigidbody2D rigid;
    Animator anim;
    float h;
    float v;
    bool isHorizonMove;
    Vector2 dirVec;
    GameObject ScanObject;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dirVec = Vector3.down;
    }

    void Update()
    {
        h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal");
        v = manager.isAction ? 0 : Input.GetAxisRaw("Vertical");

        bool hDown = manager.isAction ? false : Input.GetButtonDown("Horizontal");
        bool vDown = manager.isAction ? false : Input.GetButtonDown("Vertical");
        bool hUp = manager.isAction ? false : Input.GetButtonUp("Horizontal");
        bool vUp = manager.isAction ? false : Input.GetButtonUp("Vertical");

        if (hDown || vUp)
            isHorizonMove = true;
        else if (vDown || hUp)
            isHorizonMove = false;

        if (anim)
        {
            if (anim.GetInteger("hAxisRaw") != h)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("hAxisRaw", (int)h);
            }
            else if (anim.GetInteger("vAxisRaw") != v)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("vAxisRaw", (int)v);
            }
            else
            {
                anim.SetBool("isChange", false);
            }

            anim.SetInteger("hAxisRaw", (int)h);
            anim.SetInteger("vAxisRaw", (int)v);
        }

        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && h == -1) dirVec = Vector3.left;
        else if (hDown && h == 1) dirVec = Vector3.right;

        if (Input.GetButtonDown("Jump") && ScanObject != null)
        {
            manager.Action();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            manager.OpenItemUI();
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * Speed;

        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
        {
            ScanObject = rayHit.collider.gameObject;
            manager.scanObject = ScanObject;

            CheckAutoInteractionForScanObject();
        }
        else
        {
            ScanObject = null;
        }
    }

    void CheckAutoInteractionForScanObject()
    {
        if (ScanObject == null)
        {
            return;
        }

        AutoDialogObject autoDialogObject = ScanObject.GetComponent<AutoDialogObject>();
        if(autoDialogObject != null && autoDialogObject.IsFirstInteraction == true)
        {
            bool isTalkSucceed = manager.Talk(autoDialogObject.DialogKey, autoDialogObject.IsNPC);
            if(isTalkSucceed == true)
            {
                autoDialogObject.IsFirstInteraction = false;
            }
        }
    }
}
