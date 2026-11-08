using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform holdPoint;

    [Header("Detección")]
    [Tooltip("Distancia hacia delante desde el centro del personaje")]
    [SerializeField] private float distanciaFrontal = 0.5f;
    [Tooltip("Radio de la esfera de detección")]
    [SerializeField] private float radioDeteccion = 0.5f;
    [SerializeField] private LayerMask capaObjetos;

    [Header("Ajuste de Mesas")]
    [Tooltip("Distancia máxima real entre el punto de mira y el SnapPoint para poder dejar/coger")]
    [SerializeField] private float distanciaMaxSnap = 0.9f;

    [Header("Lanzamiento")]
    [SerializeField] private float fuerzaLanzamiento = 8f;
    [SerializeField] private float anguloElevacion = 0.3f;

    private ItemRecogible objetoEnMano;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GestionarInteraccion();
        }

        if (Input.GetKeyDown(KeyCode.C) && objetoEnMano != null)
        {
            LanzarObjeto();
        }
    }

    private void GestionarInteraccion()
    {
        Vector3 puntoDeteccion = transform.position + Vector3.up * 0.7f + transform.forward * distanciaFrontal;
        Collider[] detectados = Physics.OverlapSphere(puntoDeteccion, radioDeteccion, capaObjetos);

        // 1. PRIORIDAD: Buscar la MESA MÁS CERCANA cuyo SnapPoint esté a tiro
        MesaLaboratorio mesaCandidata = null;
        float distanciaMinima = float.MaxValue;

        foreach (Collider col in detectados)
        {
            MesaLaboratorio mesa = col.GetComponentInParent<MesaLaboratorio>();
            if (mesa != null)
            {
                // Medimos la distancia real desde las manos/mirada hasta el SnapPoint de la mesa
                float distAlSnap = Vector3.Distance(puntoDeteccion, mesa.SnapPoint.position);

                // Solo la consideramos si está dentro del umbral de cercanía real
                if (distAlSnap <= distanciaMaxSnap && distAlSnap < distanciaMinima)
                {
                    distanciaMinima = distAlSnap;
                    mesaCandidata = mesa;
                }
            }
        }

        // Si encontramos una mesa suficientemente cercana
        if (mesaCandidata != null)
        {
            // Colocar objeto
            if (objetoEnMano != null && mesaCandidata.PuedeRecibirItem())
            {
                mesaCandidata.ColocarItem(objetoEnMano);
                objetoEnMano = null;
                return;
            }

            // Retirar objeto
            if (objetoEnMano == null && mesaCandidata.EstaOcupada)
            {
                objetoEnMano = mesaCandidata.RetirarItem();
                objetoEnMano.Coger(holdPoint);
                return;
            }

            // Si ambos están ocupados, bloqueamos la acción para no tirar nada al suelo
            if (objetoEnMano != null && mesaCandidata.EstaOcupada)
            {
                return;
            }
        }

        // 2. SEGUNDA OPCIÓN: Suelo / Espacio abierto
        if (objetoEnMano != null)
        {
            objetoEnMano.Soltar();
            objetoEnMano = null;
        }
        else
        {
            foreach (Collider col in detectados)
            {
                if (col.gameObject == gameObject || col.transform.IsChildOf(transform)) continue;

                ItemRecogible itemSuelo = col.GetComponentInParent<ItemRecogible>();
                if (itemSuelo != null)
                {
                    objetoEnMano = itemSuelo;
                    objetoEnMano.Coger(holdPoint);
                    return;
                }
            }
        }
    }

    private void LanzarObjeto()
    {
        Vector3 dirLanzamiento = (transform.forward + Vector3.up * anguloElevacion).normalized;
        objetoEnMano.Lanzar(dirLanzamiento, fuerzaLanzamiento);
        objetoEnMano = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 puntoDeteccion = transform.position + Vector3.up * 0.7f + transform.forward * distanciaFrontal;
        Gizmos.DrawWireSphere(puntoDeteccion, radioDeteccion);
    }
}