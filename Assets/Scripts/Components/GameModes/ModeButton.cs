using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour 
{
    [SerializeField] private int modeIndex;
    private ModeButton[] buttons;

    private void Awake()
    {
        buttons = GameObject.FindObjectsOfType<ModeButton>();
    }

    private void OnEnable()
    {
        if (!GameObject.FindObjectOfType<CharacterSelection>().IsServer) Destroy(transform.parent.gameObject);
    }

    public void OnClick()
    {
        GameObject.FindObjectOfType<ModeSelector>().SetMode(modeIndex);
        MakeSelectionVisual();
    }

    private void MakeSelectionVisual()
    {
        foreach(ModeButton button in buttons)
        {
            button.transform.localScale = new Vector3(1, 1, 1);
            button.GetComponent<Image>().color = Color.white;
        }
        this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        this.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
}
