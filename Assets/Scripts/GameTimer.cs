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
            tempoRimanente -= Time.deltaTime;

            if (tempoRimanente <= 0)
            {
                tempoRimanente = 0;
                timerAttivo = false;
                TempoScaduto();
            }
        }
    }

    // --- NUOVA FUNZIONE PER I POWERUP ---
    public void AggiungiTempo(float secondiExtra)
    {
        tempoRimanente += secondiExtra;
        Debug.Log($"+{secondiExtra} sec! Nuovo tempo: {OttieniTempoFormattato()}");
    }

    private void TempoScaduto()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("TEMPO SCADUTO!");
        }
    }

    public string OttieniTempoFormattato()
    {
        int minuti = Mathf.FloorToInt(tempoRimanente / 60);
        int secondi = Mathf.FloorToInt(tempoRimanente % 60);
        return string.Format("{0:00}:{1:00}", minuti, secondi);
    }
}