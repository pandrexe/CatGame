using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CatPawCursor : MonoBehaviour
{
    [Header("Grafica Zampa")]
    public Sprite zampaAperta;
    public Sprite zampaChiusa; // A pugno, o per colpire

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        
        rb.bodyType = RigidbodyType2D.Kinematic; 
        col.enabled = false; 
        sr.enabled = false; // Invisibile all'inizio
    }

    void Update()
    {
        // Se NON siamo nel minigioco, nascondi la zampa e rimetti la freccetta del PC
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco)
        {
            sr.enabled = false;
            col.enabled = false;
            Cursor.visible = true; 
            return;
        }

        // --- SIAMO NEL MINIGIOCO! ---
        sr.enabled = true;     // Mostra la zampa
        Cursor.visible = false; // Nascondi la freccetta di Windows

        // Segui SEMPRE il mouse (anche se non clicchi)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.MovePosition(new Vector2(mousePos.x, mousePos.y));

        // Gestione del Click ("Lo Schiaffo")
        if (Input.GetMouseButton(0))
        {
            col.enabled = true;         // Attiva la collisione
            sr.sprite = zampaChiusa;    // Cambia immagine
        }
        else
        {
            col.enabled = false;        // Disattiva collisione (il cursore "passa attraverso")
            sr.sprite = zampaAperta;    // Rilassa la zampa
        }
    }
}