using UnityEngine;
using System.Collections;

public class WheelTask : MonoBehaviour
{
    public InteractableTask taskManager;

    public Collider2D[] bulloni;

    public AudioClip suonoBulloneCaduto;

    private int bulloniRimossi = 0;
    private AudioSource audioSource;
    private bool taskFinito = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        bulloniRimossi = 0;
        taskFinito = false;
        
        foreach (Collider2D bullone in bulloni)
        {
            if (bullone != null)
            {
                bullone.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (taskFinito) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.contenitoreSingola != null)
            {
                // Prendiamo le coordinate della zampa
                Vector2 posZampa = MinigameCursor.Instance.contenitoreSingola.transform.position;

                for (int i = 0; i < bulloni.Length; i++)
                {
                    // Controlliamo che il bullone esista, sia ancora ACCESO e la zampa ci stia cliccando sopra
                    if (bulloni[i] != null && bulloni[i].gameObject.activeSelf && bulloni[i].OverlapPoint(posZampa))
                    {
                        // 1. Spegniamo letteralmente l'oggetto visivo del bullone
                        bulloni[i].gameObject.SetActive(false);

                        // 2. Facciamo partire il suono metallico
                        if (suonoBulloneCaduto != null)
                        {
                            audioSource.PlayOneShot(suonoBulloneCaduto);
                        }

                        // 3. Aggiorniamo il punteggio
                        bulloniRimossi++;
                        Debug.Log($"Bullone rimosso! Progressi: {bulloniRimossi}/{bulloni.Length}");

                        // 4. Controllo vittoria: li abbiamo tolti tutti?
                        if (bulloniRimossi >= bulloni.Length)
                        {
                            FineTask();
                        }

                        // Abbiamo cliccato un bullone, fermiamo il ciclo per questo singolo click
                        break;
                    }
                }
            }
        }
    }

    private void FineTask()
    {
        taskFinito = true;
        Debug.Log("TUTTI I BULLONI RIMOSSI! VITTORIA!");
  
        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        // Mezzo secondo di pausa per far finire il suono dell'ultimo tonfo
        yield return new WaitForSeconds(0.5f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}