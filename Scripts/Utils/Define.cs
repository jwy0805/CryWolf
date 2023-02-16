using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Way
    {
        West = 0,
        North = 1,
        East = 2,
    }
    
    public enum Layer
    {
        Player = 6,
        Sheep = 7,
        Monsters = 8,
        Ground = 9,
        Block = 10,
        Fence = 11,
        Border = 12,
        Tower = 13,
    }

    public enum MonsterId
    {
        Unknown = 0,
        WolfPup = 50,
        Wolf = 51,
        WereWolf = 52,
        Lurker = 60,
        Creeper = 61,
        Horror = 62,
        Snakelet = 70,
        Snake = 71,
        SnakeNaga = 72,
        MosquitoBug = 80,
        MosquitoPester = 81,
        MosquitoStinger = 82,
        Shell = 90,
        Spike = 91,
        Hermit = 92,
    }
    
    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
        Drag,
    }
    
    public enum Scene
    {
        Unknown,
        Start,
        Login,
        Lobby,
        Game,
        Store,
    }

    public enum Skill
    {
        BloomAttack,
        BloomAttackSpeed,
        BloomRange,
        BloomAttackSpeed2,
        BloomAirAttack,
        Bloom3Combo,
        
        BlossomPoison,
        BlossomAccuracy,
        BlossomAttack,
        BlossomAttackSpeed,
        BlossomRange,
        BlossomDeath,
        
        BudAttack,
        BudAttackSpeed,
        BudRange,
        BudSeed,
        BudDouble,
        
        HauntLongAttack,
        HauntAttackSpeed,
        HauntAttack,
        HauntPoisonResist,
        HauntFireResist,
        HauntFire,
        
        MothCelestialSheepHealth,
        MothCelestialSheepDefence,
        MothCelestialAccuracy,
        MothCelestialFireResist,
        MothCelestialPoisonResist,
        MothCelestialPoison,
        MothCelestialBreedSheep,
        
        MothMoonRemoveDebuffSheep,
        MothMoonHealSheep,
        MothMoonRange,
        MothMoonOutput,
        MothMoonAttackSpeed,
        
        MothLunaAttack,
        MothLunaSpeed,
        MothLunaAccuracy,
        MothLunaFaint,
        
        PracticeDummyHealth,
        PracticeDummyDefence,
        PracticeDummyHealth2,
        PracticeDummyDefence2,
        PracticeDummyAggro,
        
        SoulMageAvoid,
        SoulMageDefenceAll,
        SoulMageFireDamage,
        SoulMageShareDamage,
        SoulMageTornado,
        SoulMageDebuffResist,
        SoulMageNatureAttack,
        SoulMageCritical,
        
        SoulAttack,
        SoulDefence,
        SoulHealth,
        SoulDrain,
        
        SunBlossomHealth,
        SunBlossomSlow,
        SunBlossomHeal,
        SunBlossomSlowAttack,
        
        SunfloraPixieCurse,
        SunfloraPixieHeal,
        SunfloraPixieRange,
        SunfloraPixieFaint,
        SunfloraPixieAttackSpeed,
        SunfloraPixieTriple,
        SunfloraPixieDebuffRemove,
        SunfloraPixieAttack,
        SunfloraPixieInvincible,
        
        SunflowerFairyAttack,
        SunflowerFairyDefence,
        SunflowerFairyDouble,
        SunflowerFairyMpDown,
        SunflowerFairyFenceHeal,
        
        TargetDummyHealth,
        TargetDummyHeal,
        TargetDummyFireResist,
        TargetDummyPoisonResist,
        TargetDummyReflection,
        
        TrainingDummyAggro,
        TrainingDummyHeal,
        TrainingDummyFaint,
        TrainingDummyHealth,
        TrainingDummyDefence,
        TrainingDummyPoisonResist,
        TrainingDummyFireResist,
        TrainingDummyDebuffRemove,
    }
    
    public enum State
    {
        Die,
        Moving,
        Idle,
        Attack,
        Skill,
        Skill2,
        Rush,
        Jump,
        KnockBackCreeper,
        Faint,
        None,
    }

    public enum Condition
    {
        Good,
        Burned,
        Addicted,
        FatalAddicted,
        Aggro,
    }
    
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum TowerId
    {
        Unknown = 0,
        Bud = 1,
        Bloom = 2,
        Blossom = 3,
        PracticeDummy = 11,
        TargetDummy = 12,
        TrainingDummy = 13,
        SunBlossom = 21,
        SunflowerFairy = 22,
        SunfloraPixie = 23,
        MothLuna = 31,
        MothMoon = 32,
        MothCelestial = 33,
        Soul = 41,
        Haunt = 42,
        SoulMage = 43,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum UIState
    {
        Inactive,
        Active,
    }
    
    public enum WorldObject
    {
        Unknown,
        Fence,
        Player,
        Monster,
        Sheep,
        Tower,
        Ground,
        Item,
    }

    public enum Buff
    {
        Attack,
        AttackSpeed,
        Health,
        Defence,
        MoveSpeed,
        Invincible,
    }

    public enum Debuff
    {
        Attack,
        AttackSpeed,
        Defence,
        MoveSpeed,
        Curse,
    }
}

