using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPreviews;
    [SerializeField] private GameObject[] teammate1Preview;
    [SerializeField] private GameObject[] teammate2Preview;
    private List<GameObject[]> teammatesPreviews;
    public event Action OnPreviewChanged;
    private int index = 0;

    private void Awake()
    {
        teammatesPreviews = new List<GameObject[]> { teammate1Preview, teammate2Preview};
    }

    public int ChangePreview(int i)
    {
        characterPreviews[index].SetActive(false);
        RestorePreviewRotation();
        index += i;
        if (index < 0) index = characterPreviews.Length - 1;
        else if (index == characterPreviews.Length) index = 0;
        characterPreviews[index].SetActive(true);
        OnPreviewChanged?.Invoke();
        return index;
    }
    
    public void ForcePreview(int i) //Fuerza la muestra de un elemento
    {
        characterPreviews[index].SetActive(false);
        index = i;
        characterPreviews[index].SetActive(true);
    }

    public GameObject GetCurrentPreview()
    {
        return characterPreviews[index];
    }

    public void RestorePreviewRotation()
    {
        characterPreviews[index].transform.rotation = Quaternion.Euler(0, 203, 0);
    }

    public void UpdateTeammatesPreview()
    {
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        GameObject[] currentPreview = teammate1Preview;
        foreach(PlayerBehaviour player in players)
        {
            if(player.tag.Equals(PlayerBehaviour.ownerTag) && !player.IsOwner)
            {
                int t1Character = player.GetComponent<CharacterInfo>().characterID;
                currentPreview[t1Character].SetActive(true);
                foreach(PreviewSkinManager manager in currentPreview[t1Character].GetComponents<PreviewSkinManager>())
                {
                    manager.SetSkin(player.GetComponent<CharacterSkinManager>().GetSkin());
                }
                currentPreview = teammate2Preview; //Un poco hardcodeado, pero da igual porque como mucho va a tener sólo dos compañeros.
            }
        }
    }
}
