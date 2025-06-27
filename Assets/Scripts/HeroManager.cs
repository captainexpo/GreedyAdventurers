using DG.Tweening;
using EasyButtons;
using Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HeroType
{
    Normal
}

public class HeroManager : MonoBehaviour
{

    public Pathfinder pathfinder;
    public Hero activeHero;
    public GridManager gm;

    public GameObject heroPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Started Hero Manager");
        activeHero = Hero.New(new Vector2Int(0, 0), heroPrefab, gm, this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
