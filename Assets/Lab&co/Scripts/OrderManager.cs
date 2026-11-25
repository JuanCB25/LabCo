
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Estado del Pedido")]
    [SerializeField] private int pedidoActual = 1;
    [SerializeField] private bool pedidoCompletado = false;

    [Header("Final del Nivel")]
    [SerializeField] private GameOverManager gameOverManager;

    public int PedidoActual => pedidoActual;
    public bool PedidoCompletado => pedidoCompletado;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool CompletarPedido()
    {
        // Evitar completar el mismo pedido varias veces
        if (pedidoCompletado)
            return false;

        // Buscar el GameOverManager si no está asignado
        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }

        if (gameOverManager == null)
        {
            Debug.LogError("No se encuentra el GameOverManager.");
            return false;
        }

        pedidoCompletado = true;

        Debug.Log("¡Pedido " + pedidoActual + " completado!");

        // Terminar el nivel y desbloquear el siguiente
        gameOverManager.ShowLevelComplete();

        return true;
    }
}
