using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    public enum RolJugador { Jugador1, Jugador2 }

    [Header("Identificación")]
    [SerializeField] private RolJugador rol = RolJugador.Jugador1;

    [Header("Referencias")]
    [SerializeField] private Transform holdPoint;

    [Header("Detección")]
    [SerializeField] private float distanciaFrontal = 0.5f;
    [SerializeField] private float radioDeteccion = 0.5f;
    [SerializeField] private float distanciaMaxSnap = 0.9f;
    [SerializeField] private LayerMask capaObjetos;

    [Header("Lanzamiento")]
    [SerializeField] private float fuerzaLanzamiento = 8.5f;
    [SerializeField] private float anguloElevacion = 0.25f;

    private ItemRecogible objetoEnMano;

    void Update()
    {
        // Tecla de Interacción (Coger / Colocar / Soltar)
        if (EsTeclaInteractuarPulsada())
        {
            GestionarInteraccion();
        }

        // Tecla de Lanzar
        if (EsTeclaLanzarPulsada() && objetoEnMano != null)
        {
            LanzarObjeto();
        }
    }

    private bool EsTeclaInteractuarPulsada()
    {
        if (rol == RolJugador.Jugador1)
        {
            return Input.GetKeyDown(KeyCode.E);
        }
        else
        {
            // Mando: Botón A (Xbox) / Cruz X (PlayStation) + respaldo Enter
            return Input.GetKeyDown(KeyCode.Joystick1Button0) ||
                   Input.GetKeyDown(KeyCode.Return) ||
                   Input.GetKeyDown(KeyCode.KeypadEnter);
        }
    }

    private bool EsTeclaLanzarPulsada()
    {
        if (rol == RolJugador.Jugador1)
        {
            return Input.GetKeyDown(KeyCode.C);
        }
        else
        {
            // Mando: Botón X (Xbox) / Cuadrado (PS) o Botón B (Xbox) / Círculo (PS) + respaldo Backspace
            return Input.GetKeyDown(KeyCode.Joystick1Button2) ||
                   Input.GetKeyDown(KeyCode.Joystick1Button1) ||
                   Input.GetKeyDown(KeyCode.Backspace);
        }
    }

    private void GestionarInteraccion()
    {
        Vector3 puntoDeteccion = transform.position + Vector3.up * 0.7f + transform.forward * distanciaFrontal;
        Collider[] detectados = Physics.OverlapSphere(puntoDeteccion, radioDeteccion, capaObjetos);

        // 1. PRIORIDAD: Buscar mesa cercana
        MesaLaboratorio mesaCandidata = null;
        float distanciaMinima = float.MaxValue;

        foreach (Collider col in detectados)
        {
            MesaLaboratorio mesa = col.GetComponentInParent<MesaLaboratorio>();
            if (mesa != null)
            {
                float distAlSnap = Vector3.Distance(puntoDeteccion, mesa.SnapPoint.position);
                if (distAlSnap <= distanciaMaxSnap && distAlSnap < distanciaMinima)
                {
                    distanciaMinima = distAlSnap;
                    mesaCandidata = mesa;
                }
            }
        }

        if (mesaCandidata != null)
        {
            // 1. Mesa libre: intentar colocar el objeto que llevamos
            if (objetoEnMano != null && mesaCandidata.PuedeRecibirItem())
            {
                if (mesaCandidata.ColocarItem(objetoEnMano))
                {
                    objetoEnMano = null;
                }
                return;
            }

            // 2. Mesa ocupada y manos vacías: retirar objeto
            if (objetoEnMano == null && mesaCandidata.EstaOcupada)
            {
                if (holdPoint == null) return;

                ItemRecogible itemObtenido = mesaCandidata.RetirarItem();
                if (itemObtenido != null)
                {
                    objetoEnMano = itemObtenido;
                    objetoEnMano.Coger(holdPoint);
                }
                return;
            }

            // 3. Mesa ocupada y tenemos objeto en mano: intentar combinar (vertido)
            if (objetoEnMano != null && mesaCandidata.EstaOcupada)
            {
                mesaCandidata.IntentarCombinarConItem(ref objetoEnMano);
                return;
            }
        }

        // 2. SEGUNDA OPCIÓN: Suelo
        if (objetoEnMano != null)
        {
            objetoEnMano.Soltar();
            objetoEnMano = null;
        }
        else
        {
            foreach (Collider col in detectados)
            {
                // Ignorar colisiones consigo mismo o sus propios hijos
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
        Gizmos.color = (rol == RolJugador.Jugador1) ? Color.yellow : Color.magenta;
        Vector3 puntoDeteccion = transform.position + Vector3.up * 0.7f + transform.forward * distanciaFrontal;
        Gizmos.DrawWireSphere(puntoDeteccion, radioDeteccion);
    }
}