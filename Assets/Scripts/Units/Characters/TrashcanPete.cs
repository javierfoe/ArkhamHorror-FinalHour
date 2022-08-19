public class TrashcanPete : Investigator
{
    protected override BadAction BadAction => BadAction.EndOfTheWorldVision;

    protected override GoodAction GetGoodAction(int action)
    {
        return action switch
        {
            1 => GoodAction.DukeToTheRescue,
            2 => GoodAction.AVersatileInstrument,
            3 => GoodAction.ThrowABone,
            4 => GoodAction.PropheticDreams,
            _ => GoodAction.EndOfTheWorldVision
        };
    }
}