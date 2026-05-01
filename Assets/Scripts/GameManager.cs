using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Riferimenti")]
    public GameObject gatto;
    public CinemachineCamera telecameraGatto;
    private CinemachineCamera telecameraMinigiocoAttuale;

    public bool inMinigioco = false;

    // Questa variabile ricorda QUALE task stiamo giocando
    private InteractableTask taskAttivo;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Abbiamo aggiunto il parametro "taskCheHaIniziato"
    public void IniziaMinigioco(CinemachineCamera telecameraMinigioco, InteractableTask taskCheHaIniziato)
    {
        inMinigioco = true;

        // Salviamo il task attivo!
        taskAttivo = taskCheHaIniziato;

        telecameraMinigiocoAttuale = telecameraMinigioco;
        telecameraMinigiocoAttuale.Priority = 20;
    }

    public void VinciMinigioco()
    {
        Debug.Log("Task Completato! Torniamo al gatto.");
        inMinigioco = false;

        if (telecameraMinigiocoAttuale != null)
        {
            telecameraMinigiocoAttuale.Priority = 0;
        }

        if (telecameraGatto != null)
        {
            telecameraGatto.Priority = 10;
        }

        // Se c'era un task attivo, gli diciamo di spegnersi e poi resettiamo la memoria
        if (taskAttivo != null)
        {
            taskAttivo.DisattivaNemico();
            taskAttivo = null;
        }
    }
}