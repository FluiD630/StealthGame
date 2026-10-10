using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
   
    public int amountToAdd;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().GetAmmo(amountToAdd);


            Destroy(gameObject);
        }

    }


 
}
