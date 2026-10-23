
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 4f;
    public float velocidadCorrer = 7f;
    public float fuerzaSalto = 6f;
    public float gravedad = -20f;

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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direccion =transform.right * horizontal + transform.forward * vertical;

        if (animator != null){
             animator.SetFloat("Speed", direccion.magnitude);
        }

        direccion = Vector3.ClampMagnitude(direccion, 1f);

        bool corriendo = Input.GetKey(KeyCode.LeftShift) ||  Input.GetKey(KeyCode.RightShift);

        float velocidad = corriendo ? velocidadCorrer : velocidadCaminar;

        controller.Move(direccion * velocidad * Time.deltaTime);

        if (controller.isGrounded && velocidadVertical.y < 0){
            velocidadVertical.y = -2f;
        }

        if (controller.isGrounded &&Input.GetKeyDown(KeyCode.Space)) {
            velocidadVertical.y = fuerzaSalto;
        }

        velocidadVertical.y += gravedad * Time.deltaTime;//esto es ara la gravedad
        controller.Move(velocidadVertical * Time.deltaTime);
    }
}
