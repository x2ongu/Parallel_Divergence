using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum BossState
    {
        Appear,
        Idle,
        Move,
        PreAction,
        Pattern,
        Stunned,
        Disappear,
    }

    public enum EnemyType
    {
        Ground,
        Aireal,
        Boss
    }

    public enum AttackerType
    {
        Player,
        Enemy
    }

    public enum PlayerAttackCollider
    {
        Attack = 0,
        JumpAttack,
        Bash_Forward,
        Bash_Up,
        Bash_Down,
        MAX
    }

    public enum UIEvent
    {
        Enter,
        Exit,
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Drop
    }

    public enum Layer
    {
        Player = 3,
        Enemy = 6,
        Ground,
        Ground_Object,
        Ground_Wall,
        Ground_Wall_Object,
        Background,
        Switch,
        Humanoid,
        Coin,
        Teleport,
        Boss = 22,
        Summoner
    }

    public enum WorldObject
    {
        Unknown,
        Player,
        Enemy,
    }

    public enum State
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    public enum VirtualCamera
    {
        MainCam = -1,
        PlayerCam,
        HackingCam,
    }

    public enum Scene
    {
        Unknown = -1,
        Lobby,
        Loading,
        Map1,
        Map2,
    }
}