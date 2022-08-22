using System.Collections;

public class WaitForFactory
{
    public static WaitForDamage WaitForDamage(Building building, int distance,int  damage, bool includeSelf, bool adjacent)
    {
        var result = new WaitForDamage(building, distance, damage, includeSelf, adjacent);
        return ResetCoroutine(result);
    }

    private static T ResetCoroutine<T>(T coroutine) where T : IEnumerator
    {
        coroutine.Reset();
        return coroutine;
    }
}