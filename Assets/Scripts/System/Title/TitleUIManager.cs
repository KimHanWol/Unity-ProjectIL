using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public List<GameObject> HeartObjectList;
    public Sprite BrokenHeartSprite;

    public GameObject BrokenGlass;

    void Start()
    {
        if (BrokenGlass != null)
        {
            BrokenGlass.SetActive(false);
        }
        StartCoroutine(PlayHeartMoving());
    }

    public void PlayStart()
    {
        foreach (GameObject HeartObject in HeartObjectList)
        {
            if (HeartObject != null)
            {
                Animator HeartAnimator = HeartObject.GetComponent<Animator>();
                if (HeartAnimator != null)
                {
                    HeartAnimator.SetTrigger("HeartBreak");
                    StartCoroutine(HeartMovingAnimation(HeartObject));
                }
            }
        }
    }

    IEnumerator HeartMovingAnimation(GameObject HeartObject)
    {
        if (HeartObject == null)
        {
            yield return null;
        }

        float VibrateRange = 0.1f;
        while (VibrateRange <= 20.0f)
        {
            float VibratePowerX = Random.Range(0.1f, VibrateRange);
            float VibratePowerY = Random.Range(0.1f, VibrateRange);
            Vector3 ActualPosition = HeartObject.transform.position;
            HeartObject.transform.position = ActualPosition + new Vector3(VibratePowerX, VibratePowerY, 0);
            yield return new WaitForSeconds(0.01f);
            HeartObject.transform.position = ActualPosition;
            yield return new WaitForSeconds(0.01f);
            VibrateRange += 0.3f;
        }

        foreach (GameObject InHeartObject in HeartObjectList)
        {
            if (InHeartObject != null)
            {
                Image HeartImage = InHeartObject.GetComponent<Image>();
                HeartImage.sprite = BrokenHeartSprite;
            }
        }

        if (BrokenGlass != null)
        {
            BrokenGlass.SetActive(true);
        }
    }

    IEnumerator PlayHeartMoving()
    {
        foreach (GameObject HeartObject in HeartObjectList)
        {
            if (HeartObject != null)
            {
                Animator HeartAnimator = HeartObject.GetComponent<Animator>();
                if (HeartAnimator != null)
                {
                    HeartAnimator.SetTrigger("StartMoving");
                }
            }
        }

        while (true)
        {
            foreach (GameObject HeartObject in HeartObjectList)
            {
                if (HeartObject != null)
                {
                    Animator HeartAnimator = HeartObject.GetComponent<Animator>();
                    if (HeartAnimator != null)
                    {
                        HeartAnimator.speed = Random.Range(0.7f, 0.8f);
                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
        }
    }
}
