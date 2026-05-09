/*
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progressione Roguelite")]
    public int viteMassime = 7;
    public int viteAttuali;
    public int livelloCorrente = 1; // 1 = Facile, 2 = Medio, 3 = Difficile

    [Header("Riferimenti Telecamere (Si riassegnano ogni scena)")]
    public GameObject gatto;
    public CinemachineCamera telecameraGatto;
    private CinemachineCamera telecameraMinigiocoAttuale;

    public bool inMinigioco = false;
    private InteractableTask taskAttivo;
    private bool gattoInvulnerabile = false;
    public float durataInvulnerabilita = 2f; // Durata I-Frames
    private SpriteRenderer spriteGatto;

    void Awake()
    {
        // PATTERN SINGLETON IMMORTALE
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Questo oggetto sopravvive ai cambi di scena!
            viteAttuali = viteMassime; // Inizializza le vite solo alla primissima partita
        }
        else
        {
            // Se ricaricando la scena si crea un SECONDO GameManager, distruggilo!
            Destroy(gameObject);
        }
    }

    public void RegistraPlayer(GameObject playerObject)
    {
        gatto = playerObject;
        spriteGatto = gatto.GetComponent<SpriteRenderer>();

        // --- RESET TELECAMERE AL CARICAMENTO ---
        if (telecameraGatto != null)
        {
            telecameraGatto.Follow = gatto.transform;
            telecameraGatto.Priority = 10; // Priorità base del gatto
        }

        // Reset della variabile minigioco per sicurezza estrema
        inMinigioco = false;
    }

    public void IniziaMinigioco(CinemachineCamera telecameraMinigioco, InteractableTask taskCheHaIniziato)
    {
        inMinigioco = true;
        taskAttivo = taskCheHaIniziato;
        telecameraMinigiocoAttuale = telecameraMinigioco;
        telecameraMinigiocoAttuale.Priority = 20;
    }

    public void VinciMinigioco()
    {
        inMinigioco = false;
        if (telecameraMinigiocoAttuale != null) telecameraMinigiocoAttuale.Priority = 0;
        if (telecameraGatto != null) telecameraGatto.Priority = 10;

        if (taskAttivo != null)
        {
            taskAttivo.CompletaTask();
            taskAttivo = null;
        }
    }


    public void PerdiVita()
    {
        // Se il gatto è già stato colpito di recente, ignora il danno!
        if (gattoInvulnerabile) return;

        viteAttuali--;
        Debug.Log($"Ahi! Il gatto è stato colpito. Vite rimaste: {viteAttuali}");

        if (viteAttuali <= 0)
        {
            GameOver("Hai finito le tue 7 vite!");
        }
        else
        {
            StartCoroutine(GestisciInvulnerabilita());
        }
    }

    private System.Collections.IEnumerator GestisciInvulnerabilita()
    {
        gattoInvulnerabile = true;

        // Se per caso non abbiamo lo sprite (errore), usciamo subito
        if (spriteGatto == null)
        {
            yield return new WaitForSeconds(durataInvulnerabilita);
            gattoInvulnerabile = false;
            yield break;
        }

        Color coloreOriginale = spriteGatto.color;
        // Creiamo una versione semi-trasparente del colore
        Color coloreTrasparente = coloreOriginale;
        coloreTrasparente.a = 0.3f; // 30% opacità

        float tempoPassato = 0f;
        float velocitaLampeggio = 0.15f; // Quanto dura un singolo flash

        // Finchè non scade il tempo totale... lampeggia!
        while (tempoPassato < durataInvulnerabilita)
        {
            // Alterna tra trasparente e originale
            if (spriteGatto.color == coloreOriginale)
                spriteGatto.color = coloreTrasparente;
            else
                spriteGatto.color = coloreOriginale;

            yield return new WaitForSeconds(velocitaLampeggio);
            tempoPassato += velocitaLampeggio;
        }

        // Assicuriamoci di rimettere il colore originale alla fine!
        spriteGatto.color = coloreOriginale;
        gattoInvulnerabile = false;
    }

    public void FallisciTask()
    {
        GameOver("Hai fallito un task letale! Ricominci da capo.");
    }

    private void GameOver(string motivazione)
    {
        Debug.Log($"GAME OVER: {motivazione}");

        // --- IL FIX FONDAMENTALE ---
        inMinigioco = false;      // Sblocca i comandi del gatto
        taskAttivo = null;        // Pulisce il riferimento al vecchio task
        Time.timeScale = 1f;      // Assicura che il tempo non sia fermo

        // Resetta la progressione roguelite
        livelloCorrente = 1;
        viteAttuali = viteMassime;

        // Ricarica la scena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VittoriaLivello()
    {
        if (livelloCorrente < 3)
        {
            livelloCorrente++;
            Debug.Log($"Livello {livelloCorrente - 1} completato! Passiamo al livello {livelloCorrente}");
            // Ricarica la stessa scena, ma il TaskManager leggerà il nuovo "livelloCorrente"
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log("HAI COMPLETATO TUTTO IL GIOCO! SEI IL RE DEI GATTI!");
            // Qui in futuro caricherai una scena di Crediti o Vittoria
        }
    }
}
*/