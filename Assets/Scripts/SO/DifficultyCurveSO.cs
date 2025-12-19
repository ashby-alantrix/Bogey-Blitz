using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficultyCurveType
{
    None = 0, 
    WorldMoveSpeed = 1, 
    MovableTrainSpeed = 2, 
    NonMovableObstacleSpeed = 3, 
}

[System.Serializable]
public class DifficultyCurveData
{
    public DifficultyCurveType CurveType;
    public AnimationCurve AnimCurve;
}

[CreateAssetMenu(menuName = "DifficultyCurveSO", fileName = "DifficultyCurveSO")]
public class DifficultyCurveSO : BaseSO
{
    [SerializeField] private DifficultyCurveData[] difficultyCurveDatas;

    private Dictionary<DifficultyCurveType, AnimationCurve> curvesDict = new Dictionary<DifficultyCurveType, AnimationCurve>();

    public override void InitScriptableData()
    {
        base.InitScriptableData();

        foreach (var difficultyCurveData in difficultyCurveDatas)
        {
            if (curvesDict.ContainsKey(difficultyCurveData.CurveType))
                curvesDict[difficultyCurveData.CurveType] = difficultyCurveData.AnimCurve;
            else
                curvesDict.Add(difficultyCurveData.CurveType, difficultyCurveData.AnimCurve);
        }
    }

    public AnimationCurve GetDifficultyCurve(DifficultyCurveType curveType)
    {
        Debug.Log($"### GetDifficultyCurve: {curveType} :: {curvesDict.ContainsKey(curveType)}");
        return curvesDict[curveType];
    }
}
