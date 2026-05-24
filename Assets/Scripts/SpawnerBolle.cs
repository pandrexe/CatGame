using UnityEngine;

public class SpawnerBolle : MonoBehaviour
{
    [Header("Lista delle Bolle")]
    // Questo array (le parentesi quadre) ci permette di inserire quanti prefab vogliamo!
    public GameObject[] prefabsBolle;

    [Header("Impostazioni Timer")]
    public float tempoTraSpawn = 2f;

    private float timer = 0f;

    void Update()
    {
        // Evitiamo di far uscire bolle se siamo in un minigioco
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnaBollaRandom();
            timer = tempoTraSpawn; // Resetta il timer per la prossima bolla
        }
    }

    void SpawnaBollaRandom()
    {
        // Controllo di sicurezza: se non hai inserito bolle nell'Inspector, non fa nulla
        if (prefabsBolle.Length == 0) return;

        // Sceglie un numero a caso tra 0 e la quantità di bolle che hai inserito
        int indiceRandom = Random.Range(0, prefabsBolle.Length);

        // Instanzia (crea) la bolla scelta esattamente nella posizione di questo Spawner
        Instantiate(prefabsBolle[indiceRandom], transform.position, Quaternion.identity);
    }
}