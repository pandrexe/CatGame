using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CatAudio : MonoBehaviour
{
    [Header("Componenti")]
    [SerializeField] private AudioSource audioSource;

    [Header("Miagolii Predefiniti")]
    [SerializeField] private AudioClip[] predefinedMeows;

    [Header("Impostazioni Attuali")]
    public bool useRecordedMeow = false;
    private int selectedMeowIndex = 0;

    private AudioClip recordedMeow;
    private string targetDevice;
    private int recordDuration = 1;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0)
        {
            targetDevice = Microphone.devices[0];
        }
    }

    public void PlayMeow()
    {
        if (useRecordedMeow && recordedMeow != null)
        {
            audioSource.PlayOneShot(recordedMeow);
        }
        else if (predefinedMeows.Length > 0 && selectedMeowIndex < predefinedMeows.Length)
        {
            audioSource.PlayOneShot(predefinedMeows[selectedMeowIndex]);
        }
    }

 
    public void SelectPredefinedMeow(int index)
    {
        if (index >= 0 && index < predefinedMeows.Length)
        {
            selectedMeowIndex = index;
            useRecordedMeow = false;
            Debug.Log("Miagolio predefinito selezionato: " + index);
            audioSource.PlayOneShot(predefinedMeows[selectedMeowIndex]);
        }
    }

    public void StartRecording()
    {
        if (targetDevice != null)
        {
            recordedMeow = Microphone.Start(targetDevice, false, recordDuration, 44100);
            useRecordedMeow = true;
            Debug.Log("Registrazione iniziata...");
        }
        else
        {
            Debug.LogWarning("Nessun microfono trovato!");
        }
    }
}