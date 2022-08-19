public class TommyMuldoon : Investigator
{
    protected override BadAction BadAction => BadAction.ServeAndProtect;

    protected override GoodAction GetGoodAction(int action)
    {
        return action switch
        {
            1 => GoodAction.Becky,
            2 => GoodAction.PoliceSurveillance,
            3 => GoodAction.HighGround,
            4 => GoodAction.StandFirm,
            _ => GoodAction.ServeAndProtect
        };
    }
}