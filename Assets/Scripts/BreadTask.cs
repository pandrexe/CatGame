using UnityEngine;

public class BreadTask : MonoBehaviour 
{
    public Color coloreCrema = Color.green; 
    public int raggioPennello = 15; 
    public float percentualePerVincere = 0.85f; 

    private SpriteRenderer toastRenderer;
    private Texture2D textureDinamica;
    private Color32[] pixelOriginali;
    private bool[] pixelColorati;
    private int totaliPixelDaColorare = 0;
    private int pixelColoratiCorrenti = 0;
    private bool taskFinito = false;

    void Start()
    {
        toastRenderer = GetComponent<SpriteRenderer>();
        Texture2D textureOriginale = toastRenderer.sprite.texture;
        
        textureDinamica = new Texture2D(textureOriginale.width, textureOriginale.height, TextureFormat.RGBA32, false);
        pixelOriginali = textureOriginale.GetPixels32();
        
        int totalePixel = pixelOriginali.Length;
        pixelColorati = new bool[totalePixel];
        Color32[] pixelLavoro = new Color32[totalePixel];

        for (int i = 0; i < totalePixel; i++)
        {
            pixelLavoro[i] = pixelOriginali[i];
            // Contiamo solo i pixel visibili, ignorando il fondo trasparente
            if (pixelOriginali[i].a > 5)
            {
                totaliPixelDaColorare = totaliPixelDaColorare + 1;
            }
        }

        textureDinamica.SetPixels32(pixelLavoro);
        textureDinamica.Apply();

        // Ricreiamo lo sprite forzando il centro corretto
        Sprite nuovoSprite = Sprite.Create(textureDinamica, new Rect(0, 0, textureDinamica.width, textureDinamica.height), new Vector2(0.5f, 0.5f), toastRenderer.sprite.pixelsPerUnit);
        toastRenderer.sprite = nuovoSprite;
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        if (Input.GetMouseButton(0))
        {
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.puntaColtello != null)
            {
                PintaSuToast(MinigameCursor.Instance.puntaColtello.position);
            }
            else
            {
                Debug.Log("ATTENZIONE: Manca il riferimento alla punta del coltello nel MinigameCursor!");
            }
        }
    }

    private void PintaSuToast(Vector3 posizioneLama)
    {
        // Usiamo i confini globali nel mondo (A PROVA DI BOMBA)
        Bounds limitiMondo = toastRenderer.bounds;

        // Calcoliamo la percentuale da 0 a 1 di dove si trova il coltello
        float xNorm = Mathf.InverseLerp(limitiMondo.min.x, limitiMondo.max.x, posizioneLama.x);
        float yNorm = Mathf.InverseLerp(limitiMondo.min.y, limitiMondo.max.y, posizioneLama.y);

        // Trasformiamo la percentuale in pixel esatti della texture
        int xTex = Mathf.FloorToInt(xNorm * textureDinamica.width);
        int yTex = Mathf.FloorToInt(yNorm * textureDinamica.height);

        // Se siamo dentro i limiti della texture, spennelliamo!
        if (xTex >= 0 && xTex < textureDinamica.width && yTex >= 0 && yTex < textureDinamica.height)
        {
            Color32 color32Crema = coloreCrema;
            bool cambiataTexture = false;

            for (int x = xTex - raggioPennello; x <= xTex + raggioPennello; x++)
            {
                for (int y = yTex - raggioPennello; y <= yTex + raggioPennello; y++)
                {
                    if (x >= 0 && x < textureDinamica.width && y >= 0 && y < textureDinamica.height)
                    {
                        if (Vector2.Distance(new Vector2(xTex, yTex), new Vector2(x, y)) <= raggioPennello)
                        {
                            int index = y * textureDinamica.width + x;

                            // Coloriamo solo se c'è del pane e non è già colorato
                            if (pixelOriginali[index].a > 5 && !pixelColorati[index])
                            {
                                textureDinamica.SetPixel(x, y, color32Crema);
                                pixelColorati[index] = true;
                                pixelColoratiCorrenti = pixelColoratiCorrenti + 1;
                                cambiataTexture = true;
                            }
                        }
                    }
                }
            }

            if (cambiataTexture)
            {
                textureDinamica.Apply();
                
                float percentualeAttuale = (float)pixelColoratiCorrenti / totaliPixelDaColorare;
                if (percentualeAttuale >= percentualePerVincere)
                {
                    FineTask();
                }
            }
        }
    }

    private void FineTask()
    {
        taskFinito = true;
        Debug.Log("Toast spalmato con successo!");
        
        if (MinigameCursor.Instance != null)
            MinigameCursor.Instance.ImpostaCursore(TipoCursore.Nessuno);

        GameManager.Instance.VinciMinigioco();
    }
}