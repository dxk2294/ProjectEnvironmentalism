using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRoute : MonoBehaviour
{
    public City City;
    public OilExtractor OilExtractor;
    public GameObject OilTruck;
    public GameObject OilTanker;

    public GameObject OilVehicle { get; private set; }

    public TradeRoutePath TradeRoutePath;

    public float LoadedOilCapacity = 100.0f;
    public float CurrentlyLoadedOil = 0.0f;
    public float OilLoadRate = 20.0f; // Unit/Second

    public float CapacityMultiplier = 1.0f;

    bool _isLoadingOil;
    bool _isUnloadingOil;
    float _startingLoadedOil;

    private ResourceManager _resourceManager;
    private RoadManager _roadManager;

    // Start is called before the first frame update
    void Start()
    {
        _resourceManager = FindObjectOfType<ResourceManager>();

        if (OilExtractor.ExtractedOilSlick.type == OilSlickType.Land)
        {
            OilVehicle = OilTruck;
            LoadedOilCapacity = 100f;
        }
        else
        {
            OilVehicle = OilTanker;
            LoadedOilCapacity = 1_000f;
        }
        //OilVehicle.SetActive(true);
        //OilVehicle.transform.position = OilExtractor.DockingArea.transform.position;

        City.RegisterTradeRoute(this);
        OilExtractor.RegisterTradeRoute(this);

        _roadManager = OilExtractor.ExtractedOilSlick.type == OilSlickType.Land ? GameObject.FindGameObjectWithTag("TruckRoadManager").GetComponent<RoadManager>() :
            GameObject.FindGameObjectWithTag("SeaLaneManager").GetComponent<RoadManager>();
        _roadManager.GenerateTradeRoutePath(this);
    }

    float OilLoadRateVehicleMultiplier()
    {
        if (OilExtractor.ExtractedOilType == OilSlickType.Land)
        {
            return 1.0f;
        } else
        {
            return 10.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_isLoadingOil)
        {
            float remainingOilCapacity = LoadedOilCapacity - CurrentlyLoadedOil;
            float loadableOil = Mathf.Min(remainingOilCapacity, Mathf.Min(OilExtractor.CurrentOilStorage, OilLoadRateVehicleMultiplier() * OilLoadRate * Time.deltaTime));
            if (loadableOil > 0)
            {
                OilExtractor.CurrentOilStorage -= loadableOil;
                CurrentlyLoadedOil += loadableOil;
            }
            
            if(CurrentlyLoadedOil >= (LoadedOilCapacity - 0.001f))
            {
                CurrentlyLoadedOil = LoadedOilCapacity;
                _isLoadingOil = false;
                TradeRoutePath.ResumePath();
                AudioManager.Instance.Play("Resource/OilGlug", pitchMin: 0.8f, pitchMax: 1.2f, volumeMin: 0.45f, volumeMax: 0.65f);
            }
        }

        if (_isUnloadingOil)
        {
            float remainingOilDemand = City.CurrentOilDemand;
            float unloadableOil = Mathf.Min(remainingOilDemand, Mathf.Min(CurrentlyLoadedOil, OilLoadRateVehicleMultiplier() * OilLoadRate * Time.deltaTime));

            if (unloadableOil > 0)
            {
                City.SellOil(unloadableOil);
                CurrentlyLoadedOil -= unloadableOil;
            }

            if (CurrentlyLoadedOil <= (0.001f))
            {
                AudioManager.Instance.Play("Resource/KaChing", pitchMin: 0.8f, pitchMax: 1.2f, volumeMin: 0.45f, volumeMax: 0.65f);

                CurrentlyLoadedOil = 0.0f;
                _startingLoadedOil = 0.0f;
                _isUnloadingOil = false;

                TradeRoutePath.ResumePath();
            }

        }
    }

    public void BeginUnloadingOil()
    {
        _isUnloadingOil = true;
        _startingLoadedOil = CurrentlyLoadedOil;
    }

    public void BeginLoadingOil()
    {
        _isLoadingOil = true;
    }

    private void OnDestroy()
    {
        City.UnregisterTradeRoute(this);
        OilExtractor.UnregisterTradeRoute(this);
    }
}
