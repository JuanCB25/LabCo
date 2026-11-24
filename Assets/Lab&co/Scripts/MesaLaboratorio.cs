using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MesaLaboratorio : MonoBehaviour
{
    [Header("Anclaje")]
    [Tooltip("Objeto vacío hijo donde reposará el matraz")]
    [SerializeField] private Transform snapPoint;

    [Header("Indicador Visual")]
    [SerializeField] protected MeshRenderer indicadorVisual;
    [SerializeField] protected Color colorDisponible = new Color(0f, 1f, 0.8f, 0.4f);
    [SerializeField] protected Color colorOcupado = new Color(1f, 0.2f, 0.2f, 0.15f);

    protected MaterialPropertyBlock propBlock;
    protected ItemRecogible itemActual;

    public Transform SnapPoint => snapPoint != null ? snapPoint : transform;

    protected virtual void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        ActualizarIndicador();
    }

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

    public virtual bool PuedeRecibirItem() => !EstaOcupada;

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

    public virtual ItemRecogible RetirarItem()
    {
        if (itemActual == null) return null;

        ItemRecogible itemRetirado = itemActual;
        itemActual = null;

        itemRetirado.transform.SetParent(null);

        ActualizarIndicador();
        return itemRetirado;
    }

    // Permite verter el reactivo verde dentro de un matraz vacío sobre la mesa
    public virtual bool IntentarCombinarConItem(ref ItemRecogible itemEntrante)
    {
        if (itemActual == null || itemEntrante == null) return false;

        // Caso 1: Hay un matraz vacío en la mesa y el jugador trae el reactivo verde
        if (itemActual.Tipo == ItemRecogible.TipoItem.Matraz && itemEntrante.Tipo == ItemRecogible.TipoItem.ReactivoVerde)
        {
            MatrazQuimico matraz = itemActual.GetComponent<MatrazQuimico>();
            if (matraz != null && matraz.EstaVacio)
            {
                matraz.LlenarConReactivo();
                Destroy(itemEntrante.gameObject);
                itemEntrante = null;
                return true;
            }
        }

        // Caso 2: Hay un reactivo verde en la mesa y el jugador trae un matraz vacío
        if (itemActual.Tipo == ItemRecogible.TipoItem.ReactivoVerde && itemEntrante.Tipo == ItemRecogible.TipoItem.Matraz)
        {
            MatrazQuimico matraz = itemEntrante.GetComponent<MatrazQuimico>();
            if (matraz != null && matraz.EstaVacio)
            {
                matraz.LlenarConReactivo();
                ItemRecogible reactivoConsumido = RetirarItem();
                if (reactivoConsumido != null) Destroy(reactivoConsumido.gameObject);
                return true;
            }
        }

        return false;
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