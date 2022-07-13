using System.Collections;
using System.Collections.Generic;

public class WaitForMovement : WaitForAction
{
    public Building TargetBuilding { get; private set; }

    public WaitForMovement(Investigator investigator, ArkhamHorror arkhamHorror) : base(investigator, arkhamHorror)
    {
    }
}