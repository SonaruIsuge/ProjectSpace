﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class BalloonInitSetting : MonoBehaviour, IItemInitSetting
{
    [SerializeField] private List<Material> randomBalloonColorSeeds;
    private Renderer balloonRenderer;

    public void Awake()
    {
        balloonRenderer = GetComponentInChildren<Renderer>();
    }

    public void InitSetting()
    {
        var randomMat = Random.Range(0, randomBalloonColorSeeds.Count);
        balloonRenderer.material = Instantiate(randomBalloonColorSeeds[randomMat]);
    }
}
