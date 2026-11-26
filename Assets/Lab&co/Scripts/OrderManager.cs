
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Estado del Pedido")]
    [SerializeField] private int pedidoActual = 1;
    [SerializeField] private bool pedidoCompletado = false;

    [Header("Puntuacion del Pedido")]
    [SerializeField] private int puntuacionObjetivo = 500;
    [SerializeField] private int puntosRecetaCorrecta = 50;
    [SerializeField] private int penalizacionSinCocinar = -50;
    [SerializeField] private int penalizacionQuemado = -100;

    private int puntuacionActual = 0;

    [Header("Final del Nivel")]
    [SerializeField] private GameOverManager gameOverManager;

    public int PedidoActual => pedidoActual;
    public bool PedidoCompletado => pedidoCompletado;

    public int PuntuacionActual => puntuacionActual;
    public int PuntuacionObjetivo => puntuacionObjetivo;

    // Puntuacion mostrada en el contador.
    // No permite mostrar valores negativos.
    public int PuntuacionVisible => Mathf.Max(0, puntuacionActual);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }
    }

    public bool RegistrarEntrega(MatrazQuimico.EstadoMatraz estado)
    {
        if (pedidoCompletado)
            return false;

        int puntos = 0;

        switch (estado)
        {
            case MatrazQuimico.EstadoMatraz.Cocinado:
                puntos = puntosRecetaCorrecta;
                break;

            case MatrazQuimico.EstadoMatraz.ConReactivo:
                puntos = penalizacionSinCocinar;
                break;

            case MatrazQuimico.EstadoMatraz.Quemado:
                puntos = penalizacionQuemado;
                break;

            case MatrazQuimico.EstadoMatraz.Vacio:
                Debug.LogWarning("No se puede entregar un matraz vacío.");
                return false;
        }
        //NO DEBE BAJAR DE 0 PUNTOS
        puntuacionActual = Mathf.Max(0, puntuacionActual + puntos);

        Debug.Log(
            "Entrega registrada: " + puntos +
            " puntos. Puntuación actual: " +
            puntuacionActual + "/" + puntuacionObjetivo
        );

        if (puntuacionActual >= puntuacionObjetivo)
        {
            FinalizarPedido();
        }

        return true;
    }

    private void FinalizarPedido()
    {
        if (pedidoCompletado)
            return;

        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }

        pedidoCompletado = true;

        Debug.Log("¡Pedido " + pedidoActual + " completado!");

        if (gameOverManager != null)
        {
            gameOverManager.ShowLevelComplete();
        }
        else
        {
            Debug.LogError("No se encuentra el GameOverManager.");
        }
    }

    // Compatibilidad temporal con la DeliveryStation antigua.
    // La estación actual solo llama a este método
    // cuando el matraz está cocinado correctamente.
    public bool CompletarPedido()
    {
        return RegistrarEntrega(
            MatrazQuimico.EstadoMatraz.Cocinado
        );
    }
}
