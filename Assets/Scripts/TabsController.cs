using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    private int currentTabIndex;

    void Start()
    {
        currentTabIndex = 0;
        ActivateTab(0);
    }

    void Update()
    {
        // --- FILTRO: Questo script deve ascoltare il TAB *SOLO* se il gioco è in pausa! ---
        if (Time.timeScale != 0f) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int nextIndex = (currentTabIndex + 1) % pages.Length;
            ActivateTab(nextIndex);
        }
    }

    public void ActivateTab(int index)
    {
        currentTabIndex = index;
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.gray;
        }
        pages[index].SetActive(true);
        tabImages[index].color = Color.white;
    }
}