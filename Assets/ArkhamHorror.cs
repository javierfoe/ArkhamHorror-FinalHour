using System.Collections;
using UnityEngine;

public class ArkhamHorror : MonoBehaviour
{
    [SerializeField] private Transform monsters,buildings,investigators;
    [SerializeField] private Building targetBuilding;
    private Monster[] _monsters;
    private Building[] _buildings;
    private Investigator[] _investigators;

    private void Start()
    {
        _investigators = investigators.GetComponentsInChildren<Investigator>();
        foreach (var investigator in _investigators)
        {
            targetBuilding.MoveInvestigator(investigator);
        }
        
        _monsters = monsters.GetComponentsInChildren<Monster>();
        foreach (var monster in _monsters)
        {
            targetBuilding.IncomingMonster(monster);
        }

        _buildings = buildings.GetComponentsInChildren<Building>();
        foreach (var building in _buildings)
        {
            building.FinishMonsterMovement();
        }

        StartCoroutine(MoveMonsters());
    }

    private IEnumerator MoveMonsters()
    {
        while (true)
        {
            
            foreach (var building in _buildings)
            {
                yield return building.ActivateMonsters();
            }

            foreach (var building in _buildings)
            {
                building.FinishMonsterMovement();
            }
            yield return null;
        }
    }
}