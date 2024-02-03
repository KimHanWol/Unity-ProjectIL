using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAction : MonoBehaviour
{
    public float Speed;
    public GameManager manager;

    Rigidbody2D rigid;
    BoxCollider2D boxCollider2D;
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
        boxCollider2D = GetComponent<BoxCollider2D>();
        dirVec = Vector3.down;
    }

    void Update()
    {
        if(manager.CanAction == false)
        {
            return;
        }

        h = manager.isTalking ? 0 : Input.GetAxisRaw("Horizontal");
        v = manager.isTalking ? 0 : Input.GetAxisRaw("Vertical");

        bool hDown = manager.isTalking ? false : Input.GetButtonDown("Horizontal");
        bool vDown = manager.isTalking ? false : Input.GetButtonDown("Vertical");
        bool hUp = manager.isTalking ? false : Input.GetButtonUp("Horizontal");
        bool vUp = manager.isTalking ? false : Input.GetButtonUp("Vertical");

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

        //For Flip Character Sprite
        if (h > 0.01f)
        {
            //On Right Side
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if(h < -0.01f)
        {
            //On Left Side
            GetComponent<SpriteRenderer>().flipX = false;
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

        Vector2 CharacterCenter = rigid.position + boxCollider2D.offset;

        Debug.DrawRay(CharacterCenter, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D[] rayHitArray = Physics2D.RaycastAll(CharacterCenter, dirVec, 0.7f, LayerMask.GetMask("Object"));

        foreach(RaycastHit2D rayHit in rayHitArray)
        {
            if(rayHit.collider != null)
            {
                GameObject rayHitObject = rayHit.collider.gameObject;
                if (rayHitObject != null)
                {
                    if(rayHitObject.GetComponent<LFGameObject>() != null)
                    {
                        ScanObject = rayHit.collider.gameObject;
                        manager.scanObject = ScanObject;

                        CheckAutoInteractionForScanObject();
                        return;
                    }
                }
            }
        }

        if(rayHitArray.Length <= 0)
        {
            ScanObject = null;
            manager.scanObject = ScanObject;
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
            //bool isTalkSucceed = manager.Talk(autoDialogObject.DialogKey, autoDialogObject.IsNPC);
            bool isTalkSucceed = manager.Action();
            if (isTalkSucceed == true)
            {
                autoDialogObject.IsFirstInteraction = false;
            }
        }
    }
}
