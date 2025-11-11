using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMS.Feedback;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ButtonOnClickEffect : MonoBehaviour
{
    private Button button;
    private RectTransform rectTransform;

    [SerializeField] private DoPunchScaleConfigSO config;

    private void Awake()
    {
        TryGetComponent<RectTransform>(out rectTransform);
        TryGetComponent<Button>(out button);
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    [Button]
    private void OnButtonClick()
    {
        if(config == null)
            return;

        rectTransform.DOKill();
        rectTransform.localScale = Vector3.one;

        rectTransform.DOPunchScale(punch: config.punch,
            duration: config.duration,
            vibrato: config.vibrato,
            elasticity: config.elasticity)
            .SetEase(ease: config.ease);
    }
}
