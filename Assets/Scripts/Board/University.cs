using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class University : MonoBehaviour
{
    private readonly Dictionary<Gate, Building> _gateBuildings = new();
    private readonly Dictionary<Zone, List<Building>> _zoneBuildings = new();
    private Building _ritualBuilding;
    private Building[] _buildings;

    public List<Building> Buildings => _buildings.ToList();

    public Building GetGateBuilding(Gate gate)
    {
        return _gateBuildings[gate];
    }

    public Building GetLowestGateBuilding()
    {
        var aux = 10;
        Building lowestGatePower = null;
        foreach (var building in _buildings)
        {
            if (building.Gate is Gate.None or Gate.Ritual) continue;
            var gatePower = building.GatePower;
            if (gatePower >= aux) continue;
            lowestGatePower = building;
            aux = gatePower;
        }

        return lowestGatePower;
    }

    public List<Building> GetOtherBuildings()
    {
        var result = new List<Building>();
        foreach (var building in _buildings)
        {
            if (building.Gate != Gate.None) continue;
            result.Add(building);
        }

        return result;
    }

    public List<Building> GetGateBuildings(Gate gate)
    {
        var result = new List<Building>();
        foreach (var building in _buildings)
        {
            if (gate == Gate.None && building.Gate is Gate.None or Gate.Ritual
                || gate != Gate.None && building.Gate != gate) continue;
            result.Add(building);
        }

        return result;
    }

    public List<Seal> GetSeals()
    {
        List<Seal> seals = new();
        foreach (var building in _buildings)
        {
            foreach (var seal in building.GetActiveSeals())
            {
                if (seals.Contains(seal)) continue;
                seals.Add(seal);
            }
        }

        return seals;
    }

    public IEnumerator DamageRitual(int amount)
    {
        yield return _ritualBuilding.Wreck(amount);
    }

    public void IncomingMonsterLowestGate(Monster monster)
    {
        var aux = 10;
        Building lowestGatePower = null;
        foreach (var building in _buildings)
        {
            if (building.Gate is Gate.None or Gate.Ritual) continue;
            var gatePower = building.GatePower;
            if (gatePower >= aux) continue;
            lowestGatePower = building;
            aux = gatePower;
        }

        if (lowestGatePower != null)
        {
            lowestGatePower.IncomingMonster(monster);
        }
    }

    public void IncomingMonster(Monster monster, Gate gate)
    {
        foreach (var building in _buildings)
        {
            if (gate != building.Gate) continue;
            building.IncomingMonster(monster);
        }
    }

    public void FinishMonsterMovement()
    {
        foreach (var building in _buildings)
        {
            building.FinishMonsterMovement();
        }
    }

    private int AddGate(Building building)
    {
        return building.AddGate();
    }

    private void Awake()
    {
        _buildings = GetComponentsInChildren<Building>();

        foreach (var building in _buildings)
        {
            var zone = building.Zone;
            if (!_zoneBuildings.ContainsKey(zone))
            {
                _zoneBuildings.Add(zone, new List<Building>());
            }

            _zoneBuildings[zone].Add(building);
            var gate = building.Gate;
            switch (gate)
            {
                case Gate.None:
                    break;
                case Gate.Ritual:
                    _ritualBuilding = building;
                    break;
                default:
                    if (_gateBuildings.ContainsKey(gate))
                    {
                        Debug.LogWarning(
                            $"A building with gate {gate} already exists: {_gateBuildings[gate]}. Ignoring this building",
                            gameObject);
                        continue;
                    }

                    _gateBuildings.Add(gate, building);
                    AddGate(building);
                    break;
            }
        }
    }
}