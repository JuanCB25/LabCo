
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5.0f;
    public float velocidadCorrer = 7f;
    public float fuerzaSalto = 6f;
    public float gravedad = -20f;
    public float velocidadRotacion = 200.0f;
    public float x, y;

    private CharacterController controller;
    private Vector3 velocidadVertical;

    private Animator animator;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    
void Update()
{
    x = Input.GetAxisRaw("Horizontal");
    y = Input.GetAxisRaw("Vertical");

    Vector3 direccion = new Vector3(x, 0f, y);
    direccion = Vector3.ClampMagnitude(direccion, 1f);


    if (direccion.sqrMagnitude > 0.01f){
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime );
    }

    bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    float velocidad = corriendo? velocidadCorrer : velocidadMovimiento;

    if (animator != null){animator.SetFloat("Speed", direccion.magnitude);}

    controller.Move( transform.forward * direccion.magnitude * velocidad * Time.deltaTime);


    if (controller.isGrounded && velocidadVertical.y < 0f) {
        velocidadVertical.y = -2f;
    }


    if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space)){ 
        velocidadVertical.y = fuerzaSalto;
    }

    velocidadVertical.y += gravedad * Time.deltaTime;
    controller.Move(velocidadVertical * Time.deltaTime);
}

}
