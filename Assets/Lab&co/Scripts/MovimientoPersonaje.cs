using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float playerSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float gravityValue = -20f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Control de suelo para la gravedad
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Obtener entrada de teclado (WASD o Flechas)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        // Mover al personaje en el plano XZ
        controller.Move(move * playerSpeed * Time.deltaTime);

        // Rotar suavemente hacia la dirección del movimiento
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Aplicar gravedad constante para mantenerlo pegado al suelo del sótano
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}