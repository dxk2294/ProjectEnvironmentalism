using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OilSlickDetailPanel : MonoBehaviour
{

    public OilSlick oilSlick;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    ResourceManager rm;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }

    public void Refresh() {
        // wtf?
        if (rm == null)
        {
            rm = FindObjectOfType<ResourceManager>();
        }

        if (this.oilSlick == null)
        {
            return;
        }

        (bool, bool, float) affordability = rm.PriceToBuyOilSlick(oilSlick);
        
        buyButton.interactable = affordability.Item1 && affordability.Item2;

        string text = "Purchase ($" + affordability.Item3.ToString("N2") + ")";
        if (!affordability.Item1) {
            text += " (Out of slots)";
        }

        buyButtonText.SetText(text);
    }

    public void SetOilSlick(OilSlick oilSlick) {
        this.oilSlick = oilSlick;
        Refresh();
    }

    public void Buy() {
        PubSubSender sender = gameObject.GetComponent<PubSubSender>();
        sender.Publish("oilslick.purchase", oilSlick);
    }
}
