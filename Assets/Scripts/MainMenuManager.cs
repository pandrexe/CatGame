using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Interfaccia")]
    [Tooltip("Trascina qui l'oggetto PannelloComandi")]
    public GameObject pannelloComandi;

    void Start()
    {
        // Ci assicuriamo che all'avvio il pannello dei comandi sia invisibile
        if (pannelloComandi != null)
        {
            pannelloComandi.SetActive(false);
        }
    }

    public void IniziaGioco()
    {
        // Ricordati sempre di rimettere il nome esatto della tua scena!
        SceneManager.LoadScene("Gioco");
    }

    public void EsciDalGioco()
    {
        Debug.Log("Uscita dal gioco confermata!");
        Application.Quit();
    }

    // --- NUOVE FUNZIONI PER I COMANDI ---

    public void ApriComandi()
    {
        if (pannelloComandi != null) pannelloComandi.SetActive(true);
    }

    public void ChiudiComandi()
    {
        if (pannelloComandi != null) pannelloComandi.SetActive(false);
    }
}