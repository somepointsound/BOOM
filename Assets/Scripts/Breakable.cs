using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Breakable : MonoBehaviour
{
  
    [SerializeField] GameObject IntactGlass;
    [SerializeField] GameObject BrokenGlass;

  
    BoxCollider bc;

   
    private void Awake()
    {
     
        IntactGlass.SetActive(true);
        BrokenGlass.SetActive(false);

       
        bc = GetComponent<BoxCollider>();
    }

    
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name == "boom-pole")
        {
            Break();
        }
    }

   
    private void Break()
    {
      
        IntactGlass.SetActive(false);
        BrokenGlass.SetActive(true);

      
        bc.enabled = false;
    }
}
