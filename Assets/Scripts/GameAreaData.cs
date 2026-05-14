using System;
using UnityEngine;

[Serializable]
public class GameAreaData
{
    public GameAreaType areaType;

    [Header("Camera")]
    public Transform cameraPoint;

    [Header("UI")]
    public GameObject areaUI;
}