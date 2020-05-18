using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public float soundDelay;
    Animator animator;
    AudioSource sound;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    public void StartEffect()
    {
        animator.SetTrigger("AtkTrigger");
        sound.PlayDelayed(soundDelay);
    }

    public void StopEffect()
    {
        animator.SetTrigger("StopTrigger");
    }
}
