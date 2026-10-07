using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player;

    private Vector3 offcet;

    public float moveSpeed;
    void Start()
    {
        offcet = transform.position - player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position =Vector3.Lerp(transform.position,player.position + offcet,moveSpeed * Time.deltaTime);
    }
}
