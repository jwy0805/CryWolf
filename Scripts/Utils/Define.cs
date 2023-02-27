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
        WolfPup = 150,
        Wolf = 151,
        WereWolf = 152,
        Lurker = 160,
        Creeper = 161,
        Horror = 162,
        Snakelet = 170,
        Snake = 171,
        SnakeNaga = 172,
        MosquitoBug = 180,
        MosquitoPester = 181,
        MosquitoStinger = 182,
        Shell = 190,
        Spike = 191,
        Hermit = 192,
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
        MothCelestialGroundAttack,
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
        
        FenceRepair,
        StorageLvUp,
        GoldIncrease,
        SheepHealth,
        SheepIncrease,
        
        WolfPupSpeed,
        WolfPupHealth,
        WolfPupAttack,
        WolfPupAttackSpeed,
        
        WolfDrain,
        WolfDefence,
        WolfAvoid,
        WolfCritical,
        WolfFireResist,
        WolfPoisonResist,
        WolfDna,
        
        WerewolfThunder,
        WerewolfDebuffResist,
        WerewolfFaint,
        WerewolfHealth,
        WerewolfEnhance,
        
        LurkerSpeed,
        LurkerHealth,
        LurkerDefence,
        LurkerHealth2,
        
        CreeperSpeed,
        CreeperAttackSpeed,
        CreeperAttack,
        CreeperRoll,
        CreeperPoison,
        
        HorrorRollPoison,
        HorrorPoisonStack,
        HorrorHealth,
        HorrorPoisonResist,
        HorrorDefence,
        HorrorPoisonBelt,
        
        SnakeletSpeed,
        SnakeletRange,
        SnakeletAttackSpeed,
        SnakeletAttack,
        
        SnakeAttack,
        SnakeAttackSpeed,
        SnakeRange,
        SnakeAccuracy,
        SnakeFire,
        
        SnakeNagaAttack,
        SnakeNagaRange,
        SnakeNagaFireResist,
        SnakeNagaCritical,
        SnakeNagaDrain,
        SnakeNagaMeteor,
        
        MosquitoBugSpeed,
        MosquitoBugDefence,
        MosquitoBugAvoid,
        MosquitoBugWoolDown,
        
        MosquitoPesterAttack,
        MosquitoPesterHealth,
        MosquitoPesterWoolDown2,
        MosquitoPesterWoolRate,
        MosquitoPesterWoolStop,
        
        MosquitoStingerLongAttack,
        MosquitoStingerHealth,
        MosquitoStingerAvoid,
        MosquitoStingerPoison,
        MosquitoStingerPoisonResist,
        MosquitoStingerInfection,
        MosquitoStingerSheepDeath,
        
        ShellAttackSpeed,
        ShellSpeed,
        ShellHealth,
        ShellRoll,
        
        SpikeSelfDefence,
        SpikeLostHeal,
        SpikeDefence,
        SpikeAttack,
        SpikeDoubleBuff,
        
        HermitFireResist,
        HermitPoisonResist,
        HermitDebuffRemove,
        HermitRange,
        HermitAggro,
        HermitReflection,
        HermitFaint,
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
    
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum TowerId
    {
        Unknown = 100,
        Bud = 101,
        Bloom = 102,
        Blossom = 103,
        PracticeDummy = 111,
        TargetDummy = 112,
        TrainingDummy = 113,
        SunBlossom = 121,
        SunflowerFairy = 122,
        SunfloraPixie = 123,
        MothLuna = 131,
        MothMoon = 132,
        MothCelestial = 133,
        Soul = 141,
        Haunt = 142,
        SoulMage = 143,
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
        PlayerSheep,
        PlayerWolf,
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
        Addicted,
        Aggro,
    }

    public enum SheepCharacter
    {
        PlayerCharacter
    }

    public enum WolfCharacter
    {
        PoisonBomb
    }
}

