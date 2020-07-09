using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Style Config", menuName = "Configuration/Style", order = 1)]
public class StyleConfiguration : Configuration
{
    [Header("Player")]
    [SerializeField] private bool autoGenerateColors;
    [SerializeField] [ShowIf("generateColors")] Color color;
    [HideIf("generateColors")] public Color HeadColor;
    [HideIf("generateColors")] public Color BodyColor;
    [HideIf("generateColors")] public Color LowerBodyColor;
    [HideIf("generateColors")] public Color HandsColor;
    [HideIf("generateColors")] public Gradient HandLines;
    public Color EyesColor;
    public Gradient HandsTrails;

    private bool generateColors() { return autoGenerateColors; }
    public override void Validation()
    {
        if(autoGenerateColors)
        {
            HeadColor = color + new Color(0.1f,0.1f,0.1f, 1);
            BodyColor = color - new Color(0.1f,0.1f,0.1f,-1);
            LowerBodyColor = color - new Color(0.2f, 0.2f ,0.2f, -1);
            HandsColor = color + new Color(0.2f,0.2f,0.2f, 1);

            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(color,1);
            colorKeys[1] = new GradientColorKey(color,0);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1, 0);
            alphaKeys[1] = new GradientAlphaKey(1, 1);
            HandLines.SetKeys(colorKeys, alphaKeys);
        }
    }

}
