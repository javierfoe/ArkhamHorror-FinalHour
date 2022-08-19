public class RitaYoung : Investigator
{
    protected override BadAction BadAction => BadAction.Obstacles;

    protected override GoodAction GetGoodAction(int action)
    {
        return action switch
        {
            1 => GoodAction.Relays,
            2 => GoodAction.Overcome,
            3 => GoodAction.CrossCountry,
            4 => GoodAction.Marathon,
            _ => GoodAction.Obstacles
        };
    }
}