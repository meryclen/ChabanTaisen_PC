public interface ITickable
{
    void Tick();
    void LateTick();
}

public interface IPoolable
{
    void SetPool(IPool pool);
}

public interface IPool
{
    void Return(IPoolable obj);
}

public interface ICancelable
{
    void Cancel();
}
