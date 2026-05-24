using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MinigameCursor : MonoBehaviour
{
    public static MinigameCursor Instance;

    [Header("I Contenitori")]
    public GameObject contenitoreSingola;
    public GameObject contenitoreDoppia;
    public GameObject contenitoreColtello; // Il contenitore con zampa + coltello

    [Header("Riferimenti Zampe")]
    public Transform zampaSx; 
    public Transform zampaDx; 

    [Header("Riferimenti Coltello")]
    public Transform puntaColtello; // Il punto esatto da cui esce il sapone

    [Header("Fisica (Per il Roomba)")]
    public Collider2D colliderSingola;

    private TipoCursore tipoAttuale = TipoCursore.Nessuno;
    private Rigidbody2D rb;

    void Awake()
    {
        if (Instance == null) Instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        bool stiamoGiocando = GameManager.Instance != null && GameManager.Instance.inMinigioco;

        if (!stiamoGiocando)
        {
            SpegniTutto();
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;

        // Accendiamo il contenitore giusto in base al tipo
        if (tipoAttuale == TipoCursore.Singola)
        {
            // Se il coltello e assegnato, diamo priorita al coltello, altrimenti alla zampa singola
            if (contenitoreColtello != null)
            {
                if (!contenitoreColtello.activeSelf) contenitoreColtello.SetActive(true);
                if (contenitoreSingola != null && contenitoreSingola.activeSelf) contenitoreSingola.SetActive(false);
            }
            else if (contenitoreSingola != null && !contenitoreSingola.activeSelf)
            {
                contenitoreSingola.SetActive(true);
            }
        }
        else if (tipoAttuale == TipoCursore.Doppia && !contenitoreDoppia.activeSelf)
        {
            contenitoreDoppia.SetActive(true);
        }

        // Segue il mouse
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;

        rb.MovePosition(worldPos);

        // Fisica per il Roomba
        if (tipoAttuale == TipoCursore.Singola && colliderSingola != null && contenitoreSingola.activeSelf)
        {
            colliderSingola.enabled = Input.GetMouseButton(0);
        }
    }

    public void ImpostaCursore(TipoCursore nuovoTipo)
    {
        tipoAttuale = nuovoTipo;
    }

    private void SpegniTutto()
    {
        if (contenitoreSingola != null && contenitoreSingola.activeSelf) contenitoreSingola.SetActive(false);
        if (contenitoreDoppia != null && contenitoreDoppia.activeSelf) contenitoreDoppia.SetActive(false);
        if (contenitoreColtello != null && contenitoreColtello.activeSelf) contenitoreColtello.SetActive(false);
    }
}