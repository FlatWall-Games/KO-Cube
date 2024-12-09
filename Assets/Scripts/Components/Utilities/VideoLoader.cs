using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Transform parent;

    void Start()
    {
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);
        videoPlayer.GetComponent<RectTransform>().parent = parent;
        videoPlayer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        videoPlayer.GetComponent<RectTransform>().localScale = Vector2.one;
        videoPlayer.prepareCompleted -= OnPrepared;
        Destroy(this.gameObject);
    }
}
