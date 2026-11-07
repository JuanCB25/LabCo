using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemRecogible : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void Coger(Transform holdPoint)
    {
        if (holdPoint == null) return;

        // Búsqueda de seguridad por si Awake no capturó la referencia a tiempo
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (col == null) col = GetComponent<Collider>();

        if (rb != null)
        {
            // Detener cualquier inercia previa antes de emparentar
            rb.linearVelocity = Vector3.zero; // En versiones anteriores a Unity 6 usa: rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Desactivamos el collider para que no empuje al jugador ni caiga por gravedad
        if (col != null)
        {
            col.enabled = false;
        }

        Vector3 escalaOriginal = transform.localScale;

        // El segundo parámetro (false) emparenta reseteando las coordenadas locales a (0,0,0)
        transform.SetParent(holdPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = escalaOriginal;
    }

    public void Soltar()
    {
        transform.SetParent(null);

        // Reactivamos el collider
        if (col != null)
        {
            col.enabled = true;
        }

        // Reactivamos la física normal
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void Lanzar(Vector3 direccion, float fuerza)
    {
        Soltar();

        if (rb != null)
        {
            rb.AddForce(direccion * fuerza, ForceMode.Impulse);
        }
    }
}