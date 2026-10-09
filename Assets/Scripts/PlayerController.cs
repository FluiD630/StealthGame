using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private UIContoller uiCon;

    public InputActionReference moveAction;
    public float moveSpeed;
    public InputActionReference walkAction;
    public float walkSpeed;
    private float activeMoveSpeed;

    public InputActionReference rollAction;
    public float rollSpeed, rollLength;
    private float rollCounter;

    public InputActionReference aimAction,lookAction;

    public InputActionReference shootAction;
    public Transform shootPoint;
    public float range;

    public InputActionReference reloadAction;

    public GameObject shootEffect, impactEffect;


    public CharacterController charCon;

    public Transform model;

    public float turnSpeed;

    private float ySpeed;

    public float gravityScale = 5f;

    private Vector2 lastMousePos;
    public GameObject weapon;

    public int currentAmmo, remainingAmmo, clipSize;

    public Animator anim;
    void Start()
    {

        uiCon = FindAnyObjectByType<UIContoller>();

        activeMoveSpeed = moveSpeed;

        uiCon.UpdateAmmoText(currentAmmo, remainingAmmo);

    }

    // Update is called once per frame
    void Update()
    {

        if (walkAction.action.IsPressed())
        {
            activeMoveSpeed = walkSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();


        if (rollAction.action.WasPressedThisFrame() && !aimAction.action.IsPressed())
        {
            if (rollCounter <= 0)
            {
                anim.SetTrigger("roll");

                rollCounter = rollLength;
            }
           
        }

        if (moveInput != Vector2.zero && !aimAction.action.IsPressed())
        {
            Quaternion lookDir = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));

            model.rotation = Quaternion.Slerp(model.rotation,lookDir,turnSpeed * Time.deltaTime);
        }

        if (aimAction.action.IsPressed())
        {

            moveInput = Vector2.zero;

            rollCounter = 0f;
            
            anim.SetBool("aiming", true);

            Vector3 aimDir = Vector3.zero;

            weapon.SetActive(true);

            if (lastMousePos != Mouse.current.position.value)
            {

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.value.x, Mouse.current.position.value.y, Camera.main.transform.position.y - model.position.y));
            
                mousePos.y = model.position.y;
                
                aimDir = mousePos - model.position;
            
            }
            
            lastMousePos = Mouse.current.position.value;

            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
            
            if (lookInput != Vector2.zero)
            {
                aimDir = new Vector3(lookInput.x, 0f, lookInput.y);
            }

            if (aimDir != Vector3.zero)
            {
                model.rotation = Quaternion.LookRotation(aimDir);
            }


            if(shootAction.action.WasPressedThisFrame())
            {

                if (currentAmmo > 0)
                {
                
                 Instantiate(shootEffect, shootEffect.transform.position, shootPoint.transform.rotation).SetActive(true);
 

                 anim.SetTrigger("shoot");

                 RaycastHit hit;

                    if(Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, range))
                    {
                     Debug.Log("Hit: " + hit.transform.name);

                     Instantiate(impactEffect, hit.point, Quaternion.identity); 
                    }

                   currentAmmo--;

                    uiCon.UpdateAmmoText(currentAmmo, remainingAmmo);
                }
                else
                {

                    //Reload();

                }
            }
        }
        else
        {
            anim.SetBool("aiming", false);

            weapon.SetActive(false);
        }

        if (reloadAction.action.WasPressedThisFrame() || currentAmmo == 0 && shootAction.action.IsPressed())
        {
            Reload();
        }


        if (charCon.isGrounded)
        {
            ySpeed = -2f;
        }
        
         ySpeed +=  Physics.gravity.y * Time.deltaTime * gravityScale;

        if(rollCounter > 0)
        {
            rollCounter -= Time.deltaTime;

            activeMoveSpeed = rollSpeed;

            Vector3 rollDirection = model.forward;

            moveInput.x = rollDirection.x;
            moveInput.y = rollDirection.z;
        }

        //charCon.Move(new Vector3(moveInput.x * moveSpeed, ySpeed, moveInput.y * moveSpeed) * Time.deltaTime);
        charCon.Move(new Vector3(moveInput.x * activeMoveSpeed, ySpeed, moveInput.y * activeMoveSpeed) * Time.deltaTime);

        anim.SetFloat("speed", moveInput.magnitude * activeMoveSpeed);


        lastMousePos = Mouse.current.position.value;
    }

    void Reload()
    {
        int refileAmount = clipSize - currentAmmo;

        if (remainingAmmo >= refileAmount)
        {
            remainingAmmo -= refileAmount;
        }

        else
        {
            refileAmount = remainingAmmo;
            remainingAmmo = 0;
        }

        currentAmmo += refileAmount;

        uiCon.UpdateAmmoText(currentAmmo, remainingAmmo);
    }
}
