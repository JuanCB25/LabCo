using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemRecogible : MonoBehaviour
{
    public enum TipoItem
    {
        Otro,
        Matraz,
        ReactivoVerde
    }

    [Header("Clasificación del Ítem")]
    [SerializeField] private TipoItem tipo = TipoItem.Otro;

    private Rigidbody rb;
    private Collider[] collidersPropios;

    public TipoItem Tipo => tipo;
    public Vector3 EscalaOriginal { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collidersPropios = GetComponentsInChildren<Collider>(true);
        EscalaOriginal = transform.localScale;
    }

    public void Coger(Transform holdPoint)
    {
        if (holdPoint == null) return;

        if (rb == null) rb = GetComponent<Rigidbody>();
        if (collidersPropios == null || collidersPropios.Length == 0)
            collidersPropios = GetComponentsInChildren<Collider>(true);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        foreach (Collider col in collidersPropios)
        {
            col.enabled = false;
        }

        Vector3 escala = EscalaOriginal != Vector3.zero ? EscalaOriginal : transform.localScale;

        transform.SetParent(holdPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = escala;
    }

    public void Soltar()
    {
        transform.SetParent(null);

        if (collidersPropios == null || collidersPropios.Length == 0)
            collidersPropios = GetComponentsInChildren<Collider>(true);

        foreach (Collider col in collidersPropios)
        {
            col.enabled = true;
            col.isTrigger = false;
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
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddForce(direccion * fuerza, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (transform.parent != null) return;

        MesaLaboratorio mesa = collision.gameObject.GetComponentInParent<MesaLaboratorio>();
        if (mesa != null)
        {
            ContactPoint contacto = collision.GetContact(0);
            bool impactoDesdeArriba = contacto.normal.y > 0.4f;
            bool alturaValida = contacto.point.y >= (mesa.SnapPoint.position.y - 0.35f);

            if (impactoDesdeArriba || alturaValida)
            {
                // Si se lanza un reactivo verde encima de una mesa con matraz vacío, se vierte directo
                ItemRecogible referenciaPropia = this;
                if (mesa.IntentarCombinarConItem(ref referenciaPropia))
                {
                    return;
                }

                if (mesa.PuedeRecibirItem())
                {
                    mesa.ColocarItem(this);
                }
            }
        }
    }
}