using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float moveSpeed;

    
    void Start()
    {

        Transform player = FindAnyObjectByType<PlayerController>().transform;

        transform.LookAt(player.position + Vector3.up);

        Destroy(gameObject,5f);
    }

    
    void Update()
    {

        transform.position += transform.forward * (moveSpeed * Time.deltaTime);

       

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player got hit by bullet");
        }

        Destroy(gameObject);
    }
}
