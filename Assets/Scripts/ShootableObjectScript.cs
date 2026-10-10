using UnityEngine;

public class ShootableObjectScript : MonoBehaviour
{
    public GameObject destroyEffect;

    public GameObject ammoToDrop;
    [Range(0f, 100f)]
    public float ammChance;
    public void DestroyObject()
    {
        Instantiate(destroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);


        if (ammoToDrop != null)
        {
            if(Random.Range(0f,100f) < ammChance)
            {
                Instantiate(ammoToDrop, transform.position, ammoToDrop.transform.rotation);
            }
        }
    }

}
