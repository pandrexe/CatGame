using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI Instance;

    [Header("Pannelli (Oggetti UI)")]
    public GameObject pannelloTempoScaduto;
    public GameObject pannelloVittoria;
    public GameObject pannelloTaskMancanti;
    public GameObject pannelloViteFinite; // <-- NUOVO PANNELLO

    [Header("Testi Dinamici")]
    public TextMeshProUGUI testoTempoImpiegato;
    public TextMeshProUGUI testoListaMancanti;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (pannelloTempoScaduto) pannelloTempoScaduto.SetActive(false);
        if (pannelloVittoria) pannelloVittoria.SetActive(false);
        if (pannelloTaskMancanti) pannelloTaskMancanti.SetActive(false);
        if (pannelloViteFinite) pannelloViteFinite.SetActive(false); // Spegniamo anche questo
    }

    // --- LE FUNZIONI PER APRIRE I PANNELLI ---

    public void MostraTempoScaduto()
    {
        Time.timeScale = 0f;
        if (pannelloTempoScaduto) pannelloTempoScaduto.SetActive(true);
    }

    // <-- NUOVA FUNZIONE -->
    public void MostraViteFinite()
    {
        Time.timeScale = 0f;
        if (pannelloViteFinite) pannelloViteFinite.SetActive(true);
    }

    public void MostraVittoria(string tempo)
    {
        Time.timeScale = 0f;
        if (testoTempoImpiegato != null) testoTempoImpiegato.text = $"{tempo} MINUTES!";
        if (pannelloVittoria) pannelloVittoria.SetActive(true);
    }

    public void MostraTaskMancanti(string lista)
    {
        Time.timeScale = 0f;
        // Ho rimosso "Non hai fatto:\n\n" dal codice visto che nella tua UI grafica hai già scritto "YOU MISSED SOMETHING TO DO!"
        if (testoListaMancanti != null) testoListaMancanti.text = lista;
        if (pannelloTaskMancanti) pannelloTaskMancanti.SetActive(true);
    }

    public void Ricomincia()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TornaAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}