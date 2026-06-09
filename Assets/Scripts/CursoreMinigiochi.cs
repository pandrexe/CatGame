using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MinigameCursor : MonoBehaviour
{
    public static MinigameCursor Instance;

    [Header("I Contenitori")]
    public GameObject contenitoreSingola;
    public GameObject contenitoreDoppia;
    public GameObject contenitoreColtello;
    public GameObject contenitoreDoppiaCentrale; // <-- NUOVO SLOT

    [Header("Riferimenti Zampe")]
    public Transform zampaSx;
    public Transform zampaDx;

    [Header("Riferimenti Coltello")]
    public Transform puntaColtello;

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

        // --- LA GESTIONE PULITA DEI CURSORI ---
        if (tipoAttuale == TipoCursore.Singola)
        {
            if (contenitoreSingola != null && !contenitoreSingola.activeSelf) contenitoreSingola.SetActive(true);
            if (contenitoreColtello != null && contenitoreColtello.activeSelf) contenitoreColtello.SetActive(false);
            if (contenitoreDoppia != null && contenitoreDoppia.activeSelf) contenitoreDoppia.SetActive(false);
            if (contenitoreDoppiaCentrale != null && contenitoreDoppiaCentrale.activeSelf) contenitoreDoppiaCentrale.SetActive(false);
        }
        else if (tipoAttuale == TipoCursore.SingolaColtello)
        {
            if (contenitoreColtello != null && !contenitoreColtello.activeSelf) contenitoreColtello.SetActive(true);
            if (contenitoreSingola != null && contenitoreSingola.activeSelf) contenitoreSingola.SetActive(false);
            if (contenitoreDoppia != null && contenitoreDoppia.activeSelf) contenitoreDoppia.SetActive(false);
            if (contenitoreDoppiaCentrale != null && contenitoreDoppiaCentrale.activeSelf) contenitoreDoppiaCentrale.SetActive(false);
        }
        else if (tipoAttuale == TipoCursore.Doppia)
        {
            if (contenitoreDoppia != null && !contenitoreDoppia.activeSelf) contenitoreDoppia.SetActive(true);
            if (contenitoreSingola != null && contenitoreSingola.activeSelf) contenitoreSingola.SetActive(false);
            if (contenitoreColtello != null && contenitoreColtello.activeSelf) contenitoreColtello.SetActive(false);
            if (contenitoreDoppiaCentrale != null && contenitoreDoppiaCentrale.activeSelf) contenitoreDoppiaCentrale.SetActive(false);
        }
        else if (tipoAttuale == TipoCursore.DoppiaCentrale) // --- IL NUOVO STATO ---
        {
            if (contenitoreDoppiaCentrale != null && !contenitoreDoppiaCentrale.activeSelf) contenitoreDoppiaCentrale.SetActive(true);
            if (contenitoreSingola != null && contenitoreSingola.activeSelf) contenitoreSingola.SetActive(false);
            if (contenitoreColtello != null && contenitoreColtello.activeSelf) contenitoreColtello.SetActive(false);
            if (contenitoreDoppia != null && contenitoreDoppia.activeSelf) contenitoreDoppia.SetActive(false);
        }

        // Segue il mouse
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;

        rb.MovePosition(worldPos);

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
        if (contenitoreDoppiaCentrale != null && contenitoreDoppiaCentrale.activeSelf) contenitoreDoppiaCentrale.SetActive(false);
    }
}