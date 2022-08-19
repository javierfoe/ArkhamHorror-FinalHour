public class MichaelMcGlen : Investigator
{
    protected override BadAction BadAction => BadAction.Doorkeeper;

    protected override GoodAction GetGoodAction(int action)
    {
        return action switch
        {
            1 => GoodAction.FinishTheWork,
            2 => GoodAction.Dynamite,
            3 => GoodAction.MakeAFuss,
            4 => GoodAction.ChicagoTypewriterMachineGun,
            _ => GoodAction.Doorkeeper
        };
    }
}