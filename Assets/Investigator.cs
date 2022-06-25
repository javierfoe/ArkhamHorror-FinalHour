public class Investigator : DwellerGeneric<Location>
{
    protected override void Fill(Location location)
    {
        base.Fill(location);
        location.Building.AddInvestigator(this);
    }

    protected override void Empty(Location location)
    {
        base.Empty(location);
        location.Building.RemoveInvestigator(this);
    }
}
