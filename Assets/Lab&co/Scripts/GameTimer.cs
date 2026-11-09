
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    [Header("Configuracion del temporizador")]
    [SerializeField] private float durationSeconds = 180f; //establece la duración de la partida 3 min

    [Header("Eventos")]
    [SerializeField] private UnityEvent onTimerFinished;

    private float remainingTime;
    private bool isRunning;

    public float RemainingTime => remainingTime;
    public bool IsRunning => isRunning;

    private void Start()
    {
        StartTimer();
    }

    private void Update() //actualiza el reloj constantemente
    {
        if (!isRunning)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;

            Debug.Log("¡La policia ha llegado! Fin de la partida.");

            onTimerFinished?.Invoke();
        }
    }

    public void StartTimer() //inicia la cuenta atrás
    {
        remainingTime = durationSeconds;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}

