using UnityEngine;

public interface IPoolable
{
    void Init();
}

public class Poolable : MonoBehaviour
{
    public bool IsUsing;
}