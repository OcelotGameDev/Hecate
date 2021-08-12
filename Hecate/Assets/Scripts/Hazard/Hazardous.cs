using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hazards {type1,type2}
public class Hazardous : MonoBehaviour
{
    public Hazards interacts;
    public GameObject door = null;
    bool trap, wisg;

    void SwitchInteractables()
    {
        switch (interacts)
        {
            case Hazards.type1:
            {
                    trap = true;
            }
            break;

            case Hazards.type2:
            {
                    wisg = true;
            }
            break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (trap)
        {
            if (collision.CompareTag("Player"))
            {
                //acessa o player e diminui a sua vida. (IHittable?)
            }
        }

        if (wisg)
        {
            if (collision.CompareTag("Player"))
            {
                //acessa o player e diminui sua velocidade
            }
        }

        if (collision.CompareTag("Head"))
        {
            //abre a porta
        }

    }
}
