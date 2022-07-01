public abstract class AncientOneOmen
{
    private int _omenSymbols;
    private readonly int[] _variants;
    
    protected AncientOneOmen(int[] variants)
    {
        _variants = variants;
    }

    public void AddOmenSymbols(int omens)
    {
        _omenSymbols += omens;
    }

    public void ActivateOmenSymbols()
    {
        for(var i = 0; i < _variants.Length; i++)
        {
            if (_variants[i] >= _omenSymbols) continue;
            ActivateInterval(i);
            break;
        }
        _omenSymbols = 0;
    }
    
    public abstract void AddMonster(Monster monster);
    protected abstract void ActivateInterval(int index);
}
