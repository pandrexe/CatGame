using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events; 

public class InteractableTask : MonoBehaviour
{
    [Header("Identità Task")]
    public TaskType tipoDiTask;

    [Header("Impostazioni Task")]
    public CinemachineCamera telecameraDelMinigioco;

    [Header("UI Interazione")]
    public GameObject testoInterazioneUI;

    [Header("Conseguenze Vittoria")]
    public UnityEvent azioniAllaVittoria; 
    private bool gattoVicino = false;
    private bool taskGiocato = false;

    void Start()
    {
        if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
    }

    void Update()
    {
        if (gattoVicino && !taskGiocato && Input.GetKeyDown(KeyCode.E))
        {
            taskGiocato = true;

            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);

            GameManager.Instance.IniziaMinigioco(telecameraDelMinigioco, this);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !taskGiocato)
        {
            gattoVicino = true;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gattoVicino = false;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
        }
    }

    // Il GameManager ora chiamerà questa funzione, che è totalmente generica
    public void CompletaTask()
    {
        // 1. Diciamo al TaskManager di spuntare la lista
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.SegnalaTaskCompletato(tipoDiTask);
        }

        azioniAllaVittoria?.Invoke();
    }
}