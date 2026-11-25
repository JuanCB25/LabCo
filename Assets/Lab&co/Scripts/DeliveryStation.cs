
using UnityEngine;

public class DeliveryStation : MesaLaboratorio
{
    public override bool ColocarItem(ItemRecogible item)
    {
        if (item == null || EstaOcupada)
            return false;

        // Comprobar que el objeto es un matraz
        MatrazQuimico matraz = item.GetComponent<MatrazQuimico>();

        if (matraz == null)
        {
            Debug.Log("La estación de entrega solo acepta matraces.");
            return false;
        }

        // Comprobar que el matraz está cocinado
        if (matraz.EstaQuemado)
        {
            Debug.LogWarning("¡Entrega rechazada! El matraz está quemado.");
            return false;
        }

        if (!matraz.EstaCocinado)
        {
            Debug.LogWarning(
                "Entrega rechazada: el matraz todavía no está cocinado. "
            );
            return false;
        }

        // Comprobar que existe el gestor de pedidos
        if (OrderManager.Instance == null)
        {
            Debug.LogError("No se encuentra el OrderManager.");
            return false;
        }

        // Completar el pedido
        if (!OrderManager.Instance.CompletarPedido())
            return false;

        Debug.Log("¡Matraz entregado correctamente!");

        // Consumir el objeto entregado
        Destroy(item.gameObject);

        return true;
    }
}
