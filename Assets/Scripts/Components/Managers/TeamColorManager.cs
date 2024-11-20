using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamColorManager : MonoBehaviour
{
    private SpriteRenderer teamColorImage;
    private string teamTag = "Untagged";

    void Awake()
    {
        teamColorImage = GetComponent<SpriteRenderer>();
        StartCoroutine(GetTeam());
    }

    public void SetColor(Color color)
    {
        teamColorImage.color = new Color(color.r, color.g, color.b, 0.5f);
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
