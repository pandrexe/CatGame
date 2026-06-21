using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Impostazioni Tempo")]
    public float tempoMassimoSecondi = 60f; // Impostato di base a 60 secondi
    private float tempoGiocato = 0f;

    [Header("Interfaccia Grafica")]
    [Tooltip("Il testo principale del Timer (es. 60:00)")]
    public TextMeshProUGUI testoTimerUI;

    [Tooltip("Il testo che appare quando prendi il bonus (es. +5)")]
    public TextMeshProUGUI testoBonusUI;

    private float tempoRimanente;
    private bool timerAttivo = false;
    private Coroutine animazioneBonusCorrente;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        tempoRimanente = tempoMassimoSecondi;
        timerAttivo = true;

        if (testoBonusUI != null)
        {
            testoBonusUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (timerAttivo)
        {
            tempoRimanente -= Time.deltaTime;
            tempoGiocato += Time.deltaTime;

            if (testoTimerUI != null)
            {
                testoTimerUI.text = OttieniTempoFormattato();
            }

            if (tempoRimanente <= 0)
            {
                tempoRimanente = 0;
                timerAttivo = false;

                if (testoTimerUI != null) testoTimerUI.text = "00:00";

                TempoScaduto();
            }
        }
    }

    public void AggiungiTempo(float secondiExtra)
    {
        tempoRimanente += secondiExtra;
        Debug.Log($"+{secondiExtra} sec! Nuovo tempo: {OttieniTempoFormattato()}");

        if (testoBonusUI != null)
        {
            if (animazioneBonusCorrente != null)
            {
                StopCoroutine(animazioneBonusCorrente);
            }
            animazioneBonusCorrente = StartCoroutine(MostraTestoBonus(secondiExtra));
        }
    }

    private IEnumerator MostraTestoBonus(float secondi)
    {
        testoBonusUI.text = $"+{secondi}";
        testoBonusUI.gameObject.SetActive(true);

        Color coloreTesto = testoBonusUI.color;
        coloreTesto.a = 1f;
        testoBonusUI.color = coloreTesto;

        yield return new WaitForSeconds(1f);

        float durataSfumo = 0.5f;
        float timerSfumo = 0f;

        while (timerSfumo < durataSfumo)
        {
            timerSfumo += Time.deltaTime;
            coloreTesto.a = Mathf.Lerp(1f, 0f, timerSfumo / durataSfumo);
            testoBonusUI.color = coloreTesto;

            yield return null;
        }

        testoBonusUI.gameObject.SetActive(false);
    }

    private void TempoScaduto()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("TEMPO SCADUTO!");
        }
    }

    // --- MAGIA QUI: FORMATO SECONDI:CENTESIMI ---
    public string OttieniTempoFormattato()
    {
        int secondi = Mathf.FloorToInt(tempoRimanente);
        // Prende la parte decimale e la moltiplica per 100 per avere i centesimi
        int centesimi = Mathf.FloorToInt((tempoRimanente % 1) * 100);
        return string.Format("{0:00}:{1:00}", secondi, centesimi);
    }

    public void FermaTimer()
    {
        timerAttivo = false;
        Debug.Log("Timer bloccato al tempo: " + OttieniTempoFormattato());
    }

    // Aggiorniamo anche il formato del tempo giocato per la schermata di vittoria
    public string OttieniTempoGiocatoFormattato()
    {
        int secondi = Mathf.FloorToInt(tempoGiocato);
        int centesimi = Mathf.FloorToInt((tempoGiocato % 1) * 100);
        return string.Format("{0:00}:{1:00}", secondi, centesimi);
    }
}