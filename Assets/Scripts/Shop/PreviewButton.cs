using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewButton : MonoBehaviour
{
    [SerializeField] private PreviewManager manager;
    [SerializeField] private int previewIndex;
    
    public void SetIndex(int index)
    {
        previewIndex = index;
    }

    public void UpdatePreview()
    {
        manager.ForcePreview(previewIndex);
    }
}
