
using UnityEngine;

public enum TipoIngrediente
{
    MatrazVacio,
    LiquidoVerde,
    MatrazVerde
}

public class TipoComponente : MonoBehaviour
{
    [Header("Identificación del componente")]
    [SerializeField] private TipoIngrediente tipo;

    public TipoIngrediente Tipo => tipo;
}
