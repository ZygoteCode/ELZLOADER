public class ResourceSemaphore
{
    private int x;

    public ResourceSemaphore() => x = 1;
    public bool IsResourceAvailable() => x == 1;
    public bool IsResourceNotAvailable() => x == 0;

    public bool LockResource()
    {
        if (x == 1)
        {
            x = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UnlockResource()
    {
        if (x == 0)
        {
            x = 1;
        }
    }
}