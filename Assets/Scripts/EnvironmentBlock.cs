using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBlock : MonoBehaviour
{
    public int ID
    {
        get;
        private set;
    }

    public void Init(int id)
    {
        this.ID = id;
    }
}
