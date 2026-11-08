using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MesaLaboratorio : MonoBehaviour
{
    [Header("Anclaje")]
    [Tooltip("Objeto vacío hijo donde reposará el matraz")]
    [SerializeField] private Transform snapPoint;

    [Header("Indicador Visual")]
    [SerializeField] private MeshRenderer indicadorVisual;
    [SerializeField] private Color colorDisponible = new Color(0f, 1f, 0.8f, 0.4f);
    [SerializeField] private Color colorOcupado = new Color(1f, 0.2f, 0.2f, 0.15f);

    protected MaterialPropertyBlock propBlock;
    protected ItemRecogible itemActual;

    public Transform SnapPoint => snapPoint != null ? snapPoint : transform;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        ActualizarIndicador();
    }

    // Marcado como VIRTUAL para que el dispensador pueda sobreescribirlo
    public virtual bool EstaOcupada
    {
        get
        {
            if (itemActual != null && itemActual.transform.parent != SnapPoint)
            {
                itemActual = null;
            }
            return itemActual != null;
        }
    }

    // Marcado como VIRTUAL
    public virtual bool PuedeRecibirItem() => !EstaOcupada;

    // Marcado como VIRTUAL
    public virtual bool ColocarItem(ItemRecogible item)
    {
        if (item == null || EstaOcupada) return false;

        itemActual = item;
        itemActual.transform.SetParent(SnapPoint, true);
        itemActual.transform.position = SnapPoint.position;
        itemActual.transform.rotation = Quaternion.Euler(0f, SnapPoint.eulerAngles.y, 0f);

        if (itemActual.EscalaOriginal != Vector3.zero)
            itemActual.transform.localScale = itemActual.EscalaOriginal;

        Rigidbody rb = itemActual.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] colliders = itemActual.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) c.enabled = false;

        ActualizarIndicador();
        return true;
    }

    // Marcado como VIRTUAL
    public virtual ItemRecogible RetirarItem()
    {
        if (itemActual == null) return null;

        ItemRecogible itemRetirado = itemActual;
        itemActual = null;

        itemRetirado.transform.SetParent(null);

        ActualizarIndicador();
        return itemRetirado;
    }

    protected virtual void ActualizarIndicador()
    {
        if (indicadorVisual == null) return;

        if (propBlock == null) propBlock = new MaterialPropertyBlock();

        indicadorVisual.GetPropertyBlock(propBlock);
        Color colorFinal = EstaOcupada ? colorOcupado : colorDisponible;
        propBlock.SetColor("_BaseColor", colorFinal);
        indicadorVisual.SetPropertyBlock(propBlock);
    }
}