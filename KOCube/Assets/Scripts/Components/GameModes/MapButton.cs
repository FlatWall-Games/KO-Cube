using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class MapButton: MonoBehaviour
{
    [SerializeField] private int mapIndex;
    [SerializeField] private bool mainMap; //Indica que es el mapa por defecto del modo de juego seleccionado
    private MapButton[] buttons;
    [SerializeField] private string[] mapNames;

    private void Awake()
    {
        buttons = GameObject.FindObjectsOfType<MapButton>();
    }

    private void OnEnable()
    {
        if (!GameObject.FindObjectOfType<CharacterSelection>().IsServer) Destroy(this.gameObject);
        ToggleSelected(this, mainMap);
    }

    public void OnClick()
    {
        MakeSelectionVisual();
    }

    private void MakeSelectionVisual()
    {
        foreach (MapButton button in buttons)
        {
            ToggleSelected(button, false);
        }
        ToggleSelected(this, true);
    }
    
    private void ToggleSelected(MapButton button, bool selected)
    {
        if (selected)
        {
            button.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            button.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            GameObject.FindObjectOfType<MapSelector>().SetMap(mapIndex);
        }
        else
        {
            button.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            button.GetComponent<Image>().color = Color.white;
        }
    }

    public void SetMapIndex(int index)
    {
        mapIndex = index;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = mapNames[index];
        ToggleSelected(this, mainMap);
    }
}
