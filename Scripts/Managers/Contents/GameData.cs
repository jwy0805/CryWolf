using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData
{
    // Game 초기 설정
    // 가변
    public static int[] SpawnMonsterCnt = { 1, 1, 0 }; // { West, North, East }
    
    // 불변
    public static Vector3 Center = new Vector3(0.0f, 6.0f, 0.0f); // Center of the Map
    
    public static int SpawnersCnt = 3;
    public static Vector3[] SpawnerPos { get; set; } = {
        new Vector3(-45.0f, 4.0f, 0.0f), // West
        new Vector3(0.0f, 4.0f, 45.0f),  // North
        new Vector3(45.0f, 4.0f, 0.0f)  // East
    };
    
    public static float RoundTime = 10.0f;
    
    #region FenceData

    public static string[] FenceName = { "", "FenceLv1", "FenceLv2", "FenceLv3" };
    public static int[] FenceCnt = { 0, 18, 34, 42 };
    public static int CurrentFenceCnt = 0;
    public static int[] FenceRow = { 0, 4, 7, 9 };

    public static Vector3[] FenceStartPos =
    {
        new Vector3(0, 0, 0), new Vector3(-4, 6.5f, -7),
        new Vector3(-7, 6, -9), new Vector3(-9, 6, -10)
    };

    public static Vector3[] FenceCenter =
    {
        new Vector3(0, 0, 0), new Vector3(0, 6, -2),
        new Vector3(0, 6, 1), new Vector3(0, 6, 2)
    };

    public static Vector3[] FenceSize =
    {
        new Vector3(0, 0, 0), new Vector3(8, 6, 10),
        new Vector3(12, 6, 20), new Vector3(18, 6, 24)
    };

    public static Bounds FenceBounds;
    
    public static Bounds[] NorthFenceBounds =
    {
        new Bounds(), 
        new Bounds(FenceCenter[1] + new Vector3(0, 0, FenceSize[1].z / 2), 
            new Vector3(FenceSize[1].x / 2, 10, 1)),
        new Bounds(FenceCenter[2] + new Vector3(0, 0, FenceSize[2].z / 2),
            new Vector3(FenceSize[2].x / 2, 10, 1.5f)),
        new Bounds(FenceCenter[3] + new Vector3(0, 0, FenceSize[3].z / 2),
            new Vector3(FenceSize[3].x / 2, 10, 2)),
    };

    public static Bounds[] EastFenceBounds =
    {
        new Bounds(),
        new Bounds(FenceCenter[1] + new Vector3(-FenceSize[1].x / 2, 0, 0),
            new Vector3(1, 10, FenceSize[1].z / 2)),
        new Bounds(FenceCenter[2] + new Vector3(-FenceSize[2].x / 2, 0, 0),
            new Vector3(1.5f, 10, FenceSize[2].z / 2)),
        new Bounds(FenceCenter[3] + new Vector3(-FenceSize[3].x / 2, 0, 0),
            new Vector3(2, 10, FenceSize[3].z / 2)),
    };

    public static Bounds[] WestFenceBounds =
    {
        new Bounds(),
        new Bounds(FenceCenter[1] + new Vector3(FenceSize[1].x / 2, 0, 0),
            new Vector3(1, 10, FenceSize[1].z / 2)),
        new Bounds(FenceCenter[2] + new Vector3(FenceSize[2].x / 2, 0, 0),
            new Vector3(1.5f, 10, FenceSize[2].z / 2)),
        new Bounds(FenceCenter[3] + new Vector3(FenceSize[3].x / 2, 0, 0),
            new Vector3(2, 10, FenceSize[3].z / 2)),
    };

    public static Bounds[] SheepBounds =
    {
        new Bounds(),
        new Bounds(Center, new Vector3(3, 10, 2)),
        new Bounds(new Vector3(0, 6, 2), new Vector3(5, 10, 7)),
        new Bounds(new Vector3(3, 6, 0), new Vector3(10, 10, 9)),
    };
        
    public static Vector3[] GetPos(int cnt, int row, Vector3 startPos)
    {
        Vector3[] posArr = new Vector3[cnt];
        int col = cnt / 2 - row;

        for (int i = 0; i < row + 1; i++)
        {
            posArr[i] = new Vector3(startPos.x + (i * 2), startPos.y, startPos.z);
            posArr[row + col + i] = new Vector3(-startPos.x - (i * 2), startPos.y, startPos.z + col * 2);
        }

        for (int i = 0; i < col - 1; i++)
        {
            posArr[row + 1 + i] = new Vector3(-startPos.x , startPos.y, startPos.z + ((i + 1)* 2));
            posArr[row + col + row + 1 + i] =
                new Vector3(startPos.x, startPos.y, (startPos.z + col * 2) - ((i + 1) * 2));
        }

        return posArr;
    }
    
    public static float[] GetRotation(int cnt, int row)
    {
        float[] rotationArr = new float[cnt];
        int col = cnt / 2 - row;

        for (int i = 0; i < row; i++)
        {
            rotationArr[i] = -90;
            rotationArr[row + col + i] = 90;
        }

        for (int i = 0; i < col; i++)
        {
            rotationArr[row + i] = 180;
            rotationArr[row + col + row + i] = 0;
        }

        return rotationArr;
    }

    #endregion

    // 게임 진행 정보
    #region GameInfo

    public static int StartSheepResource = 200;
    public static int SheepYield = 20;
    public static int StartWolfResource = 120;
    
    public static int StorageLevel = 0;
    public static int[] StorageLvUpCost = { 0, 600, 2000 };
    public static int TowerCapacity = 0;
    public static int[] TowerMaxCapacity = {0, 3, 5, 7};
    public static int SheepCapacity = 3;
    public static int[] SheepMaxCapacity = {0, 6, 12, 20};
    

    public static int[] MonsterCapacity = new int[3];
    public static int MonsterMaxCapacity = 0;
    
    #endregion
    
    public static readonly Dictionary<string, string> Tower = new Dictionary<string, string>()
    {
        { "00", "Bud" }, { "01", "Bloom" }, { "02", "Blossom" },
        { "10", "PracticeDummy" }, { "11", "TargetDummy" }, { "12", "TrainingDummy" },
        { "20", "SunBlossom" }, { "21", "SunflowerFairy" }, { "22", "SunfloraPixie" },
        { "30", "MothLuna" }, { "31", "MothMoon" }, { "32", "MothCelestial" },
        { "40", "Soul" }, { "41", "Haunt" }, { "42", "SoulMage" },
    };
    
    public static readonly Dictionary<string, string> Monster = new Dictionary<string, string>()
    {
        { "50", "WolfPup" }, { "51", "Wolf" }, { "52", "Werewolf" },
        { "60", "Lurker" }, { "61", "Creeper" }, { "62", "Horror" },
        { "70", "Snakelet" }, { "71", "Snake" }, { "72", "SnakeNaga" },
        { "80", "MosquitoBug" }, { "81", "MosquitoPester" }, { "82", "MosquitoStinger" },
        { "90", "Shell" }, { "91", "Spike" }, { "92", "Hermit" },
    };
    
    public static readonly List<string> TowerList = new List<string>(Tower.Values);

    public static readonly List<string> MonsterList = new List<string>(Monster.Values);

    public static readonly List<string> UnitList = TowerList.Concat(MonsterList).ToList();

    public static readonly Dictionary<Define.Skill, int> SkillCost = new Dictionary<Define.Skill, int>()
    {
        { Define.Skill.Bloom3Combo, 120 }
    };
    public static readonly Dictionary<string, string[]> SkillTree = new Dictionary<string, string[]>()
    {
        { "BloomAttack", new[] { "free" } },
        { "BloomAttackSpeed", new[] { "free" } },
        { "BloomRange", new[] { "free" } },
        { "BloomAttackSpeed2", new[] { "BloomAttackSpeed" } },
        { "BloomAirAttack", new[] { "BloomRange" } },
        { "Bloom3Combo", new[] { "BloomAttack", "BloomAttackSpeed2" } },

        { "BlossomPoison", new[] { "free" } },
        { "BlossomAccuracy", new[] { "free" } },
        { "BlossomAttack", new[] { "BlossomPoison", "BlossomAccuracy" } },
        { "BlossomAttackSpeed", new[] { "BlossomPoison", "BlossomAccuracy" } },
        { "BlossomRange", new[] { "BlossomPoison", "BlossomAccuracy" } },
        { "BlossomDeath", new[] { "BlossomAttack", "BlossomAttackSpeed", "BlossomRange" } },

        { "BudAttack", new[] { "free" } },
        { "BudAttackSpeed", new[] { "free" } },
        { "BudRange", new[] { "free" } },
        { "BudSeed", new[] { "BudAttack", "BudAttackSpeed" } },
        { "BudDouble", new[] { "BudSeed", "BudRange" } },

        { "HauntLongAttack", new[] { "free" } },
        { "HauntAttackSpeed", new[] { "free" } },
        { "HauntAttack", new[] { "free" } },
        { "HauntPoisonResist", new[] { "HauntLongAttack", "HauntAttackSpeed", "HauntAttack" } },
        { "HauntFireResist", new[] { "HauntLongAttack", "HauntAttackSpeed", "HauntAttack" } },
        { "HauntFire", new[] { "HauntPoisonResist", "HauntFireResist" } },

        { "MothCelestialSheepHealth", new[] { "free" } },
        { "MothCelestialGroundAttack", new[] { "free" } },
        { "MothCelestialAccuracy", new[] { "free" } },
        { "MothCelestialFireResist", new[] { "MothCelestialSheepHealth", "MothCelestialGroundAttack" } },
        { "MothCelestialPoisonResist", new[] { "MothCelestialSheepHealth", "MothCelestialGroundAttack" } },
        { "MothCelestialPoison", new[] { "MothCelestialAccuracy" } },
        { "MothCelestialBreedSheep", new[] { "MothCelestialPoisonResist", "MothCelestialFireResist" } },

        { "MothMoonRemoveDebuffSheep", new[] { "free" } },
        { "MothMoonHealSheep", new[] { "free" } },
        { "MothMoonRange", new[] { "free" } },
        { "MothMoonOutput", new[] { "MothMoonRemoveDebuffSheep", "MothMoonHealSheep" } },
        { "MothMoonAttackSpeed", new[] { "MothMoonRange" } },

        { "MothLunaAttack", new[] { "free" } },
        { "MothLunaSpeed", new[] { "free" } },
        { "MothLunaAccuracy", new[] { "free" } },
        { "MothLunaFaint", new[] { "MothLunaAttack", "MothLunaSpeed" } },

        { "PracticeDummyHealth", new[] { "free" } },
        { "PracticeDummyDefence", new[] { "free" } },
        { "PracticeDummyHealth2", new[] { "PracticeDummyHealth" } },
        { "PracticeDummyDefence2", new[] { "PracticeDummyDefence" } },
        { "PracticeDummyAggro", new[] { "PracticeDummyHealth2", "PracticeDummyDefence2" } },

        { "SoulMageAvoid", new[] { "free" } },
        { "SoulMageDefenceAll", new[] { "free" } },
        { "SoulMageFireDamage", new[] { "free" } },
        { "SoulMageShareDamage", new[] { "SoulMageDefenceAll" } },
        { "SoulMageTornado", new[] { "SoulMageFireDamage" } },
        { "SoulMageDebuffResist", new[] { "SoulMageAvoid", "SoulMageShareDamage", "SoulMageTornado" } },
        { "SoulMageNatureAttack", new[] { "SoulMageAvoid", "SoulMageShareDamage", "SoulMageTornado" } },
        { "SoulMageCritical", new[] { "SoulMageDebuffResist", "SoulMageNatureAttack" } },

        { "SoulAttack", new[] { "free" } },
        { "SoulDefence", new[] { "free" } },
        { "SoulHealth", new[] { "free" } },
        { "SoulDrain", new[] { "SoulAttack", "SoulDefence", "SoulHealth" } },

        { "SunBlossomHealth", new[] { "free" } },
        { "SunBlossomSlow", new[] { "free" } },
        { "SunBlossomHeal", new[] { "SunBlossomHealth" } },
        { "SunBlossomSlowAttack", new[] { "SunBlossomSlow" } },

        { "SunfloraPixieFaint", new[] { "free" } },
        { "SunfloraPixieHeal", new[] { "free" } },
        { "SunfloraPixieRange", new[] { "free" } },
        { "SunfloraPixieCurse", new[] { "SunfloraPixieFaint" } },
        { "SunfloraPixieAttackSpeed", new[] { "SunfloraPixieHeal" } },
        { "SunfloraPixieTriple", new[] { "SunfloraPixieRange" } },
        {
            "SunfloraPixieDebuffRemove",
            new[] { "SunfloraPixieCurse", "SunfloraPixieAttackSpeed" }
        },
        {
            "SunfloraPixieAttack",
            new[] { "SunfloraPixieCurse", "SunfloraPixieAttackSpeed" }
        },
        { "SunfloraPixieInvincible", new[] { "SunfloraPixieDebuffRemove", "SunfloraPixieAttack" } },

        { "SunflowerFairyAttack", new[] { "free" } },
        { "SunflowerFairyDouble", new[] { "free" } },
        { "SunflowerFairyDefence", new[] { "SunflowerFairyAttack" } },
        { "SunflowerFairyMpDown", new[] { "SunflowerFairyDouble" } },
        { "SunflowerFairyFenceHeal", new[] { "SunflowerFairyDefence", "SunflowerFairyMpDown" } },

        { "TargetDummyHealth", new[] { "free" } },
        { "TargetDummyHeal", new[] { "free" } },
        { "TargetDummyFireResist", new[] { "TargetDummyHealth", "TargetDummyHeal" } },
        { "TargetDummyPoisonResist", new[] { "TargetDummyHealth", "TargetDummyHeal" } },
        { "TargetDummyReflection", new[] { "TargetDummyHealth", "TargetDummyHeal" } },

        { "TrainingDummyAggro", new[] { "free" } },
        { "TrainingDummyHeal", new[] { "free" } },
        { "TrainingDummyFaint", new[] { "free" } },
        { "TrainingDummyHealth", new[] { "TrainingDummyAggro", "TrainingDummyHeal" } },
        { "TrainingDummyDefence", new[] { "TrainingDummyAggro", "TrainingDummyHeal" } },
        { "TrainingDummyPoisonResist", new[] { "TrainingDummyAggro", "TrainingDummyHeal" } },
        { "TrainingDummyFireResist", new[] { "TrainingDummyAggro", "TrainingDummyHeal" } },
    };
    
    public static List<string> SkillUpgradedList = new List<string>();
}



































