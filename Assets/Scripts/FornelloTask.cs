using UnityEngine;
using System.Collections;

public class StoveSliderTask : MonoBehaviour
{
    [Header("Riferimenti Manager")]
    public InteractableTask taskManager; // Obbligatorio!

    [Header("Riferimenti Slider")]
    [Tooltip("Il Collider2D dello Slider da trascinare col mouse")]
    public Collider2D sliderCollider;
    [Tooltip("L'oggetto Traguardo a SINISTRA")]
    public Transform traguardoTransform;

    [Header("Oggetti Fornello (Hierarchy)")]
    public GameObject fornoAccesoObj;
    public GameObject fornoSpentoObj;

    [Header("Audio")]
    public AudioClip suonoSpegnimento;

    private AudioSource audioSource;
    private bool taskFinito = false;
    private bool staTrascinando = false;
    private float offsetX;

    // Blocchi di posizione
    private float startX;
    private float fixedY;
    private float fixedZ;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (sliderCollider != null)
        {
            startX = sliderCollider.transform.position.x;
            fixedY = sliderCollider.transform.position.y;
            fixedZ = sliderCollider.transform.position.z;
        }
    }

    void OnEnable()
    {
        // Reset totale del minigioco se si esce e rientra
        taskFinito = false;
        staTrascinando = false;

        if (sliderCollider != null)
        {
            sliderCollider.transform.position = new Vector3(startX, fixedY, fixedZ);
        }

        // All'inizio il forno deve essere acceso (spento invisibile)
        if (fornoAccesoObj != null) fornoAccesoObj.SetActive(true);
        if (fornoSpentoObj != null) fornoSpentoObj.SetActive(false);
    }

    void Update()
    {
        if (taskFinito) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 1. CLICK E PRESA
        if (Input.GetMouseButtonDown(0))
        {
            if (sliderCollider != null && sliderCollider.OverlapPoint(mouseWorldPos))
            {
                staTrascinando = true;
                offsetX = sliderCollider.transform.position.x - mouseWorldPos.x;
            }
        }

        // 2. TRASCINAMENTO
        if (staTrascinando && Input.GetMouseButton(0))
        {
            float nuovaX = mouseWorldPos.x + offsetX;

            // IL MURO MATEMATICO: La X non può mai essere maggiore della X iniziale (così non va verso destra)
            nuovaX = Mathf.Min(nuovaX, startX);

            sliderCollider.transform.position = new Vector3(nuovaX, fixedY, fixedZ);

            // 3. CONTROLLO TRAGUARDO (<= perché si muove verso sinistra)
            if (traguardoTransform != null)
            {
                if (sliderCollider.transform.position.x <= traguardoTransform.position.x)
                {
                    SpegniFornello();
                }
            }
        }

        // 3. RILASCIO
        if (Input.GetMouseButtonUp(0))
        {
            staTrascinando = false;
        }
    }

    private void SpegniFornello()
    {
        taskFinito = true;
        staTrascinando = false;

        // TOGGLE DEGLI OGGETTI: Spegni quello acceso, accendi quello spento!
        if (fornoAccesoObj != null) fornoAccesoObj.SetActive(false);
        if (fornoSpentoObj != null) fornoSpentoObj.SetActive(true);

        if (suonoSpegnimento != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoSpegnimento);
        }

        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        // Aspetta un istante per far vedere il forno spento
        yield return new WaitForSeconds(1f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}