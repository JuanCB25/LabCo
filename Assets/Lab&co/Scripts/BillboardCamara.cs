using UnityEngine;

public class BillboardCamara : MonoBehaviour
{
    private Transform camaraPrincipal;

    void Start()
    {
        if (Camera.main != null)
        {
            camaraPrincipal = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (camaraPrincipal != null)
        {
            transform.forward = camaraPrincipal.forward;
        }
    }
}