using UnityEngine;

public abstract class Brain : MonoBehaviour
{
    public abstract void Move();
    public abstract void Behave();

    public abstract float DistanceFromPlayer();
}