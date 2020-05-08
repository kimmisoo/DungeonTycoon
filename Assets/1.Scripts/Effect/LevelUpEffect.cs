using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    public Animator animator;
    public AudioSource sound;
    float destroyTime = 3.0f;

    public void OnEnable()
    {
        animator.SetTrigger("LevelUPTrigger");
        sound.PlayDelayed(0.0f);

        Invoke("DestroyObject", destroyTime);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
