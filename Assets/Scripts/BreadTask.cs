using UnityEngine;
using System.Collections.Generic;

public class BreadTask : MonoBehaviour
{
    [Header("Impostazioni Spalmatura")]
    public Transform[] zoneToast; // Trascina qui tutti i punti vuoti creati sul toast
    public GameObject prefabSapone; // Lo sprite della macchia di sapone da far apparire
    public int zonePerVincere = 15; // Quante zone devi coprire per completare il task
    public float raggioSpalmatura = 0.5f; // Quanto deve essere vicino il coltello per spalmare

    [Header("Coltello")]
    public Transform coltelloGatto; // L'oggetto coltello che segue il mouse

    private HashSet<Transform> zoneCoperte = new HashSet<Transform>();
    private bool taskFinito = false;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        // Ottieni la posizione del mouse nel mondo 2D
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Il coltello segue sempre il mouse durante il minigioco
        if (coltelloGatto != null)
        {
            coltelloGatto.position = mousePos;
        }

        // Se tieni premuto il clic sinistro, stai spalmando
        if (Input.GetMouseButton(0))
        {
            SpalmaSapone(mousePos);
        }
    }

    private void SpalmaSapone(Vector3 posizioneMouse)
    {
        // Controlla tutte le zone del toast
        foreach (Transform zona in zoneToast)
        {
            // Se non è ancora stata coperta
            if (!zoneCoperte.Contains(zona))
            {
                // Se il mouse e abbastanza vicino a questa zona
                if (Vector2.Distance(posizioneMouse, zona.position) < raggioSpalmatura)
                {
                    // Segnala la zona come coperta
                    zoneCoperte.Add(zona);

                    // Crea la macchia di sapone esattamente in quel punto
                    if (prefabSapone != null)
                    {
                        Instantiate(prefabSapone, zona.position, Quaternion.identity, zona);
                    }

                    // Controlla la vittoria
                    if (zoneCoperte.Count >= zonePerVincere)
                    {
                        FineTask();
                    }
                }
            }
        }
    }

    private void FineTask()
    {
        taskFinito = true;
        Debug.Log("Toast al detersivo completato");
        GameManager.Instance.VinciMinigioco();
    }
}
