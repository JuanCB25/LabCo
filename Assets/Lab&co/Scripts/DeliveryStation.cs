
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

        // Comprobar que existe el gestor de pedidos
        if (OrderManager.Instance == null)
        {
            Debug.LogError("No se encuentra el OrderManager.");
            return false;
        }

        // Registrar la entrega según el estado del matraz
        MatrazQuimico.EstadoMatraz estado = matraz.EstadoActual;

        if (!OrderManager.Instance.RegistrarEntrega(estado))
        {
            return false;
        }

        Debug.Log("Matraz entregado. Estado: " + estado);

        // Consumir el objeto entregado
        Destroy(item.gameObject);

        return true;
    }
}
