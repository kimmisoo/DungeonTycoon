using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEffect : MonoBehaviour
{
    Animator animator;
    AudioSource sound;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    public void OnEffect()
    {
        animator.SetBool("isOn", true);
        sound.Play();
    }

    public void OffEffect()
    {
        animator.SetBool("isOn", false);
        sound.Stop();
    }
}