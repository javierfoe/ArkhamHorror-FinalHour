public class WaitForSeal : WaitForAllActions
{
    public new Pathway SealOn => base.SealOn;

    public WaitForSeal(Building building, int distance = 1) : base(building, true, false, false, false, distance)
    {
    }
}