using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstaclesPathData
{
    public TrackObstacleType trackObstacleType;
    public float extraDelay;
    public float pathTimerMinLimit;
    public float pathTimerMaxLimit;
    public float safeDistance;
    public float extraOffsetMinDist;
    public float extraOffsetMaxDist;
}

[CreateAssetMenu(fileName = "ObstaclesPathSO", menuName = "ObstaclesPathSO")]
public class ObstaclesPathSO : BaseSO
{
    // obstacles 
    // extraDelay: 0f
    // pathTimerLimit => min -> 2, max -> 5
    // safeDistance -> 5, extraOffsetDist => min -> 3, max -> 5

    // non-movable trains
    // extraDelay: 0f
    // pathTimerLimit => min -> 3, max -> 6
    // safeDistance -> 10, extraOffsetDist => min -> 4, max -> 8

    // note: delay is required if movable trains are to be spawned

    // movable trains
    // extraDelay: 2f
    // pathTimerLimit => min -> 2, max -> 4
    // safeDistance -> 15, extraOffsetDist => min -> 4, max -> 8

    [SerializeField] private ObstaclesPathData[] startBoundObstaclesPathData;
    [SerializeField] private ObstaclesPathData[] endBoundObstaclesPathData;

    private Dictionary<TrackObstacleType, ObstaclesPathData> startBoundObstaclesPathDict = new Dictionary<TrackObstacleType, ObstaclesPathData>();
    private Dictionary<TrackObstacleType, ObstaclesPathData> endBoundObstaclesPathDict = new Dictionary<TrackObstacleType, ObstaclesPathData>();

    public override void InitScriptableData()
    {
        foreach (var data in startBoundObstaclesPathData)
        {
            if (startBoundObstaclesPathDict.ContainsKey(data.trackObstacleType))
                startBoundObstaclesPathDict[data.trackObstacleType] = data;
            else 
                startBoundObstaclesPathDict.Add(data.trackObstacleType, data);
        }

        foreach (var data in endBoundObstaclesPathData)
        {
            if (endBoundObstaclesPathDict.ContainsKey(data.trackObstacleType))
                endBoundObstaclesPathDict[data.trackObstacleType] = data;
            else 
                endBoundObstaclesPathDict.Add(data.trackObstacleType, data);
        }
    }

    public ObstaclesPathData GetStartBoundObstaclesPathData(TrackObstacleType trackObstacleType) => startBoundObstaclesPathDict[trackObstacleType];
    public ObstaclesPathData GetEndBoundObstaclesPathData(TrackObstacleType trackObstacleType) => endBoundObstaclesPathDict[trackObstacleType];
}
