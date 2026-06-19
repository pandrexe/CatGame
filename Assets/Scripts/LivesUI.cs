using UnityEngine;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour
{
    public static LivesUI Instance;

    // Trascina qui i cuori in ordine rigoroso: 
    // Elemento 0 = Cuore tutto a SINISTRA
    // Elemento 6 = Cuore tutto a DESTRA
    public List<GameObject> cuoriUI = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        AggiornaCuoriGrafica();
    }

    public void AggiornaCuoriGrafica()
    {
        if (GameManager.Instance == null) return;

        int viteRimaste = GameManager.Instance.viteAttuali;
        int totaleCuori = cuoriUI.Count;
        
        // Calcoliamo quante vite abbiamo perso
        int vitePerse = totaleCuori - viteRimaste;

        // Spegniamo da SINISTRA verso DESTRA
        for (int i = 0; i < totaleCuori; i++)
        {
            if (cuoriUI[i] != null)
            {
                // Se l'indice del cuore è minore del numero di vite perse, lo spegniamo.
                // Es: Persa 1 vita -> vitePerse = 1. L'indice 0 (il primo a sinistra) si spegne.
                if (i < vitePerse)
                {
                    cuoriUI[i].SetActive(false);
                }
                else
                {
                    cuoriUI[i].SetActive(true);
                }
            }
        }
    }
}