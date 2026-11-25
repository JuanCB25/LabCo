using UnityEngine;
using UnityEngine.UI;

public class ReactorQuimico : MesaLaboratorio
{
    [Header("Tiempos del Reactor (Segundos)")]
    [SerializeField] private float tiempoCoccion = 4f;
    [SerializeField] private float tiempoAntesDeQuemarse = 5f;

    [Header("Colores de Estado del Reactor")]
    [SerializeField] private Color colorProcesando = new Color(1f, 0.65f, 0f, 0.7f);   // Naranja
    [SerializeField] private Color colorListo = new Color(0.1f, 1f, 0.3f, 0.85f);       // Verde brillante
    [SerializeField] private Color colorPeligro = new Color(1f, 0.1f, 0.1f, 0.9f);      // Rojo alarma
    [SerializeField] private Color colorQuemado = new Color(0.25f, 0.05f, 0.05f, 0.9f); // Rojo oscuro

    [Header("UI Progreso Opcional (Canvas World Space)")]
    [SerializeField] private GameObject contenedorBarra;
    [SerializeField] private Image rellenoBarra;

    private MatrazQuimico matrazEnReactor;
    private float temporizador = 0f;

    protected override void Awake()
    {
        base.Awake();
        if (contenedorBarra != null) contenedorBarra.SetActive(false);
    }

    void Update()
    {
    
        if (matrazEnReactor == null) return;

        // FASE 1: Cocinando
        if (matrazEnReactor.TieneReactivo)
        {
            temporizador += Time.deltaTime;

            float progresoCoccion = temporizador / tiempoCoccion;
            ActualizarBarraUI(progresoCoccion, colorProcesando);
            PintarIndicador(colorProcesando);

            if (temporizador >= tiempoCoccion)
            {
                matrazEnReactor.CompletarCoccion();

                temporizador = 0f;

                // Mostrar inmediatamente el estado cocinado
                ActualizarBarraUI(0f, colorListo);
                PintarIndicador(colorListo);

                Debug.Log("¡MATRAZ COCINADO! Retíralo del reactor.");
                Debug.Log("Estado actual: " + matrazEnReactor.EstadoActual);
            }
        }
        // FASE 2: Cocinado, esperando a que se retire
        else if (matrazEnReactor.EstaCocinado)
        {
            temporizador += Time.deltaTime;

            float tiempoMaximo = Mathf.Max(0.1f, tiempoAntesDeQuemarse);
            float progresoQuemado = temporizador / tiempoMaximo;

            // La barra se mantiene verde durante la primera mitad
            if (progresoQuemado < 0.5f)
            {
                ActualizarBarraUI(progresoQuemado, colorListo);
                PintarIndicador(colorListo);
            }
            else
            {
                // En la segunda mitad avisa de que está a punto de quemarse
                ActualizarBarraUI(progresoQuemado, colorPeligro);

                bool pulso = Mathf.PingPong(Time.time * 8f, 1f) > 0.5f;
                PintarIndicador(pulso ? colorPeligro : colorListo);
            }

            if (temporizador >= tiempoMaximo)
            {
                matrazEnReactor.Quemar();

                ActualizarBarraUI(1f, colorQuemado);
                PintarIndicador(colorQuemado);

                Debug.LogWarning("¡MATRAZ QUEMADO!");
            }
        }
            // FASE 3: Matraz quemado
        else if (matrazEnReactor.EstaQuemado)
        {
            ActualizarBarraUI(1f, colorQuemado);
            PintarIndicador(colorQuemado);
        }
    }

    

    public override bool ColocarItem(ItemRecogible item)
    {
        if (item == null || EstaOcupada) return false;

        // El reactor SOLO acepta matraces que ya tengan el reactivo verde dentro
        MatrazQuimico matraz = item.GetComponent<MatrazQuimico>();
        if (matraz == null || !matraz.TieneReactivo)
        {
            return false;
        }

        if (base.ColocarItem(item))
        {
            matrazEnReactor = matraz;
            temporizador = 0f;
            if (contenedorBarra != null) contenedorBarra.SetActive(true);
            ActualizarIndicador();
            return true;
        }

        return false;
    }

    public override ItemRecogible RetirarItem()
    {
        matrazEnReactor = null;
        temporizador = 0f;
        if (contenedorBarra != null) contenedorBarra.SetActive(false);
        return base.RetirarItem();
    }

    // No se hacen mezclas dentro del reactor, solo en mesas
    public override bool IntentarCombinarConItem(ref ItemRecogible itemEntrante) => false;

    protected override void ActualizarIndicador()
    {
        if (matrazEnReactor == null)
        {
            base.ActualizarIndicador();
            return;
        }

        if (matrazEnReactor.TieneReactivo) PintarIndicador(colorProcesando);
        else if (matrazEnReactor.EstaCocinado) PintarIndicador(colorListo);
        else if (matrazEnReactor.EstaQuemado) PintarIndicador(colorQuemado);
    }

    private void PintarIndicador(Color color)
    {
        if (indicadorVisual == null) return;
        if (propBlock == null) propBlock = new MaterialPropertyBlock();

        indicadorVisual.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", color);
        indicadorVisual.SetPropertyBlock(propBlock);
    }

    private void ActualizarBarraUI(float porcentaje, Color color)
    {
        if (rellenoBarra == null) return;
        rellenoBarra.fillAmount = Mathf.Clamp01(porcentaje);
        rellenoBarra.color = color;
    }
}