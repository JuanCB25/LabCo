using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public enum RolJugador { Jugador1, Jugador2 }

    [Header("Configuración del Jugador")]
    [SerializeField] private RolJugador rol = RolJugador.Jugador1;

    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 5f;
    [SerializeField] private float velocidadEsprint = 8f;
    [SerializeField] private float velocidadRotacion = 14f;

    [Header("Mando (Jugador 2)")]
    [SerializeField] private float deadzoneMando = 0.2f;

    [Header("Gravedad y Suelo")]
    [SerializeField] private float gravedad = -20f;
    [SerializeField] private float fuerzaPegadoSuelo = -2f;

    private CharacterController controller;
    private Vector3 velocidadVertical;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Mover();
    }

    private void Mover()
    {
        float horizontal = 0f;
        float vertical = 0f;
        bool estaEsprintando = false;

        if (rol == RolJugador.Jugador1)
        {
            // TECLADO (Jugador 1)
            if (Input.GetKey(KeyCode.W)) vertical += 1f;
            if (Input.GetKey(KeyCode.S)) vertical -= 1f;
            if (Input.GetKey(KeyCode.D)) horizontal += 1f;
            if (Input.GetKey(KeyCode.A)) horizontal -= 1f;

            estaEsprintando = Input.GetKey(KeyCode.LeftShift);
        }
        else
        {
            // MANDO - Stick Izquierdo (Jugador 2)
            horizontal = Input.GetAxisRaw("Mando_Horizontal");
            vertical = Input.GetAxisRaw("Mando_Vertical");

            // Respaldo con flechas del teclado si no se mueve el stick
            if (Mathf.Abs(horizontal) < deadzoneMando && Mathf.Abs(vertical) < deadzoneMando)
            {
                horizontal = 0f;
                vertical = 0f;
                if (Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
                if (Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
                if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;
                if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
            }

            // Esprintar con RB/R1 (Joystick1Button5), LB/L1 (Joystick1Button4) o Shift Derecho
            estaEsprintando = Input.GetKey(KeyCode.Joystick1Button5) ||
                              Input.GetKey(KeyCode.Joystick1Button4) ||
                              Input.GetKey(KeyCode.RightShift);
        }

        Vector3 direccionPlana = new Vector3(horizontal, 0f, vertical);

        if (direccionPlana.magnitude > 1f)
        {
            direccionPlana.Normalize();
        }

        float velocidadActual = estaEsprintando ? velocidadEsprint : velocidadNormal;
        Vector3 movimientoHorizontal = direccionPlana * velocidadActual;

        if (direccionPlana.sqrMagnitude > 0.001f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionPlana);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * velocidadRotacion);
        }

        if (controller.isGrounded)
        {
            velocidadVertical.y = fuerzaPegadoSuelo;
        }
        else
        {
            velocidadVertical.y += gravedad * Time.deltaTime;
        }

        Vector3 movimientoFinal = (movimientoHorizontal + velocidadVertical) * Time.deltaTime;
        controller.Move(movimientoFinal);
    }

    public void ConfigurarRol(RolJugador nuevoRol)
    {
        rol = nuevoRol;
    }
}