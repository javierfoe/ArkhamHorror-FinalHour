using System.Collections;

public partial class ArkhamHorror
{
    private IEnumerator StartGoodAction(Investigator investigator, GoodAction goodAction)
    {
        yield return null;
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
        var waitForDamage = new WaitForDamageMonsters(4, investigator.Building.GetAdjacentBuildings(true), 1);
        yield return waitForDamage;
        yield return MoveIfDifferent(investigator, waitForDamage.Building);
        foreach (var monster in waitForDamage.SelectedMonsters)
        {
            yield return monster.Destroy();
        }
        var waitForOtherDamage = new WaitForDamageMonsters(4, new[] { OtherInvestigator(investigator).Building });
        yield return waitForOtherDamage;
        foreach (var monster in waitForOtherDamage.SelectedMonsters)
        {
            yield return monster.Destroy();
        }
    }

    private static IEnumerator MoveIfDifferent(Investigator investigator, Building building)
    {
        if (building != investigator.Building)
        {
            yield return building.MoveInvestigator(investigator);
        }
    }
}