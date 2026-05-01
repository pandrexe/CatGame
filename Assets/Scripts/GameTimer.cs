using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("Impostazioni Tempo")]
    public float tempoMassimoSecondi = 300f; // 5 minuti

    private float tempoRimanente;
    private bool timerAttivo = false;

    void Start()
    {
        tempoRimanente = tempoMassimoSecondi;
        timerAttivo = true;
    }

    void Update()
    {
        if (timerAttivo)
        {
            // Time.deltaTime tiene già conto del Time.timeScale = 0 della pausa!
            tempoRimanente -= Time.deltaTime;

            if (tempoRimanente <= 0)
            {
                tempoRimanente = 0;
                timerAttivo = false;
                TempoScaduto();
            }
        }
    }

    private void TempoScaduto()
    {
        Debug.Log("TEMPO SCADUTO! Hai Perso!");
        // Qui in futuro chiameremo GameManager.Instance.Sconfitta()
    }

    // Funzione utile se in futuro vorrai mostrare il tempo a schermo (sulla UI)
    public string OttieniTempoFormattato()
    {
        int minuti = Mathf.FloorToInt(tempoRimanente / 60);
        int secondi = Mathf.FloorToInt(tempoRimanente % 60);
        return string.Format("{0:00}:{1:00}", minuti, secondi);
    }
}