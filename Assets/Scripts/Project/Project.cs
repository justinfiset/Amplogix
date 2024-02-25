using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Project
{
    public string name = "default";
    [NonSerialized] public string savePath;

    public List<ElectricComponentData> componentDataList;
}
