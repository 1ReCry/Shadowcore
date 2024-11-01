using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Doors : MonoBehaviour
{
    Animator animator;
    [Header("Закрыта ли дверь при старте?")]
    public bool closed;
    private bool dontCloseMore;
    
    [Header("Закрывать при касании?")]
    public bool CloseWhenCollide;

    private bool requestSended;
    private float timerRequestDelay;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(closed)
        {
            animator.SetBool("IsClosed", true);
        }
        if(!closed)
        {
            animator.SetBool("IsClosed", false);
        }
    }

    void Update()
    {
        if(timerRequestDelay > 0) timerRequestDelay -= Time.deltaTime;
        if(timerRequestDelay <= 0 & !requestSended)
        {
            requestSended = true;
            Globals.openDoorsRequest = false;
        }
        if(Globals.openDoorsRequest)
        {
            if(animator.GetBool("IsClosed"))
            {
                requestSended = false;
                timerRequestDelay = 0.2f;
                animator.SetBool("IsClosed", false);
                dontCloseMore = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Contains("Player"))
        // если двери не закрыты то закрыть и обновить нав меш
        if(!animator.GetBool("IsClosed") && CloseWhenCollide && !dontCloseMore)
        {
            animator.SetBool("IsClosed", true);
        }
    }
}
