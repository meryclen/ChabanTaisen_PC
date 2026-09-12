using UnityEngine;

public class Coloring
{
    Renderer mr;
    MaterialPropertyBlock mpb = new();

    public Coloring(Renderer mr)
    {
        this.mr = mr;
    }

    public void ColorSetting(float r, float g, float b, float a, float emissionStrength)
    {
        var color = new Color(r, g, b, a);
        mpb.SetColor("_Color", color);
        mpb.SetFloat("_EmissionStrength", emissionStrength);
        mr.SetPropertyBlock(mpb);
    }

    public void TextureSetting(Texture2D texture)
    {
        mpb.SetTexture("_BaseMap", texture);
        mr.SetPropertyBlock(mpb);
    }
}
