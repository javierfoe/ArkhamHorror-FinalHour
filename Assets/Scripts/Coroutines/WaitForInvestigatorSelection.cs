using System.Collections.Generic;

public class WaitForInvestigatorSelection : WaitForSelection<Investigator>
{
    public WaitForInvestigatorSelection(IEnumerable<Investigator> elements) : base(elements)
    {
    }

    public WaitForInvestigatorSelection(Building building) : base(building.GetInvestigators())
    {
    }
}