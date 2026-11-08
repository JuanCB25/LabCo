using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimientoJugador : MonoBehaviour
{
    public enum RolJugador { Jugador1, Jugador2 }

    [Header("Configuración del Jugador")]
    [SerializeField] private RolJugador rol = RolJugador.Jugador1;

    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 5f;
    [SerializeField] private float velocidadEsprint = 8f;
    [SerializeField] private float velocidadRotacion = 14f;

    [Header("Gravedad y Suelo")]
    [SerializeField] private float gravedad = -20f;
    [SerializeField] private float fuerzaPegadoSuelo = -2f; // Mantiene los pies en el suelo y rampas

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
        // 1. Obtener la entrada de movimiento según el jugador
        float horizontal = 0f;
        float vertical = 0f;
        bool estaEsprintando = false;

        if (rol == RolJugador.Jugador1)
        {
            if (Input.GetKey(KeyCode.W)) vertical += 1f;
            if (Input.GetKey(KeyCode.S)) vertical -= 1f;
            if (Input.GetKey(KeyCode.D)) horizontal += 1f;
            if (Input.GetKey(KeyCode.A)) horizontal -= 1f;

            estaEsprintando = Input.GetKey(KeyCode.LeftShift);
        }
        else // Jugador 2
        {
            if (Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
            if (Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;
            if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;

            estaEsprintando = Input.GetKey(KeyCode.RightShift);
        }

        Vector3 direccionPlana = new Vector3(horizontal, 0f, vertical).normalized;

        // 2. Velocidad horizontal
        float velocidadActual = estaEsprintando ? velocidadEsprint : velocidadNormal;
        Vector3 movimientoHorizontal = direccionPlana * velocidadActual;

        // 3. Rotación hacia la dirección del movimiento
        if (direccionPlana.sqrMagnitude > 0.001f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionPlana);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * velocidadRotacion);
        }

        // 4. Gravedad y fijación al suelo (sin salto)
        if (controller.isGrounded)
        {
            // Pequeña fuerza descendente constante para no flotar en bajadas de escaleras
            velocidadVertical.y = fuerzaPegadoSuelo;
        }
        else
        {
            velocidadVertical.y += gravedad * Time.deltaTime;
        }

        // 5. Aplicar desplazamiento total al CharacterController
        Vector3 movimientoFinal = (movimientoHorizontal + velocidadVertical) * Time.deltaTime;
        controller.Move(movimientoFinal);
    }

    public void ConfigurarRol(RolJugador nuevoRol)
    {
        rol = nuevoRol;
    }
}