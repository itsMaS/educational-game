using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerStyle : MonoBehaviour
{
    [SerializeField] private StyleConfiguration configuration;
    [HorizontalLine()]
    [Header("Inner dependancies")]
    [SerializeField] private SpriteRenderer Head;
    [SerializeField] private SpriteRenderer Body;
    [SerializeField] private SpriteRenderer LowerBody;
    [SerializeField] private SpriteRenderer[] Hands;
    [SerializeField] private SpriteRenderer Eyes;
    [SerializeField] private LineRenderer[] HandLines;
    [SerializeField] private TrailRenderer[] HandTrails;

    private void OnValidate()
    {
        configuration.Subscribe(UpdateStyle);
        UpdateStyle();
    }

    public void UpdateStyle()
    {
        Head.color = configuration.HeadColor;
        Body.color = configuration.BodyColor;
        LowerBody.color = configuration.LowerBodyColor;
        Eyes.color = configuration.EyesColor;
        foreach (var item in Hands)
        {
            item.color = configuration.HandsColor;
        }
        foreach (var item in HandLines)
        {
            item.colorGradient = configuration.HandLines;
        }
        foreach (var item in HandTrails)
        {
            item.colorGradient = configuration.HandsTrails;
        }
    }
}
