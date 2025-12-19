using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyEvaluator : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private DifficultyCurveSO difficultyCurveSO;

    public DifficultyCurveSO DifficultyCurveSO => difficultyCurveSO;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<DifficultyEvaluator>(this);
    }
}
