using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemRecogible : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    // Añade esta propiedad pública accesible para las mesas
    public Vector3 EscalaOriginal { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        // Guardamos el tamaño real del prefab antes de cualquier emparentamiento
        EscalaOriginal = transform.localScale;
    }

    public void Coger(Transform holdPoint)
    {
        if (holdPoint == null) return;

        if (rb == null) rb = GetComponent<Rigidbody>();
        if (col == null) col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        Vector3 escalaOriginal = transform.localScale;

        transform.SetParent(holdPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = escalaOriginal;
    }

    public void Soltar()
    {
        transform.SetParent(null);

        // Reactivamos todos los colliders del objeto
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
    }

    public void Lanzar(Vector3 direccion, float fuerza)
    {
        Soltar();

        if (rb != null)
        {
            // Detección continua durante el vuelo para no perder el impacto a gran velocidad
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddForce(direccion * fuerza, ForceMode.Impulse);
        }
    }

   private void OnCollisionEnter(Collision collision)
    {
        if (transform.parent != null) return;

        MesaLaboratorio mesa = collision.gameObject.GetComponentInParent<MesaLaboratorio>();
        if (mesa != null && mesa.PuedeRecibirItem())
        {
            ContactPoint contacto = collision.GetContact(0);
            bool impactoDesdeArriba = contacto.normal.y > 0.4f;
            bool alturaValida = contacto.point.y >= (mesa.SnapPoint.position.y - 0.35f);

            if (impactoDesdeArriba || alturaValida)
            {
                // Si ColocarItem devuelve false (mesa ocupada en ese frame), no hace nada y rebota
                mesa.ColocarItem(this);
            }
        }
    }
}