using UnityEngine;
using TMPro;
using System.Collections; // Serve per le Coroutine

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Impostazioni Tempo")]
    public float tempoMassimoSecondi = 300f;

    [Header("Interfaccia Grafica")]
    [Tooltip("Il testo principale del Timer (es. 05:00)")]
    public TextMeshProUGUI testoTimerUI;

    [Tooltip("Il testo che appare quando prendi il bonus (es. +5)")]
    public TextMeshProUGUI testoBonusUI; // <-- NUOVO RIFERIMENTO

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

        // Assicuriamoci che il testo bonus sia invisibile all'inizio della run
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

        // --- ATTIVAZIONE GRAFICA DEL BONUS ---
        if (testoBonusUI != null)
        {
            // Se c'è già un'animazione in corso (es. fa due task velocissimi), la fermiamo per farla ripartire
            if (animazioneBonusCorrente != null)
            {
                StopCoroutine(animazioneBonusCorrente);
            }
            animazioneBonusCorrente = StartCoroutine(MostraTestoBonus(secondiExtra));
        }
    }

    private IEnumerator MostraTestoBonus(float secondi)
    {
        // 1. Scrive il testo (es. "+5") e lo accende
        testoBonusUI.text = $"+{secondi}";
        testoBonusUI.gameObject.SetActive(true);

        // 2. Assicura che l'opacità (Alpha) sia al 100%
        Color coloreTesto = testoBonusUI.color;
        coloreTesto.a = 1f;
        testoBonusUI.color = coloreTesto;

        // 3. Aspetta 1 secondo lasciandolo ben visibile
        yield return new WaitForSeconds(1f);

        // 4. Sfuma verso il trasparente per mezzo secondo
        float durataSfumo = 0.5f;
        float timerSfumo = 0f;

        while (timerSfumo < durataSfumo)
        {
            timerSfumo += Time.deltaTime;
            // Mathf.Lerp abbassa gradualmente il valore da 1 (visibile) a 0 (invisibile)
            coloreTesto.a = Mathf.Lerp(1f, 0f, timerSfumo / durataSfumo);
            testoBonusUI.color = coloreTesto;

            yield return null; // Aspetta il frame successivo
        }

        // 5. Lo spegne del tutto fino al prossimo bonus
        testoBonusUI.gameObject.SetActive(false);
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