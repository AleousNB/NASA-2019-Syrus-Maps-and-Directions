using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasControl : MonoBehaviour
{
    public CanvasGroup canvas1;

    void Awake()
    {
        canvas1.alpha = 1f;
        canvas1.interactable = true;
        canvas1.blocksRaycasts = true;
    }

    public void OnButtonClick()
    {
        canvas1.alpha = 0f;
        canvas1.interactable = false;
        canvas1.blocksRaycasts = false;
    }
}
