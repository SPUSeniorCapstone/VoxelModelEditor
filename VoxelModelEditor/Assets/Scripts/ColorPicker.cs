using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Image image;

    public Color color = Color.black;

    private void Start()
    {
        image = GetComponent<Image>();
        UpdateColor();
    }

    public void Red(float r)
    {
        color.r = r;
        UpdateColor();
    }

    public void Blue(float b)
    {
        color.b = b;
        UpdateColor();
    }

    public void Green(float g)
    {
        color.g = g;
        UpdateColor();
    }

    void UpdateColor()
    {
        image.color = color;
    }
}
