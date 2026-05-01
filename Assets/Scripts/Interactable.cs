using UnityEngine;
using Unity.Cinemachine;

public class InteractableTask : MonoBehaviour
{
    [Header("Impostazioni Task")]
    public CinemachineCamera telecameraDelMinigioco;

    [Header("UI Interazione")]
    public GameObject testoInterazioneUI;

    [Header("Sconfitta Nemico")]
    public MonoBehaviour scriptMovimentoNemico;
    public Collider2D colliderDannoNemico;

    private bool gattoVicino = false;
    private bool taskGiocato = false;

    [Header("Audio")]
    public AudioSource audioContinuoNemico;

    void Start()
    {
        if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
    }

    void Update()
    {
        if (gattoVicino && !taskGiocato && Input.GetKeyDown(KeyCode.E))
        {
            taskGiocato = true;

            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);

            if (audioContinuoNemico != null)
            {
                audioContinuoNemico.spatialBlend = 0f;
            }

            GameManager.Instance.IniziaMinigioco(telecameraDelMinigioco, this);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !taskGiocato)
        {
            gattoVicino = true;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gattoVicino = false;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);
        }
    }

    public void DisattivaNemico()
    {
        if (scriptMovimentoNemico != null)
        {
            scriptMovimentoNemico.enabled = false;
        }

        if (colliderDannoNemico != null)
        {
            colliderDannoNemico.enabled = false;
        }

        if (transform.parent != null)
        {
            transform.parent.tag = "Platform";
        }
    }
}