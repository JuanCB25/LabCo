using UnityEngine;
using TMPro;

public class WallClock : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private TMP_Text clockDisplay;

    private string previousTime = "";

    private void Update(){
        if (gameTimer == null || clockDisplay == null)
            return;

        int totalSeconds = Mathf.CeilToInt(gameTimer.RemainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = string.Format("{0:00}:{1:00}",minutes, seconds);

        if (formattedTime != previousTime){
            clockDisplay.text = formattedTime;
            previousTime = formattedTime;
        }
    }
}
