
using TMPro;
using UnityEngine;

public class RecipeProgressUI : MonoBehaviour
{
    private TMP_Text textoPuntuacion;
    private OrderManager orderManager;

    private void Awake()
    {
        textoPuntuacion = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        orderManager = OrderManager.Instance;

        if (textoPuntuacion == null)
        {
            Debug.LogError(
                "RecipeProgressUI: no se encuentra el componente TMP_Text."
            );
        }
    }

    private void Update()
    {
        if (orderManager == null)
        {
            orderManager = OrderManager.Instance;
        }

        if (orderManager == null || textoPuntuacion == null)
            return;

        textoPuntuacion.text =
            orderManager.PuntuacionVisible + " / " +
            orderManager.PuntuacionObjetivo + " PUNTOS";
    }
}
