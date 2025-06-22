using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Player
    public class Player
    {
        public int level;
    }

    public class PlayerData : ILoader<int, Player>
    {
        public List<Player> components = new();

        public Dictionary<int, Player> MakeDict()
        {
            Dictionary<int, Player> dict = new();

            foreach (Player component in components)
                dict.Add(component.level, component);

            return dict;
        }
    }
    #endregion

    #region General Enemy
    public class GeneralEnemy
    {
        public int ID;
        public string Name;

        public int MaxHp;
        public float MoveSpeed;
        public float AttackSpeed;
        public float AttackRange;

        public bool IsGroundEnemy;

        public float GroundTraceDistanceX;
        public float GroundTraceDistanceY;

        public float AirealTraceRange;
    }

    public class GeneralEnemyData : ILoader<int, GeneralEnemy>
    {
        public List<GeneralEnemy> components = new();

        public Dictionary<int, GeneralEnemy> MakeDict()
        {
            Dictionary<int, GeneralEnemy> dict = new();

            foreach (GeneralEnemy component in components)
                dict.Add(component.ID, component);

            return dict;
        }
    }
    #endregion

    #region Boss Enemy
    public class BossEnemy
    {
        public int ID;
        public string Name;

        public int MaxHp;
        public float MoveSpeed;
    }

    public class BossEnemyData : ILoader<int, BossEnemy>
    {
        public List<BossEnemy> components = new();

        public Dictionary<int, BossEnemy> MakeDict()
        {
            Dictionary<int, BossEnemy> dict = new();

            foreach (BossEnemy component in components)
                dict.Add(component.ID, component);

            return dict;
        }
    }
    #endregion
}