using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OilDerrickDetailPanel : MonoBehaviour
{
    public TextMeshProUGUI extraction;
    public ProgressSlider oilReserves, oilStorage;
    public OilExtractor oilExtractor;
    public GameObject productionTabButton, routesTabButton;
    public GameObject productionTab, routesTab;
    public GameObject routeRowPrefab, countryRowPrefab;
    public GameObject routesList;
    public TextMeshProUGUI routesUsedScreen;

    public Button digDeeperButton;
    public TextMeshProUGUI digDeeperButtonText;

    // Start is called before the first frame update
    void Start()
    {
        SwitchToProductionTab();
    }

    // Update is called once per frame
    void Update()
    {
        if (oilExtractor == null) {
            return;
        }

        extraction.SetText("Extracts " + oilExtractor.OilExtractionRate.ToString() + "/s");
        oilReserves.progress = oilExtractor.CurrentOilReserves / oilExtractor.MaxOilReserves;
        oilStorage.progress = oilExtractor.CurrentOilStorage / oilExtractor.MaxOilStorage;

        ResourceManager rm = GameObject.FindObjectOfType<ResourceManager>();
        digDeeperButton.interactable = rm.CurrentMoney >= oilExtractor.digDeeperCost;
        digDeeperButtonText.SetText("Dig Deeper ($" + oilExtractor.digDeeperCost.ToString("N2") + ")");
    }

    public void routesDidChange() {
        ResourceManager rm = GameObject.FindObjectOfType<ResourceManager>();
        int trucks = rm.MaximumOilTrucks - rm.CurrentOilTrucks;
        if (trucks > 1) {
            routesUsedScreen.SetText(trucks + " trucks are available");
        } else if ( trucks == 1 ) {
            routesUsedScreen.SetText("One truck is available");
        } else {
            routesUsedScreen.SetText("No trucks are available");
        }
        
    }

    public void refresh() {
        // Remove the current routes
        foreach (Transform child in routesList.transform) {
            Destroy(child.gameObject);
        }

        ResourceManager rm = GameObject.FindObjectOfType<ResourceManager>();
        List<City> sortedCities = rm.Cities.OrderBy(c=>c.Name).ToList();

        // Add in Hell cities
        if (rm.EndGameActive) {
            GameObject hell = Instantiate(countryRowPrefab, routesList.transform);
            hell.GetComponent<CountryDetailRow>().Setup(Country.Hell);
            foreach (City city in sortedCities)
            {
                if (city.Country == Country.Hell)
                {
                    GameObject routeRow = Instantiate(routeRowPrefab, routesList.transform);
                    routeRow.GetComponent<RouteDetailRow>().Setup(oilExtractor, city);
                }
            }
        }

        // Add in Mexican cities
        GameObject mexico = Instantiate(countryRowPrefab, routesList.transform);
        mexico.GetComponent<CountryDetailRow>().Setup(Country.Mexico);
        foreach (City city in sortedCities) {
            if (city.Country == Country.Mexico) {
                GameObject routeRow = Instantiate(routeRowPrefab, routesList.transform);
                routeRow.GetComponent<RouteDetailRow>().Setup(oilExtractor, city);
            }
        }
        
        // Add in American cities
        GameObject us = Instantiate(countryRowPrefab, routesList.transform);
        us.GetComponent<CountryDetailRow>().Setup(Country.UnitedStates);
        foreach (City city in sortedCities) {
            if (city.Country == Country.UnitedStates) {
                GameObject routeRow = Instantiate(routeRowPrefab, routesList.transform);
                routeRow.GetComponent<RouteDetailRow>().Setup(oilExtractor, city);
            }
        }

        routesDidChange();
    }

    public void SetOilExtractor(OilExtractor extractor) {
        this.oilExtractor = extractor;
        refresh();
    }

    public void SwitchToProductionTab() {
        productionTab.SetActive(true);
        routesTab.SetActive(false);
        productionTabButton.GetComponent<Image>().enabled = true;
        routesTabButton.GetComponent<Image>().enabled = false;
    }

    public void SwitchToRoutesTab() {
        productionTab.SetActive(false);
        routesTab.SetActive(true);
        productionTabButton.GetComponent<Image>().enabled = false;
        routesTabButton.GetComponent<Image>().enabled = true;
    }

    public void DigDeeper() {
        oilExtractor.DigDeeper();
    }
}
