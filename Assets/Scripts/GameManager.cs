using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progressione")]
    public int viteMassime = 7;
    public int viteAttuali;

    [Header("Riferimenti Telecamere")]
    public GameObject gatto;
    public CinemachineCamera telecameraGatto;
    private CinemachineCamera telecameraMinigiocoAttuale;

    public bool inMinigioco = false;
    private InteractableTask taskAttivo;

    private bool gattoInvulnerabile = false;
    public float durataInvulnerabilita = 2f;
    private SpriteRenderer spriteGatto;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            viteAttuali = viteMassime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegistraPlayer(GameObject playerObject)
    {
        gatto = playerObject;
        spriteGatto = gatto.GetComponent<SpriteRenderer>();

        if (telecameraGatto != null)
        {
            telecameraGatto.Follow = gatto.transform;
            telecameraGatto.Priority = 10;
        }
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
        if (gattoInvulnerabile) return;

        viteAttuali--;
        Debug.Log($"Vite rimaste: {viteAttuali}");

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
        if (spriteGatto == null)
        {
            yield return new WaitForSeconds(durataInvulnerabilita);
            gattoInvulnerabile = false;
            yield break;
        }

        Color coloreOriginale = spriteGatto.color;
        Color coloreTrasparente = coloreOriginale;
        coloreTrasparente.a = 0.3f;

        float tempoPassato = 0f;
        float velocitaLampeggio = 0.15f;

        while (tempoPassato < durataInvulnerabilita)
        {
            spriteGatto.color = (spriteGatto.color == coloreOriginale) ? coloreTrasparente : coloreOriginale;
            yield return new WaitForSeconds(velocitaLampeggio);
            tempoPassato += velocitaLampeggio;
        }

        spriteGatto.color = coloreOriginale;
        gattoInvulnerabile = false;
    }

    public void FallisciTask()
    {
        GameOver("Task fallito! Ricominci da capo.");
    }

    public void GameOver(string motivazione) // Reso public per poter essere chiamato dal GameTimer
    {
        Debug.Log($"GAME OVER: {motivazione}");

        inMinigioco = false;
        taskAttivo = null;
        Time.timeScale = 1f;

        viteAttuali = viteMassime;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VittoriaGioco()
    {
        inMinigioco = false;
        Debug.Log("HAI COMPLETATO TUTTI I TASK! VITTORIA!");
        // SceneManager.LoadScene("ScenaVittoria");
    }

}