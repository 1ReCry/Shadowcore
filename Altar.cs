using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public string textPrefix;
    public string altarText;
    public float notifTime;
    public bool coreBlink;
    public bool coreText;
    public AudioClip interactSound;

    private Animator animator;
    private bool used;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Altar collide with " + other);
        if(other.name == "Player" && !used)
        {
            used = true;
            PlayerAI pai = other.GetComponent<PlayerAI>();
            pai.PlaySound(interactSound, 1f, 1f, 5);
            animator.SetTrigger("activate");
            if(coreBlink) Globals.activateBlink = true;
            
            UI_Elements uielem = FindObjectOfType<UI_Elements>(); 
            if(!coreText)uielem.StoryNotification("<color=#52c8ff><size=24>"+textPrefix+"</size></color>\n"+altarText, notifTime);
            if(coreText)uielem.StoryNotification("<color=#8e47ff><size=40>"+textPrefix+"</size></color>\n<color=#ddc7ff>"+altarText, notifTime);
        }
    }
}
