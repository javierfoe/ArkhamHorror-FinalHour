public class JennyBarnes : Investigator
{
    protected override BadAction BadAction => BadAction.AColorForAllSeasons;

    protected override GoodAction GetGoodAction(int action)
    {
        return action switch
        {
            1 => GoodAction.Socialite,
            2 => GoodAction.JennysTwin45s,
            3 => GoodAction.Soiree,
            4 => GoodAction.GrandGala,
            _ => GoodAction.AColorForAllSeasons
        };
    }
}