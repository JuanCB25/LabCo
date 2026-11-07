using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform holdPoint;

    [Header("Parámetros de Detección")]
    [SerializeField] private float radioDeteccion = 1.2f;
    [SerializeField] private LayerMask capaObjetos;

    [Header("Lanzamiento")]
    [SerializeField] private float fuerzaLanzamiento = 8f;
    [SerializeField] private float anguloElevacion = 0.3f; // Elevación para la parábola

    private ItemRecogible objetoEnMano;

    void Update()
    {
        // Tecla E para Coger / Soltar
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objetoEnMano == null)
            {
                IntentarCoger();
            }
            else
            {
                SoltarObjeto();
            }
        }

        // Tecla Espacio para Lanzar
        if (Input.GetKeyDown(KeyCode.C) && objetoEnMano != null)
        {
            LanzarObjeto();
        }
    }

   private void IntentarCoger()
    {
        // Punto de detección elevado a la altura de la cintura/mesa
        Vector3 puntoDeteccion = transform.position + Vector3.up * 0.8f + transform.forward * 0.8f;
        Collider[] detectados = Physics.OverlapSphere(puntoDeteccion, radioDeteccion, capaObjetos);

        if (detectados.Length == 0)
        {
            Debug.Log("No hay nada dentro del radio de detección al pulsar E.");
            return;
        }

        // Recorremos todos los colliders detectados en el área
        foreach (Collider col in detectados)
        {
            Debug.Log("Analizando collider: " + col.name);

            // Si el collider detectado es del propio jugador, lo ignoramos
            if (col.gameObject == gameObject || col.transform.IsChildOf(transform))
            {
                continue;
            }

            ItemRecogible item = col.GetComponentInParent<ItemRecogible>();
            if (item != null)
            {
                objetoEnMano = item;
                objetoEnMano.Coger(holdPoint);
                Debug.Log("¡Objeto cogido con éxito: " + item.name + "!");
                return; // Detenemos la búsqueda al coger el objeto
            }
        }

        Debug.LogWarning("Se detectaron objetos con capa Interactuable, pero ninguno tenía ItemRecogible.");
    }

    private void SoltarObjeto()
    {
        objetoEnMano.Soltar();
        objetoEnMano = null;
    }

    private void LanzarObjeto()
    {
        // Calculamos un vector inclinado hacia arriba para la parábola física
        Vector3 dirLanzamiento = (transform.forward + Vector3.up * anguloElevacion).normalized;
        objetoEnMano.Lanzar(dirLanzamiento, fuerzaLanzamiento);
        objetoEnMano = null;
    }

    // Gizmo visual en el editor para calibrar el rango de alcance
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 puntoDeteccion = transform.position + transform.forward * 0.8f;
        Gizmos.DrawWireSphere(puntoDeteccion, radioDeteccion);
    }
}