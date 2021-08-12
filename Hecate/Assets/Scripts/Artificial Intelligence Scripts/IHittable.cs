using UnityEngine;
public interface IHittable
{
    void Hit(int damage = 1, Transform attacker=null);
}