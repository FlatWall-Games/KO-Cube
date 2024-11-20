using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamColorManager : MonoBehaviour
{
    private Image teamColorImage;
    private string teamTag = "Untagged";

    void Awake()
    {
        teamColorImage = GetComponent<Image>();
        StartCoroutine(GetTeam());
    }

    public void SetColor(Color color)
    {
        teamColorImage.color = color;
    }

    IEnumerator GetTeam()
    {
        while (teamTag.Equals("Untagged"))
        {
            teamTag = PlayerBehaviour.ownerTag;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
