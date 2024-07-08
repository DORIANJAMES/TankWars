using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private GameObject explosionPrefab;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag(targetTag))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            
            Destroy(this.gameObject.gameObject);
        }*/
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject.gameObject);
    }
}
