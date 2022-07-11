public abstract class WaitForAction : WaitFor
{
    protected readonly ArkhamHorror ArkhamHorror;
    protected readonly Investigator Investigator;

    protected WaitForAction(Investigator investigator, ArkhamHorror arkhamHorror)
    {
        Investigator = investigator;
        ArkhamHorror = arkhamHorror;
    }
}