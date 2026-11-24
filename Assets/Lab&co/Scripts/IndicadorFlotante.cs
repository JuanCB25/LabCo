using UnityEngine;

public class IndicadorFlotante : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float velocidadGiro = 55f;

    [Header("Levitación")]
    [SerializeField] private float amplitud = 0.08f;
    [SerializeField] private float frecuencia = 2.5f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // Giro continuo sobre el eje vertical
        transform.Rotate(Vector3.up, velocidadGiro * Time.deltaTime, Space.World);

        // Vaivén vertical suave
        float desplazamientoY = Mathf.Sin(Time.time * frecuencia) * amplitud;
        transform.localPosition = new Vector3(
            posicionInicial.x,
            posicionInicial.y + desplazamientoY,
            posicionInicial.z
        );
    }
}