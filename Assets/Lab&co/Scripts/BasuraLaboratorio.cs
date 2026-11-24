using UnityEngine;

public class BasuraLaboratorio : MesaLaboratorio
{
    [Header("Configuración de Basura")]
    [Tooltip("Si está activo, permite vaciar también matraces con reactivo o cocinados, no solo quemados")]
    [SerializeField] private bool permitirVaciarCualquierContenido = true;

    [Tooltip("Si está activo, destruye reactivos verdes sueltos si se tiran aquí")]
    [SerializeField] private bool destruirReactivosSueltos = true;

    [Header("Feedback Opcional")]
    [SerializeField] private ParticleSystem particulasDescarte;
    [SerializeField] private AudioSource sonidoBasura;

    // La basura nunca se queda ocupada con un objeto encima
    public override bool EstaOcupada => false;
    public override bool PuedeRecibirItem() => true;
    public override ItemRecogible RetirarItem() => null;
    public override bool IntentarCombinarConItem(ref ItemRecogible itemEntrante) => false;

    public override bool ColocarItem(ItemRecogible item)
    {
        if (item == null) return false;

        // CASO 1: El jugador trae un Matraz -> Se vacía y el jugador lo conserva limpio en sus manos
        if (item.Tipo == ItemRecogible.TipoItem.Matraz)
        {
            MatrazQuimico matraz = item.GetComponent<MatrazQuimico>();
            if (matraz != null && !matraz.EstaVacio)
            {
                if (permitirVaciarCualquierContenido || matraz.EstaQuemado)
                {
                    matraz.Vaciar();
                    ReproducirFeedback();
                }
            }

            // Devolvemos false para que InteraccionJugador NO suelte el matraz de las manos
            return false;
        }

        // CASO 2: El jugador trae un Reactivo suelto -> Se elimina por completo
        if (destruirReactivosSueltos && item.Tipo == ItemRecogible.TipoItem.ReactivoVerde)
        {
            ReproducirFeedback();
            Destroy(item.gameObject);

            // Devolvemos true para que InteraccionJugador libere las manos del jugador
            return true;
        }

        return false;
    }

    private void ReproducirFeedback()
    {
        if (particulasDescarte != null) particulasDescarte.Play();
        if (sonidoBasura != null) sonidoBasura.Play();
    }

    protected override void ActualizarIndicador() { }
}