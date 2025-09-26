using UnityEditor;
using UnityEngine;

public static class CreateDemoCards
{
    [MenuItem("Cards/Create Demo Cards (1-4)")]
    public static void Create()
    {
        string folder = "Assets/Bucky/ScriptableObjects/Cards";
        if (!AssetDatabase.IsValidFolder("Assets/Bucky/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets/Bucky", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets/Bucky/ScriptableObjects", "Cards");

        // Helpers to create effects
        T MakeEffect<T>(string name) where T : CardEffect
        {
            var e = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(e, $"{folder}/{name}.asset");
            return e;
        }

        // 1) Deal 6 to enemy, 4 to self
        var deal6 = MakeEffect<DealDamageEffect>("Deal6");
        deal6.damage = 6;
        var self4 = MakeEffect<SelfDamageEffect>("Self4");
        self4.damage = 4;
        MakeCard("Strike+Self", new CardEffect[] { deal6, self4 });

        // 2) Gain 5 block, lose 1 power
        var blk5 = MakeEffect<GainBlockEffect>("Block5");
        blk5.block = 5;
        var lose1 = MakeEffect<LosePowerEffect>("Lose1");
        lose1.amount = 1;
        MakeCard("Defend-LosePower", new CardEffect[] { blk5, lose1 });

        // 3) Gain 2 power, deal 10 to self
        var pow2 = MakeEffect<GainPowerEffect>("Power2");
        pow2.amount = 2;
        var self10 = MakeEffect<SelfDamageEffect>("Self10");
        self10.damage = 10;
        MakeCard("Rage", new CardEffect[] { pow2, self10 });

        // 4) Deal 6 to all enemies, lose 1 power
        var aoe6 = MakeEffect<DealDamageAllEnemiesEffect>("AOE6");
        aoe6.damage = 6;
        var lose1b = MakeEffect<LosePowerEffect>("Lose1b");
        lose1b.amount = 1;
        MakeCard("Whirlwind-Lose", new CardEffect[] { aoe6, lose1b });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        void MakeCard(string name, CardEffect[] effects)
        {
            var c = ScriptableObject.CreateInstance<CardData>();
            c.cardName = name;
            c.baseEffects = effects;
            AssetDatabase.CreateAsset(c, $"{folder}/{name}.asset");
        }
    }
}


