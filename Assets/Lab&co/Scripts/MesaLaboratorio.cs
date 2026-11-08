using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MesaLaboratorio : MonoBehaviour
{
    [Header("Anclaje")]
    [Tooltip("Objeto vacío hijo donde reposará el matraz")]
    [SerializeField] private Transform snapPoint;

    private ItemRecogible itemActual;

    public Transform SnapPoint => snapPoint != null ? snapPoint : transform;

    // Comprobación doble: variable en memoria Y presencia real de hijos en el Transform
    public bool EstaOcupada
    {
        get
        {
            if (itemActual != null) return true;

            // Salvaguarda: si la variable se perdió pero hay un objeto físico emparentado
            if (SnapPoint.childCount > 0)
            {
                itemActual = SnapPoint.GetComponentInChildren<ItemRecogible>();
                return itemActual != null;
            }

            return false;
        }
    }

    public bool PuedeRecibirItem()
    {
        return !EstaOcupada;
    }

    public bool ColocarItem(ItemRecogible item)
    {
        if (item == null || EstaOcupada) return false;

        itemActual = item;

        // Emparentamos respetando la escala y posición de mundo actual
        itemActual.transform.SetParent(SnapPoint, true);

        // Ajustamos la posición y rotación exactas al SnapPoint
        itemActual.transform.position = SnapPoint.position;
        itemActual.transform.rotation = SnapPoint.rotation;

        // Si el objeto tenía guardada su escala de fábrica, la restauramos
        if (itemActual.EscalaOriginal != Vector3.zero)
        {
            itemActual.transform.localScale = itemActual.EscalaOriginal;
        }

        // Desactivamos físicas
        Rigidbody rb = itemActual.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Apagamos colliders para evitar solapamientos
        Collider[] colliders = itemActual.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        return true;
    }

    public ItemRecogible RetirarItem()
    {
        if (!EstaOcupada) return null;

        ItemRecogible itemRetirado = itemActual;
        itemActual = null;
        return itemRetirado;
    }

    private void OnDrawGizmosSelected()
    {
        if (snapPoint != null)
        {
            Gizmos.color = EstaOcupada ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(snapPoint.position, 0.2f);
        }
    }
}