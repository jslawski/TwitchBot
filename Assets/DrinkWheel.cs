using PolarCoordinates;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrinkWheel : MonoBehaviour
{
    public Dictionary<string, Color> wedgeColors;
    public Image wedgePrefab;
    private float textDistanceFromCenter = 150f;
    public GameObject wheelCanvas;

    public void SetupColorDict()
    {
        wedgeColors = new Dictionary<string, Color>();
        wedgeColors.Add("Vodka/Seductive", new Color(66.0f / 255f, 160 / 255f, 181 / 255f, 1));
        wedgeColors.Add("Midori/JarJar", new Color(36.0f / 255f, 157f / 255f, 53f / 255f, 1));
        wedgeColors.Add("Tequila/Wizened", new Color(195.0f / 255f, 188f / 255f, 93f / 255f, 1));
        wedgeColors.Add("Jager/Surfer", new Color(130.0f / 255f, 7f / 255f, 7f / 255f, 1));
        wedgeColors.Add("Sake/NYBaby", new Color(94.0f / 255f, 205f / 255f, 193f / 255f, 1));
        wedgeColors.Add("SoCo/Jammer", new Color(158.0f / 255f, 79f / 255f, 11f / 255f, 1));
        wedgeColors.Add("Gin/Deep", new Color(174.0f / 255f, 235f / 255f, 152f / 255f, 1));
        wedgeColors.Add("Whiskey/Influencer", new Color(212.0f / 255f, 119f / 255f, 28f / 255f, 1));
        wedgeColors.Add("Rum/Scottish", new Color(212.0f / 255f, 210f / 255f, 210f / 255f, 1));
    }

    public void UpdateValues(Dictionary<string, int> wheelWeights)
    {
        float pieTotal = 0f;
        float wedgeRotation = 0f;

        foreach (KeyValuePair<string, int> entry in wheelWeights)
        {
            pieTotal += entry.Value;
        }

        foreach (KeyValuePair<string, int> entry in wheelWeights)
        {
            Image newWedge = Instantiate(this.wedgePrefab, this.gameObject.transform) as Image;
            TextMeshProUGUI wedgeLabel = newWedge.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            newWedge.color = this.wedgeColors[entry.Key];
            newWedge.fillAmount = (float)entry.Value / pieTotal;

            wedgeLabel.text = ((int)(newWedge.fillAmount * 100)).ToString() + "%";

            float wedgeAngle = newWedge.fillAmount * 360f;
            PolarCoordinate textPC = new PolarCoordinate(this.textDistanceFromCenter, wedgeAngle / 2.0f);
            wedgeLabel.transform.localPosition = textPC.PolarToCartesian();
            newWedge.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -wedgeRotation));
            wedgeLabel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, wedgeRotation));

            wedgeRotation += wedgeAngle;

            //this.AnchorsToCorners(newWedge.gameObject.GetComponent<RectTransform>());
        }
    }

    private void ToggleWheelActive()
    {
        this.wheelCanvas.gameObject.SetActive(!this.wheelCanvas.gameObject.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F6))
        {
            this.ToggleWheelActive();
        }
    }

    private void AnchorsToCorners(RectTransform wedgeRect)
    {
        RectTransform t = wedgeRect;
        RectTransform pt = wedgeRect.gameObject.transform.parent.GetComponent<RectTransform>();

        if (t == null || pt == null) return;

        Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / t.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / t.rect.height);
        Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / t.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / t.rect.height);

        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = new Vector2(0, 0);
    }
}
