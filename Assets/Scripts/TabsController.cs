using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    private int currentTabIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTabIndex = 0;
        ActivateTab(0);
    }

    // Update is called once per frame
    void Update()
    {
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
