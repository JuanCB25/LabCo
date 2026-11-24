using System.Collections;
using UnityEngine;

public class MatrazQuimico : MonoBehaviour
{
    public enum EstadoMatraz
    {
        Vacio,
        ConReactivo,
        Cocinado,
        Quemado
    }

    [Header("Estado Actual")]
    [SerializeField] private EstadoMatraz estadoActual = EstadoMatraz.Vacio;

    [Header("Visual del Líquido Interior")]
    [Tooltip("Malla hija que representa el líquido dentro del matraz")]
    [SerializeField] private GameObject objetoLiquido;
    [SerializeField] private MeshRenderer rendererLiquido;

    [Header("Colores por Estado")]
    [SerializeField] private Color colorReactivoVerde = new Color(0.15f, 0.95f, 0.25f, 0.9f);
    [SerializeField] private Color colorCocinado = new Color(0f, 0.85f, 1f, 0.95f);
    [SerializeField] private Color colorQuemado = new Color(0.12f, 0.1f, 0.1f, 0.98f);

    [Header("Efectos y Animación de Quemado")]
    [Tooltip("Sistema de partículas de humo negro hijo del matraz")]
    [SerializeField] private ParticleSystem particulasHumoNegro;
    [SerializeField] private float duracionSacudida = 0.45f;
    [SerializeField] private float intensidadSacudida = 0.06f;

    private MaterialPropertyBlock propBlock;
    private Coroutine corrutinaAnimacion;

    public EstadoMatraz EstadoActual => estadoActual;
    public bool EstaVacio => estadoActual == EstadoMatraz.Vacio;
    public bool TieneReactivo => estadoActual == EstadoMatraz.ConReactivo;
    public bool EstaCocinado => estadoActual == EstadoMatraz.Cocinado;
    public bool EstaQuemado => estadoActual == EstadoMatraz.Quemado;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        ActualizarVisual();
    }

    public void LlenarConReactivo()
    {
        if (estadoActual != EstadoMatraz.Vacio) return;
        estadoActual = EstadoMatraz.ConReactivo;
        ActualizarVisual();
    }

    public void CompletarCoccion()
    {
        if (estadoActual != EstadoMatraz.ConReactivo) return;
        estadoActual = EstadoMatraz.Cocinado;
        ActualizarVisual();
    }

    public void Quemar()
    {
        if (estadoActual == EstadoMatraz.Vacio || estadoActual == EstadoMatraz.Quemado) return;

        estadoActual = EstadoMatraz.Quemado;
        ActualizarVisual();

        // Lanza una sacudida visual rápida para avisar al jugador de que se acaba de quemar
        if (corrutinaAnimacion != null) StopCoroutine(corrutinaAnimacion);
        corrutinaAnimacion = StartCoroutine(AnimacionQuemado());
    }

    public void Vaciar()
    {
        if (estadoActual == EstadoMatraz.Vacio) return;

        // Si seguía ejecutándose la sacudida de quemado, la detenemos y restauramos escala
        if (corrutinaAnimacion != null)
        {
            StopCoroutine(corrutinaAnimacion);
            corrutinaAnimacion = null;
            if (transform.parent != null) transform.localPosition = Vector3.zero;
        }

        estadoActual = EstadoMatraz.Vacio;
        ActualizarVisual();
    }

    private void ActualizarVisual()
    {
        // Control de partículas de humo negro
        if (particulasHumoNegro != null)
        {
            if (estadoActual == EstadoMatraz.Quemado)
            {
                if (!particulasHumoNegro.isPlaying)
                    particulasHumoNegro.Play();
            }
            else
            {
                if (particulasHumoNegro.isPlaying)
                    particulasHumoNegro.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        if (objetoLiquido == null) return;

        if (estadoActual == EstadoMatraz.Vacio)
        {
            objetoLiquido.SetActive(false);
            return;
        }

        objetoLiquido.SetActive(true);

        if (rendererLiquido != null)
        {
            if (propBlock == null) propBlock = new MaterialPropertyBlock();

            Color colorObjetivo = colorReactivoVerde;
            if (estadoActual == EstadoMatraz.Cocinado) colorObjetivo = colorCocinado;
            else if (estadoActual == EstadoMatraz.Quemado) colorObjetivo = colorQuemado;

            rendererLiquido.GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", colorObjetivo);
            rendererLiquido.SetPropertyBlock(propBlock);
        }
    }

    private IEnumerator AnimacionQuemado()
    {
        Vector3 posicionLocalBase = transform.localPosition;
        Vector3 escalaBase = transform.localScale;
        float tiempo = 0f;

        while (tiempo < duracionSacudida)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionSacudida;
            float amortiguacion = 1f - progreso;

            // Temblor lateral rápido
            float offsetX = Mathf.Sin(tiempo * 55f) * intensidadSacudida * amortiguacion;
            float offsetZ = Mathf.Cos(tiempo * 48f) * intensidadSacudida * amortiguacion;
            transform.localPosition = posicionLocalBase + new Vector3(offsetX, 0f, offsetZ);

            // Pequeño hinchazón tipo "puff" al quemarse
            float factorEscala = 1f + Mathf.Sin(progreso * Mathf.PI) * 0.22f;
            transform.localScale = escalaBase * factorEscala;

            yield return null;
        }

        transform.localPosition = posicionLocalBase;
        transform.localScale = escalaBase;
        corrutinaAnimacion = null;
    }
}