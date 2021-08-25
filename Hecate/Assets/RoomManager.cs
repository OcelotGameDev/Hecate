using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera currentCamera;
    //public Collision2D player;
    


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentCamera.Priority = 1;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentCamera.Priority = 0;
        }
    }

    private void OnValidate()
    {
        if (currentCamera == null)
        {
            currentCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }
    }
    /*void OnTriggerEnter2D(GameObject gObject)
    {
        if (player.otherCollider.CompareTag("Room"))
        {
            gObject = player.otherCollider.gameObject;
            currentCamera?.SetActive(false);
            currentCamera = gObject.GetComponentInChildren<CinemachineVirtualCamera>()?.gameObject;
            currentCamera.SetActive(true);
        }
    }*/
}
