using UnityEditor;
using UnityEngine;

public static class CreateDemoCards
{
    [MenuItem("Cards/Create Demo Cards (1-4)")]
    public static void Create()
    {
        string effectsFolder = "Assets/Bucky/ScriptableObjects/Effects";
        string cardsFolder = "Assets/Bucky/ScriptableObjects/Cards";
        if (!AssetDatabase.IsValidFolder("Assets/Bucky/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets/Bucky", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder(effectsFolder))
            AssetDatabase.CreateFolder("Assets/Bucky/ScriptableObjects", "Effects");
        if (!AssetDatabase.IsValidFolder(cardsFolder))
            AssetDatabase.CreateFolder("Assets/Bucky/ScriptableObjects", "Cards");

        // Helpers to create effects
        T MakeEffect<T>(string name) where T : CardEffect
        {
            var e = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(e, $"{effectsFolder}/{name}.asset");
            return e;
        }

        // 1) Deal 6 to enemy, 4 to self
        var deal6 = MakeEffect<DealDamageEffect>("Deal6");
        deal6.damage = 6;
        deal6.description = $"Deal {deal6.damage} damage to a target (+ Power)";
        EditorUtility.SetDirty(deal6);

        var self4 = MakeEffect<SelfDamageEffect>("Self4");
        self4.damage = 4;
        self4.description = $"Deal {self4.damage} damage to yourself";
        EditorUtility.SetDirty(self4);

        MakeCard(
            name: "Strike+Self",
            effects: new CardEffect[] { deal6, self4 },
            cardDescription: "Deal 6 damage to a target (+ Power). Then take 4 self-damage."
        );

        // 2) Gain 5 block, lose 1 power
        var blk5 = MakeEffect<GainBlockEffect>("Block5");
        blk5.block = 5;
        blk5.description = $"Gain {blk5.block} Block";
        EditorUtility.SetDirty(blk5);

        var lose1 = MakeEffect<LosePowerEffect>("Lose1");
        lose1.amount = 1;
        lose1.description = $"Lose {lose1.amount} Power";
        EditorUtility.SetDirty(lose1);

        MakeCard(
            name: "Defend-LosePower",
            effects: new CardEffect[] { blk5, lose1 },
            cardDescription: "Gain 5 Block. Then lose 1 Power."
        );

        // 3) Gain 2 power, deal 10 to self
        var pow1 = MakeEffect<GainPowerEffect>("Power1");
        pow1.amount = 1;
        pow1.description = $"Gain {pow1.amount} Power";
        EditorUtility.SetDirty(pow1);

        var self10 = MakeEffect<SelfDamageEffect>("Self10");
        self10.damage = 10;
        self10.description = $"Deal {self10.damage} damage to yourself";
        EditorUtility.SetDirty(self10);

        MakeCard(
            name: "Rage",
            effects: new CardEffect[] { pow1, self10 },
            cardDescription: "Gain 2 Power. Then take 10 self-damage."
        );

        // 4) Deal 6 to all enemies, lose 1 power
        var aoe6 = MakeEffect<DealDamageAllEnemiesEffect>("AOE6");
        aoe6.damage = 6;
        aoe6.description = $"Deal {aoe6.damage} damage to ALL enemies (+ Power)";
        EditorUtility.SetDirty(aoe6);

        var lose1b = MakeEffect<LosePowerEffect>("Lose1b");
        lose1b.amount = 1;
        lose1b.description = $"Lose {lose1b.amount} Power";
        EditorUtility.SetDirty(lose1b);

        MakeCard(
            name: "Whirlwind-Lose",
            effects: new CardEffect[] { aoe6, lose1b },
            cardDescription: "Deal 6 damage to ALL enemies (+ Power). Then lose 1 Power."
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        void MakeCard(string name, CardEffect[] effects, string cardDescription)
        {
            var c = ScriptableObject.CreateInstance<CardData>();
            c.cardName = name;
            c.baseEffects = effects;
            c.description = cardDescription;
            AssetDatabase.CreateAsset(c, $"{cardsFolder}/{name}.asset");
            EditorUtility.SetDirty(c);
        }
    }
}


