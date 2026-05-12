using UnityEngine;

public class AlarmTask : MonoBehaviour
{
    private bool taskFinito = false;

    void Update()
    {
        // Controlliamo che il minigioco sia effettivamente iniziato e non sia già finito
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        // Se il giocatore preme 'E', vince istantaneamente
        if (Input.GetKeyDown(KeyCode.E))
        {
            FineTask();
        }
    }

    void FineTask()
    {
        taskFinito = true;

        // Chiama il metodo standard del GameManager per chiudere la visuale e tornare al gatto
        GameManager.Instance.VinciMinigioco();
    }
}