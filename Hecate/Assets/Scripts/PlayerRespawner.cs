using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = _spawnPoint.transform.position;
        }
    }
}
