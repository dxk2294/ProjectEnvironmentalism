﻿using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRoutePath : MonoBehaviour
{
    public List<PathManagerEdge> PathManagerEdges;

    [Range(0, 25.0f)]
    public float VehicleSpeed = 7.5f;

    TradeRoute _tradeRoute;
    GameObject _oilVehicle;

    PathManagerEdge _currentPathManagerEdge;

    splineMove _currentSplineMove;

    bool reversed = false;

    // Start is called before the first frame update
    void Start()
    {
        _tradeRoute = transform.GetComponent<TradeRoute>();
        _oilVehicle = _tradeRoute.OilVehicle;

        _currentPathManagerEdge = PathManagerEdges[0];
        MoveToEndOfPathManagerSegment();
    }

    void MoveToEndOfPathManagerSegment()
    {
        if (_currentSplineMove)
        {
            Destroy(_currentSplineMove);
        }

        _currentSplineMove = _oilVehicle.AddComponent<splineMove>();
        _currentSplineMove.pathContainer = _currentPathManagerEdge.PathManager;
        _currentSplineMove.pathMode = DG.Tweening.PathMode.TopDown2D;
        _currentSplineMove.onStart = true;
        _currentSplineMove.moveToPath = false;
        _currentSplineMove.loopType = splineMove.LoopType.none;
        _currentSplineMove.speed = VehicleSpeed;
        var startIndex = _currentPathManagerEdge.EntryWaypoint.transform.GetSiblingIndex();
        var endIndex = _currentPathManagerEdge.ExitWaypoint.transform.GetSiblingIndex();
        if (reversed)
        {
            var temp = endIndex;
            endIndex = startIndex;
            startIndex = temp;
        }
        _currentSplineMove.reverse = startIndex > endIndex;
        var bezierPathManager = _currentPathManagerEdge.PathManager as BezierPathManager;
        var defaultPoints = 10;
        var startIndexMultiplier = (bezierPathManager != null ? Mathf.CeilToInt(bezierPathManager.pathDetail * defaultPoints) : 1);
        _currentSplineMove.startPoint = startIndex * startIndexMultiplier + (bezierPathManager != null ? 1 : 0);
        StartCoroutine(WaitForSplineInitialization());
    }

    void PathManagerSegmentComplete()
    {
        //Output message to the console

        if ((!reversed && _currentPathManagerEdge.NextEdge == null) || (reversed && _currentPathManagerEdge.PreviousEdge == null))
        {
            reversed = !reversed;
        }
        else
        {
            if (!reversed)
            {
                _currentPathManagerEdge = _currentPathManagerEdge.NextEdge;
            }
            else
            {
                _currentPathManagerEdge = _currentPathManagerEdge.PreviousEdge;
            }
        }
        MoveToEndOfPathManagerSegment();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator WaitForSplineInitialization()
    {
        yield return new WaitForEndOfFrame();
        
        var index = reversed ? _currentPathManagerEdge.EntryWaypoint.transform.GetSiblingIndex() : _currentPathManagerEdge.ExitWaypoint.transform.GetSiblingIndex();
        _currentSplineMove.events[index].AddListener(PathManagerSegmentComplete);

        yield return null;
    }
}