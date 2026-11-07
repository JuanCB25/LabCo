using UnityEngine;

[RequireComponent(typeof(Collider))]
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
        // Desactivamos la física mientras lo llevamos en brazos
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // Lo emparentamos y reseteamos su posición local
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Soltar()
    {
        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
    }

    public void Lanzar(Vector3 direccion, float fuerza)
    {
        Soltar();

        if (rb != null)
        {
            // Impulso parabólico 3D
            rb.AddForce(direccion * fuerza, ForceMode.Impulse);
        }
    }
}