using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    Animator animator;
    AudioSource sound;

    public void Start()
    {
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    public void StartEffect()
    {
        animator.SetTrigger("AtkTrigger");
        sound.PlayDelayed(0.37f);
    }
}
