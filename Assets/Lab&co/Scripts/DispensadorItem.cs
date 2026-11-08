using UnityEngine;

public class DispensadorItem : MesaLaboratorio
{
    [Header("Configuración del Dispensador")]
    [Tooltip("Prefab del objeto que entregará este dispensador")]
    [SerializeField] private GameObject prefabItem;

    [Header("Feedback Opcional")]
    [SerializeField] private ParticleSystem particulasExtraccion;

    // Sobreescribe indicando que siempre tiene material listo
    public override bool EstaOcupada => true;

    // Bloquea recibir objetos
    public override bool PuedeRecibirItem() => false;
    public override bool ColocarItem(ItemRecogible item) => false;

    // Genera una copia del prefab en las manos del jugador
    public override ItemRecogible RetirarItem()
    {
        if (prefabItem == null)
        {
            Debug.LogWarning($"[DispensadorItem] El dispensador '{gameObject.name}' no tiene ningún Prefab asignado.");
            return null;
        }

        GameObject nuevoObjeto = Instantiate(prefabItem, SnapPoint.position, SnapPoint.rotation);
        ItemRecogible item = nuevoObjeto.GetComponent<ItemRecogible>();

        if (item == null)
        {
            Debug.LogError($"[DispensadorItem] El prefab asignado a '{gameObject.name}' no tiene el componente ItemRecogible.");
            return null;
        }

        if (particulasExtraccion != null)
        {
            particulasExtraccion.Play();
        }

        return item;
    }

    // En el dispensador no cambiamos de color ya que siempre está disponible para coger
    protected override void ActualizarIndicador() { }
}