using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class HecateAnimatorController : MonoBehaviour
{
    Rigidbody2D rBody => GetComponent<Rigidbody2D>();
    public Animator animatorH;
    public SpriteRenderer spriter;
    

    void PlayAnimations()
    {
        if (rBody.velocity.x >= 0.2f)
        {
            spriter.flipX = false;
            animatorH.SetFloat("speed", 1);
            
        }
        else { animatorH.SetFloat("speed", 0); }

        if (rBody.velocity.y >= 0.2f /* >>&&<< >>referencia do novo Input system pro pulo<<s*/)
        {
            animatorH.SetBool("jump", true);
        }
        else { animatorH.SetBool("jump", false); }
        if (rBody.velocity.x <=-0.2)
        {
            spriter.flipX = true;
            animatorH.SetFloat("speed", 1);
        }
    }

    private void Update()
    {
        PlayAnimations();
    }
}
