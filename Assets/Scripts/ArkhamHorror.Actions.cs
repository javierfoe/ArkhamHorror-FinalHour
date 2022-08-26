using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ArkhamHorror
{
    private IEnumerator StartGoodAction(Investigator investigator, GoodAction goodAction)
    {
        yield return goodAction switch
        {
            GoodAction.AColorForAllSeasons => AColorForAllSeasonsGood(investigator),
            GoodAction.JennysTwin45s => JennysTwin45s(investigator),
            GoodAction.Soiree => Soiree(investigator),
            GoodAction.GrandGala => GrandGala(investigator),
            GoodAction.Socialite => Socialite(investigator),
            _ => throw new ArgumentOutOfRangeException(nameof(goodAction), goodAction, null)
        };
    }

    private IEnumerator StartBadAction(Investigator investigator, BadAction badAction)
    {
        yield return null;
    }

    private IEnumerator AColorForAllSeasonsGood(Investigator investigator)
    {
        var waitForSeal = new WaitForSeal(investigator.Building);
        yield return waitForSeal;
        yield return MoveIfDifferent(investigator, waitForSeal.SelectedBuilding);
        if (waitForSeal.SealOn)
        {
            yield return waitForSeal.SealOn.AddSeal();
        }
    }

    private IEnumerator AColorForAllSeasonsBad(Investigator investigator)
    {
        var waitForMovement = new WaitForMovementBuilding(4, investigator.Building, true);
        yield return waitForMovement;
        foreach (var building in waitForMovement.Buildings)
        {
            yield return building.MoveInvestigator(investigator);
            yield return SpawnMonster(building);
        }
    }

    private IEnumerator Socialite(Investigator investigator)
    {
        yield return DamageOneAdjacentBuilding(investigator, 4, true);
        var waitForOtherDamage = new WaitForDamageMonsters(4, new[] { OtherInvestigator(investigator).Building });
        yield return ConfirmWaitKill(waitForOtherDamage);
    }

    private IEnumerator JennysTwin45s(Investigator investigator)
    {
        var waitForDamage = new WaitForDamageMonsters(2, investigator.Building.GetAdjacentBuildings(true), true);
        yield return ConfirmWaitKill(waitForDamage);
    }

    private IEnumerator Soiree(Investigator investigator)
    {
        Debug.Log("Move or damage (2) adjacent building");
        yield return DamageOneAdjacentBuilding(investigator, 2, true);
        var other = OtherInvestigator(investigator);
        if (other.Building.Zone != investigator.Building.Zone) yield break;
        var waitForOtherDamage = new WaitForDamageMonsters(2, new[] { other.Building });
        Debug.Log("Damage building monster 2");
        yield return ConfirmWaitKill(waitForOtherDamage);
    }

    private IEnumerator GrandGala(Investigator investigator)
    {
        yield return DamageOneAdjacentBuilding(investigator, 2, true);
        //TODO cartas preferencia
    }

    private IEnumerator DamageOneAdjacentBuilding(Investigator investigator, int damage, bool includeSelf = false)
    {
        var waitForDamage = WaitForFactory.WaitForDamage(investigator.Building, 1, damage, includeSelf, true);
        ConfirmWaitFor(waitForDamage);
        yield return waitForDamage;
        yield return MoveIfDifferent(investigator, waitForDamage.SelectedBuilding);
        yield return KillMonsters(waitForDamage.SelectedMonsters);
        yield return null;
    }

    private void ConfirmWaitFor(WaitFor waitFor)
    {
        confirm.onClick.AddListener(waitFor.ConfirmAction);
    }

    private IEnumerator ConfirmWaitKill(WaitForMonsterSelection waitFor)
    {
        ConfirmWaitFor(waitFor);
        yield return waitFor;
        yield return KillMonsters(waitFor.SelectedMonsters);
    }

    private static IEnumerator MoveIfDifferent(Investigator investigator, Building building)
    {
        if (building && building != investigator.Building)
        {
            yield return building.MoveInvestigator(investigator);
        }
    }

    private static IEnumerator KillMonsters(IEnumerable<Monster> monsters)
    {
        foreach (var monster in monsters)
        {
            yield return monster.Destroy();
        }
    }
}