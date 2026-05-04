using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("UI Interazione")]
    public GameObject testoInterazioneUI;

    protected bool gattoVicino = false;
    protected bool puoInteragire = true; 

    protected virtual void Start()
    {
        if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
    }

    protected virtual void Update()
    {
        if (gattoVicino && puoInteragire && Input.GetKeyDown(KeyCode.E))
        {
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
            EseguiInterazione();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puoInteragire)
        {
            gattoVicino = true;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gattoVicino = false;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
        }
    }

    // Il metodo che i figli dovranno personalizzare
    protected abstract void EseguiInterazione();
}