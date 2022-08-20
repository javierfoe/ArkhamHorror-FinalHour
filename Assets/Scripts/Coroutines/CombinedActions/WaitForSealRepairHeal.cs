public class WaitForSealRepairHeal : WaitForAllActions
{
    public new Pathway SealOn => base.SealOn;
    public new Investigator Heal => base.HealSomebody;
    public new Room RepairOn => base.RepairOn;

    public WaitForSealRepairHeal(Investigator investigator, int distance = 1, int maxActions = 2) : base(investigator, true, true, true,
        false, distance, maxActions)
    {
    }
}