using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsScript : MonoBehaviour
{
    public int WeaponID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectWeapon(string swordGameobjName)
    {
        foreach (Transform child in transform)
        {
            if (child.name == swordGameobjName)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
