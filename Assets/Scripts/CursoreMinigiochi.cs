using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class MinigameCursor : MonoBehaviour
{
    public static MinigameCursor Instance;

    [Header("I Contenitori")]
    public GameObject contenitoreSingola;
    public GameObject contenitoreDoppia;

    [Header("Riferimenti Zampe (Per precisione Carta Igienica)")]
    public Transform zampaSx; // Trascina qui SpriteZampaSX
    public Transform zampaDx; // Trascina qui SpriteZampaDX

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
        // 1. Aspettiamo che il gioco sia davvero iniziato
        bool stiamoGiocando = GameManager.Instance != null && GameManager.Instance.inMinigioco;

        if (!stiamoGiocando)
        {
            SpegniTutto();
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;

        // 2. Accendiamo la grafica giusta
        if (tipoAttuale == TipoCursore.Singola && !contenitoreSingola.activeSelf)
        {
            contenitoreSingola.SetActive(true);
        }
        else if (tipoAttuale == TipoCursore.Doppia && !contenitoreDoppia.activeSelf)
        {
            contenitoreDoppia.SetActive(true);
        }

        // 3. SEGUE IL MOUSE (Con il fix per non farla sparire nel vuoto)
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;

        rb.MovePosition(worldPos);

        // 4. GESTIONE FISICA (Solo per il Roomba)
        if (tipoAttuale == TipoCursore.Singola && colliderSingola != null)
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
    }
}