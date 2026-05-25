using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class RoombaMinigame : MonoBehaviour
{
    [Header("Sprite del Roomba")]
    public Sprite spriteAcceso;
    public Sprite spriteSpento;

    [Header("Audio del Minigioco")]
    public AudioClip suonoAspirapolvere;
    public AudioClip suonoSpegnimento;

    [Header("Riferimento Manager (Obbligatorio)")]
    public InteractableTask taskManager;

    // --- LA TUA IDEA: DUE "CASSE ACUSTICHE" SEPARATE ---
    private AudioSource sourceLoop; // Cassa per il rumore continuo
    private AudioSource sourceSFX;  // Cassa per i suoni singoli
    
    private SpriteRenderer spriteRenderer;
    private bool giaSpento = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 1. Creiamo e impostiamo la cassa per il LOOP
        sourceLoop = gameObject.AddComponent<AudioSource>();
        sourceLoop.playOnAwake = false; 
        if (suonoAspirapolvere != null)
        {
            sourceLoop.clip = suonoAspirapolvere;
            sourceLoop.loop = true;
            sourceLoop.volume = 1f; 
        }

        // 2. Creiamo e impostiamo la cassa per gli SFX (suoni singoli)
        sourceSFX = gameObject.AddComponent<AudioSource>();
        sourceSFX.playOnAwake = false;
        sourceSFX.loop = false; // Questa non deve mai looppare
        sourceSFX.volume = 1f;
        
        spriteRenderer.sprite = spriteAcceso;
    }

    void OnEnable()
    {
        // Accendiamo solo la cassa del loop!
        if (!giaSpento && sourceLoop != null && sourceLoop.clip != null && !sourceLoop.isPlaying)
        {
            sourceLoop.Play();
        }
    }

      void OnDisable()
    {
        // Spegniamo SOLO l'aspirapolvere in loop. 
        // NON tocchiamo sourceSFX, così il "clack" può finire di suonare anche se lo script è spento!
        if (sourceLoop != null && sourceLoop.isPlaying) sourceLoop.Stop();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (giaSpento) return;

        giaSpento = true;
        spriteRenderer.sprite = spriteSpento;

        StartCoroutine(ConcludiMinigioco());
    }

 
    private IEnumerator ConcludiMinigioco()
    {
        // Piccola pausa solo per far capire che hai colpito il tasto
        yield return new WaitForSeconds(0.5f);
        
        // 1. Spegniamo il rumore continuo
        if (sourceLoop != null) sourceLoop.Stop();
        
        // 2. Spara il suono dello spegnimento
        if (suonoSpegnimento != null && sourceSFX != null)
        {
            sourceSFX.PlayOneShot(suonoSpegnimento);
        }

        // 3. ZERO ATTESA! Chiudiamo istantaneamente il minigioco.
        // Lo script si spegnerà, ma la "cassa acustica SFX" continuerà a suonare per i fatti suoi.
        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}