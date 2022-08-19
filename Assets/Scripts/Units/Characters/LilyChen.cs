public class LilyChen : Investigator
{
    protected override BadAction BadAction => BadAction.Meditate;

    protected override GoodAction GetGoodAction(int action)
    {
        return action switch
        {
            1 => GoodAction.EmptyHand,
            2 => GoodAction.CalculatedForce,
            3 => GoodAction.PierceTheEye,
            4 => GoodAction.DaughterOfProphecy,
            _ => GoodAction.Meditate
        };
    }
}