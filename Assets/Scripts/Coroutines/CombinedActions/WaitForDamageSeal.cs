public class WaitForDamageSeal : WaitForSelectMonsters
{
    public new Pathway SealOn => base.SealOn;

    public WaitForDamageSeal(Building building, int distance, int damage, bool adjacent = false) : base(building, distance, damage, adjacent, false, true)
    {
    }
}