using System;
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

        // Upgrade route demo: cursed/purified/blessed Strike
        var deal6b = MakeEffect<DealDamageEffect>("Deal6b");
        deal6b.damage = 6;
        var deal8 = MakeEffect<DealDamageEffect>("Deal8");
        deal8.damage = 8;
        var self2 = MakeEffect<SelfDamageEffect>("Self2");
        self2.damage = 2;
        var heal2 = MakeEffect<HealEffect>("Heal2");
        heal2.amount = 2;

        var cursedStrike = ScriptableObject.CreateInstance<CardData>();
        cursedStrike.cardName = "Cursed Strike";
        cursedStrike.cursedEffects = new CardEffect[] { deal6b, self2 };
        cursedStrike.purifiedEffects = new CardEffect[] { deal6b };
        cursedStrike.blessedEffects = new CardEffect[] { deal8, heal2 };
        AssetDatabase.CreateAsset(cursedStrike, $"{cardsFolder}/Cursed Strike.asset");
        EditorUtility.SetDirty(cursedStrike);

        // 2) Gain 5 block, lose 1 power
        var blk5 = MakeEffect<GainBlockEffect>("Block5");
        blk5.block = 5;
        blk5.description = $"Gain {blk5.block} Block";
        EditorUtility.SetDirty(blk5);

        var lose1 = MakeEffect<LosePowerEffect>("Lose1");
        lose1.amount = 1;
        lose1.description = $"Lose {lose1.amount} Power";
        EditorUtility.SetDirty(lose1);

        // Cursed Block: cursed gain 6 block + take 2; purified 8 block; blessed 8 block
        var blk6 = MakeEffect<GainBlockEffect>("Block6"); blk6.block = 6;
        var blk8 = MakeEffect<GainBlockEffect>("Block8"); blk8.block = 8;
        var self2b = MakeEffect<SelfDamageEffect>("Self2b"); self2b.damage = 2;

        var cursedBlock = ScriptableObject.CreateInstance<CardData>();
        cursedBlock.cardName = "Cursed Block";
        cursedBlock.cursedEffects = new CardEffect[] { blk6, self2b };
        cursedBlock.purifiedEffects = new CardEffect[] { blk8 };
        cursedBlock.blessedEffects = new CardEffect[] { blk8 };
        AssetDatabase.CreateAsset(cursedBlock, $"{cardsFolder}/Cursed Block.asset");
        EditorUtility.SetDirty(cursedBlock);

        // 3) Gain 2 power, deal 10 to self
        var pow1 = MakeEffect<GainPowerEffect>("Power1");
        pow1.amount = 1;
        pow1.description = $"Gain {pow1.amount} Power";
        EditorUtility.SetDirty(pow1);

        var self10 = MakeEffect<SelfDamageEffect>("Self10");
        self10.damage = 10;
        self10.description = $"Deal {self10.damage} damage to yourself";
        EditorUtility.SetDirty(self10);

        // Cursed Focus: cursed +1 power, take 4; purified +1 power; blessed +2 power
        var pow2 = MakeEffect<GainPowerEffect>("Power2"); pow2.amount = 2;
        var self4b = MakeEffect<SelfDamageEffect>("Self4b"); self4b.damage = 4;

        var cursedFocus = ScriptableObject.CreateInstance<CardData>();
        cursedFocus.cardName = "Cursed Focus";
        cursedFocus.cursedEffects = new CardEffect[] { pow1, self4b };
        cursedFocus.purifiedEffects = new CardEffect[] { pow1 };
        cursedFocus.blessedEffects = new CardEffect[] { pow2 };
        AssetDatabase.CreateAsset(cursedFocus, $"{cardsFolder}/Cursed Focus.asset");
        EditorUtility.SetDirty(cursedFocus);

        // 4) Deal 6 to all enemies, lose 1 power
        var aoe6 = MakeEffect<DealDamageAllEnemiesEffect>("AOE6");
        aoe6.damage = 6;
        aoe6.description = $"Deal {aoe6.damage} damage to ALL enemies (+ Power)";
        EditorUtility.SetDirty(aoe6);

        var lose1b = MakeEffect<LosePowerEffect>("Lose1b");
        lose1b.amount = 1;
        lose1b.description = $"Lose {lose1b.amount} Power";
        EditorUtility.SetDirty(lose1b);

        // Cursed Recover: cursed heal 6, lose 4 block; purified heal 6; blessed heal 10
        var heal6 = MakeEffect<HealEffect>("Heal6"); heal6.amount = 6;
        var heal10 = MakeEffect<HealEffect>("Heal10"); heal10.amount = 10;
        var loseAllBlock = MakeEffect<RemoveAllBlockEffect>("LoseAllBlock");
        loseAllBlock.affectOnlyPlayer = true;

        var cursedRecover = ScriptableObject.CreateInstance<CardData>();
        cursedRecover.cardName = "Cursed Recover";
        // Implement "Lose 4 Block" as remove all (approximation)
        cursedRecover.cursedEffects = new CardEffect[] { heal6, loseAllBlock };
        cursedRecover.purifiedEffects = new CardEffect[] { heal6 };
        cursedRecover.blessedEffects = new CardEffect[] { heal10 };
        AssetDatabase.CreateAsset(cursedRecover, $"{cardsFolder}/Cursed Recover.asset");
        EditorUtility.SetDirty(cursedRecover);

        // Rares
        var aoe5 = MakeEffect<DealDamageAllEnemiesEffect>("AOE5"); aoe5.damage = 5;
        var aoe8 = MakeEffect<DealDamageAllEnemiesEffect>("AOE8"); aoe8.damage = 8;
        var self5 = MakeEffect<SelfDamageEffect>("Self5"); self5.damage = 5;

        var cursedCleave = ScriptableObject.CreateInstance<CardData>();
        cursedCleave.cardName = "Cursed Cleave";
        cursedCleave.cursedEffects = new CardEffect[] { aoe5, self5 };
        cursedCleave.purifiedEffects = new CardEffect[] { aoe5 };
        cursedCleave.blessedEffects = new CardEffect[] { aoe8 };
        AssetDatabase.CreateAsset(cursedCleave, $"{cardsFolder}/Cursed Cleave.asset");
        EditorUtility.SetDirty(cursedCleave);

        var pow2b = MakeEffect<GainPowerEffect>("Power2b"); pow2b.amount = 2;
        var lose2blk = MakeEffect<RemoveAllBlockEffect>("LoseAllBlockNow"); lose2blk.affectOnlyPlayer = true; // approximation for -2 block
        var self6 = MakeEffect<SelfDamageEffect>("Self6"); self6.damage = 6;
        var blk4 = MakeEffect<GainBlockEffect>("Block4"); blk4.block = 4;

        var cursedRally = ScriptableObject.CreateInstance<CardData>();
        cursedRally.cardName = "Cursed Rally";
        cursedRally.cursedEffects = new CardEffect[] { pow2b, lose2blk, self6 };
        var self4c = MakeEffect<SelfDamageEffect>("Self4c"); self4c.damage = 4;
        cursedRally.purifiedEffects = new CardEffect[] { pow2b, self4c };
        cursedRally.blessedEffects = new CardEffect[] { pow2b, blk4 };
        AssetDatabase.CreateAsset(cursedRally, $"{cardsFolder}/Cursed Rally.asset");
        EditorUtility.SetDirty(cursedRally);

		var deal8c = MakeEffect<DealDamageEffect>("Deal8c"); deal8c.damage = 8;
        var deal4 = MakeEffect<DealDamageEffect>("Deal4"); deal4.damage = 4;

        var cursedSiphon = ScriptableObject.CreateInstance<CardData>();
        cursedSiphon.cardName = "Cursed Siphon Strike";
		cursedSiphon.cursedEffects = new CardEffect[] { deal8c, MakeEffect<HealEffect>("Heal8").Also(h=> h.amount=8), MakeEffect<LosePowerEffect>("Lose1c").Also(l=> l.amount=1) };
        cursedSiphon.purifiedEffects = new CardEffect[] { deal4, MakeEffect<HealEffect>("Heal4").Also(h=> h.amount=4) };
		cursedSiphon.blessedEffects = new CardEffect[] { deal8c, MakeEffect<HealEffect>("Heal8b").Also(h=> h.amount=8) };
        AssetDatabase.CreateAsset(cursedSiphon, $"{cardsFolder}/Cursed Siphon Strike.asset");
        EditorUtility.SetDirty(cursedSiphon);

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

        // Local extension-like helper to set fields inline for newly created effects
    }

    [MenuItem("Cards/Create Cursed Set (3-Stage)")]
    public static void CreateCursedSet()
    {
        string effectsFolder = "Assets/Bucky/ScriptableObjects/Effects";
        string cardsFolder = "Assets/Bucky/ScriptableObjects/Cards";
        if (!AssetDatabase.IsValidFolder("Assets/Bucky/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets/Bucky", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder(effectsFolder))
            AssetDatabase.CreateFolder("Assets/Bucky/ScriptableObjects", "Effects");
        if (!AssetDatabase.IsValidFolder(cardsFolder))
            AssetDatabase.CreateFolder("Assets/Bucky/ScriptableObjects", "Cards");

        T MakeEffect<T>(string name) where T : CardEffect
        {
            var e = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(e, $"{effectsFolder}/{name}.asset");
            return e;
        }

        // Commons
        var deal6_cs = MakeEffect<DealDamageEffect>("Strike_Deal6"); deal6_cs.damage = 6; EditorUtility.SetDirty(deal6_cs);
        var deal8_cs = MakeEffect<DealDamageEffect>("Strike_Deal8"); deal8_cs.damage = 8; EditorUtility.SetDirty(deal8_cs);
        var self2_cs = MakeEffect<SelfDamageEffect>("Strike_Self2"); self2_cs.damage = 2; EditorUtility.SetDirty(self2_cs);
        var heal2_cs = MakeEffect<HealEffect>("Strike_Heal2"); heal2_cs.amount = 2; EditorUtility.SetDirty(heal2_cs);
        var cursedStrike = ScriptableObject.CreateInstance<CardData>();
        cursedStrike.cardName = "Cursed Strike";
        cursedStrike.cursedEffects = new CardEffect[] { deal6_cs, self2_cs };
        cursedStrike.purifiedEffects = new CardEffect[] { deal6_cs };
        cursedStrike.blessedEffects = new CardEffect[] { deal8_cs, heal2_cs };
        AssetDatabase.CreateAsset(cursedStrike, $"{cardsFolder}/Cursed Strike.asset"); EditorUtility.SetDirty(cursedStrike);

        var blk6_cb = MakeEffect<GainBlockEffect>("Block_Block6"); blk6_cb.block = 6; EditorUtility.SetDirty(blk6_cb);
        var blk8_cb = MakeEffect<GainBlockEffect>("Block_Block8"); blk8_cb.block = 8; EditorUtility.SetDirty(blk8_cb);
        var self2_cb = MakeEffect<SelfDamageEffect>("Block_Self2"); self2_cb.damage = 2; EditorUtility.SetDirty(self2_cb);
        var cursedBlock = ScriptableObject.CreateInstance<CardData>();
        cursedBlock.cardName = "Cursed Block";
        cursedBlock.cursedEffects = new CardEffect[] { blk6_cb, self2_cb };
        cursedBlock.purifiedEffects = new CardEffect[] { blk8_cb };
        cursedBlock.blessedEffects = new CardEffect[] { blk8_cb };
        AssetDatabase.CreateAsset(cursedBlock, $"{cardsFolder}/Cursed Block.asset"); EditorUtility.SetDirty(cursedBlock);

        var pow1_cf = MakeEffect<GainPowerEffect>("Focus_Power1"); pow1_cf.amount = 1; EditorUtility.SetDirty(pow1_cf);
        var pow2_cf = MakeEffect<GainPowerEffect>("Focus_Power2"); pow2_cf.amount = 2; EditorUtility.SetDirty(pow2_cf);
        var self4_cf = MakeEffect<SelfDamageEffect>("Focus_Self4"); self4_cf.damage = 4; EditorUtility.SetDirty(self4_cf);
        var cursedFocus = ScriptableObject.CreateInstance<CardData>();
        cursedFocus.cardName = "Cursed Focus";
        cursedFocus.cursedEffects = new CardEffect[] { pow1_cf, self4_cf };
        cursedFocus.purifiedEffects = new CardEffect[] { pow1_cf };
        cursedFocus.blessedEffects = new CardEffect[] { pow2_cf };
        AssetDatabase.CreateAsset(cursedFocus, $"{cardsFolder}/Cursed Focus.asset"); EditorUtility.SetDirty(cursedFocus);

        var heal6_cr = MakeEffect<HealEffect>("Recover_Heal6"); heal6_cr.amount = 6; EditorUtility.SetDirty(heal6_cr);
        var heal10_cr = MakeEffect<HealEffect>("Recover_Heal10"); heal10_cr.amount = 10; EditorUtility.SetDirty(heal10_cr);
        var lose4blk = MakeEffect<LoseBlockEffect>("Recover_Lose4Block"); lose4blk.amount = 4; EditorUtility.SetDirty(lose4blk);
        var cursedRecover = ScriptableObject.CreateInstance<CardData>();
        cursedRecover.cardName = "Cursed Recover";
        cursedRecover.cursedEffects = new CardEffect[] { heal6_cr, lose4blk };
        cursedRecover.purifiedEffects = new CardEffect[] { heal6_cr };
        cursedRecover.blessedEffects = new CardEffect[] { heal10_cr };
        AssetDatabase.CreateAsset(cursedRecover, $"{cardsFolder}/Cursed Recover.asset"); EditorUtility.SetDirty(cursedRecover);

        // Rares
        var aoe5_cc = MakeEffect<DealDamageAllEnemiesEffect>("Cleave_AOE5"); aoe5_cc.damage = 5; EditorUtility.SetDirty(aoe5_cc);
        var aoe8_cc = MakeEffect<DealDamageAllEnemiesEffect>("Cleave_AOE8"); aoe8_cc.damage = 8; EditorUtility.SetDirty(aoe8_cc);
        var self5_cc = MakeEffect<SelfDamageEffect>("Cleave_Self5"); self5_cc.damage = 5; EditorUtility.SetDirty(self5_cc);
        var cursedCleave = ScriptableObject.CreateInstance<CardData>();
        cursedCleave.cardName = "Cursed Cleave";
        cursedCleave.cursedEffects = new CardEffect[] { aoe5_cc, self5_cc };
        cursedCleave.purifiedEffects = new CardEffect[] { aoe5_cc };
        cursedCleave.blessedEffects = new CardEffect[] { aoe8_cc };
        AssetDatabase.CreateAsset(cursedCleave, $"{cardsFolder}/Cursed Cleave.asset"); EditorUtility.SetDirty(cursedCleave);

        var pow2_crl = MakeEffect<GainPowerEffect>("Rally_Power2"); pow2_crl.amount = 2; EditorUtility.SetDirty(pow2_crl);
        var lose2block = MakeEffect<LoseBlockEffect>("Rally_Lose2Block"); lose2block.amount = 2; EditorUtility.SetDirty(lose2block);
        var self6_crl = MakeEffect<SelfDamageEffect>("Rally_Self6"); self6_crl.damage = 6; EditorUtility.SetDirty(self6_crl);
        var self4_crl = MakeEffect<SelfDamageEffect>("Rally_Self4"); self4_crl.damage = 4; EditorUtility.SetDirty(self4_crl);
        var blk4_crl = MakeEffect<GainBlockEffect>("Rally_Block4"); blk4_crl.block = 4; EditorUtility.SetDirty(blk4_crl);
        var cursedRally = ScriptableObject.CreateInstance<CardData>();
        cursedRally.cardName = "Cursed Rally";
        cursedRally.cursedEffects = new CardEffect[] { pow2_crl, lose2block, self6_crl };
        cursedRally.purifiedEffects = new CardEffect[] { pow2_crl, self4_crl };
        cursedRally.blessedEffects = new CardEffect[] { pow2_crl, blk4_crl };
        AssetDatabase.CreateAsset(cursedRally, $"{cardsFolder}/Cursed Rally.asset"); EditorUtility.SetDirty(cursedRally);

        var deal8_css = MakeEffect<DealDamageEffect>("Siphon_Deal8"); deal8_css.damage = 8; EditorUtility.SetDirty(deal8_css);
        var heal8_css = MakeEffect<HealEffect>("Siphon_Heal8"); heal8_css.amount = 8; EditorUtility.SetDirty(heal8_css);
        var lose1p_css = MakeEffect<LosePowerEffect>("Siphon_Lose1"); lose1p_css.amount = 1; EditorUtility.SetDirty(lose1p_css);
        var deal4_css = MakeEffect<DealDamageEffect>("Siphon_Deal4"); deal4_css.damage = 4; EditorUtility.SetDirty(deal4_css);
        var heal4_css = MakeEffect<HealEffect>("Siphon_Heal4"); heal4_css.amount = 4; EditorUtility.SetDirty(heal4_css);
        var cursedSiphon = ScriptableObject.CreateInstance<CardData>();
        cursedSiphon.cardName = "Cursed Siphon Strike";
        cursedSiphon.cursedEffects = new CardEffect[] { deal8_css, heal8_css, lose1p_css };
        cursedSiphon.purifiedEffects = new CardEffect[] { deal4_css, heal4_css };
        cursedSiphon.blessedEffects = new CardEffect[] { deal8_css, heal8_css };
        AssetDatabase.CreateAsset(cursedSiphon, $"{cardsFolder}/Cursed Siphon Strike.asset"); EditorUtility.SetDirty(cursedSiphon);

        // Epics
        var pow4_co = MakeEffect<GainPowerEffect>("Overcharge_Power4"); pow4_co.amount = 4; EditorUtility.SetDirty(pow4_co);
        var skip2 = MakeEffect<SkipTurnsEffect>("Skip2"); ((SkipTurnsEffect)skip2).turns = 2; EditorUtility.SetDirty(skip2);
        var skip1 = MakeEffect<SkipTurnsEffect>("Skip1"); ((SkipTurnsEffect)skip1).turns = 1; EditorUtility.SetDirty(skip1);
        var cursedOvercharge = ScriptableObject.CreateInstance<CardData>();
        cursedOvercharge.cardName = "Cursed Overcharge";
        cursedOvercharge.cursedEffects = new CardEffect[] { pow4_co, skip2 };
        cursedOvercharge.purifiedEffects = new CardEffect[] { pow4_co, skip1 };
        cursedOvercharge.blessedEffects = new CardEffect[] { pow4_co };
        AssetDatabase.CreateAsset(cursedOvercharge, $"{cardsFolder}/Cursed Overcharge.asset"); EditorUtility.SetDirty(cursedOvercharge);

        var bloodletting_c = ScriptableObject.CreateInstance<CardData>();
        var psd_c = MakeEffect<PowerScalingDamageEffect>("Bloodletting_Cursed"); ((PowerScalingDamageEffect)psd_c).baseDamage = 10; ((PowerScalingDamageEffect)psd_c).perPower = 2; EditorUtility.SetDirty(psd_c);
        var self10 = MakeEffect<SelfDamageEffect>("Self10_Bloodletting"); self10.damage = 10; EditorUtility.SetDirty(self10);
        var psd_p = MakeEffect<PowerScalingDamageEffect>("Bloodletting_Purified"); ((PowerScalingDamageEffect)psd_p).baseDamage = 10; ((PowerScalingDamageEffect)psd_p).perPower = 2; EditorUtility.SetDirty(psd_p);
        var psd_b = MakeEffect<PowerScalingDamageEffect>("Bloodletting_Blessed"); ((PowerScalingDamageEffect)psd_b).baseDamage = 10; ((PowerScalingDamageEffect)psd_b).perPower = 3; EditorUtility.SetDirty(psd_b);
        bloodletting_c.cardName = "Cursed Bloodletting";
        bloodletting_c.cursedEffects = new CardEffect[] { psd_c, self10 };
        bloodletting_c.purifiedEffects = new CardEffect[] { psd_p };
        bloodletting_c.blessedEffects = new CardEffect[] { psd_b };
        AssetDatabase.CreateAsset(bloodletting_c, $"{cardsFolder}/Cursed Bloodletting.asset"); EditorUtility.SetDirty(bloodletting_c);

        var multi6x2 = MakeEffect<MultiHitDamageEffect>("EchoStrike_6x2"); ((MultiHitDamageEffect)multi6x2).damagePerHit = 6; ((MultiHitDamageEffect)multi6x2).hits = 2; EditorUtility.SetDirty(multi6x2);
        var multi10x2 = MakeEffect<MultiHitDamageEffect>("EchoStrike_10x2"); ((MultiHitDamageEffect)multi10x2).damagePerHit = 10; ((MultiHitDamageEffect)multi10x2).hits = 2; EditorUtility.SetDirty(multi10x2);
        var lose10blk = MakeEffect<LoseBlockEffect>("EchoStrike_Lose10Block"); lose10blk.amount = 10; EditorUtility.SetDirty(lose10blk);
        var cursedEcho = ScriptableObject.CreateInstance<CardData>();
        cursedEcho.cardName = "Cursed Echo Strike";
        cursedEcho.cursedEffects = new CardEffect[] { multi6x2, lose10blk };
        cursedEcho.purifiedEffects = new CardEffect[] { multi6x2 };
        cursedEcho.blessedEffects = new CardEffect[] { multi10x2 };
        AssetDatabase.CreateAsset(cursedEcho, $"{cardsFolder}/Cursed Echo Strike.asset"); EditorUtility.SetDirty(cursedEcho);

        // Legendaries
        var pwrx5 = MakeEffect<PowerScalingDamageEffect>("Wrath_5xPower"); ((PowerScalingDamageEffect)pwrx5).baseDamage = 0; ((PowerScalingDamageEffect)pwrx5).perPower = 5; EditorUtility.SetDirty(pwrx5);
        var pwrx6 = MakeEffect<PowerScalingDamageEffect>("Wrath_6xPower"); ((PowerScalingDamageEffect)pwrx6).baseDamage = 0; ((PowerScalingDamageEffect)pwrx6).perPower = 6; EditorUtility.SetDirty(pwrx6);
        var removeAllBlock = MakeEffect<RemoveAllBlockEffect>("Wrath_RemoveAllBlock"); ((RemoveAllBlockEffect)removeAllBlock).affectOnlyPlayer = true; EditorUtility.SetDirty(removeAllBlock);
        var disableBlock = MakeEffect<DisableBlockThisTurnEffect>("Wrath_DisableBlock"); EditorUtility.SetDirty(disableBlock);
        var cursedWrath = ScriptableObject.CreateInstance<CardData>();
        cursedWrath.cardName = "Cursed Wrath";
        cursedWrath.cursedEffects = new CardEffect[] { pwrx5, disableBlock };
        cursedWrath.purifiedEffects = new CardEffect[] { pwrx5, removeAllBlock };
        cursedWrath.blessedEffects = new CardEffect[] { pwrx6 };
        AssetDatabase.CreateAsset(cursedWrath, $"{cardsFolder}/Cursed Wrath.asset"); EditorUtility.SetDirty(cursedWrath);

        var heal20 = MakeEffect<HealEffect>("SecondWind_Heal20"); heal20.amount = 20; EditorUtility.SetDirty(heal20);
        var heal10 = MakeEffect<HealEffect>("SecondWind_Heal10"); heal10.amount = 10; EditorUtility.SetDirty(heal10);
        var blk10 = MakeEffect<GainBlockEffect>("SecondWind_Block10"); blk10.block = 10; EditorUtility.SetDirty(blk10);
        var endTurn = MakeEffect<EndTurnImmediatelyEffect>("SecondWind_EndTurn"); EditorUtility.SetDirty(endTurn);
        var skip1turn = MakeEffect<SkipTurnsEffect>("SecondWind_Skip1"); ((SkipTurnsEffect)skip1turn).turns = 1; EditorUtility.SetDirty(skip1turn);
        var cursedSecondWind = ScriptableObject.CreateInstance<CardData>();
        cursedSecondWind.cardName = "Cursed Second Wind";
        cursedSecondWind.cursedEffects = new CardEffect[] { heal20, blk10, skip1turn };
        cursedSecondWind.purifiedEffects = new CardEffect[] { heal10, blk10, endTurn };
        cursedSecondWind.blessedEffects = new CardEffect[] { heal20, blk10 };
        AssetDatabase.CreateAsset(cursedSecondWind, $"{cardsFolder}/Cursed Second Wind.asset"); EditorUtility.SetDirty(cursedSecondWind);

        var aoe20 = MakeEffect<DealDamageAllEnemiesEffect>("Cataclysm_AOE20"); aoe20.damage = 20; EditorUtility.SetDirty(aoe20);
        var aoe25 = MakeEffect<DealDamageAllEnemiesEffect>("Cataclysm_AOE25"); aoe25.damage = 25; EditorUtility.SetDirty(aoe25);
        var self10_cat = MakeEffect<SelfDamageEffect>("Cataclysm_Self10"); self10_cat.damage = 10; EditorUtility.SetDirty(self10_cat);
        var cursedCataclysm = ScriptableObject.CreateInstance<CardData>();
        cursedCataclysm.cardName = "Cursed Cataclysm";
        cursedCataclysm.cursedEffects = new CardEffect[] { aoe20, self10_cat };
        cursedCataclysm.purifiedEffects = new CardEffect[] { aoe20 };
        cursedCataclysm.blessedEffects = new CardEffect[] { aoe25 };
        AssetDatabase.CreateAsset(cursedCataclysm, $"{cardsFolder}/Cursed Cataclysm.asset"); EditorUtility.SetDirty(cursedCataclysm);

        // Special
        var gamble50 = MakeEffect<KillOrSelfHarmEffect>("Gamble_50"); ((KillOrSelfHarmEffect)gamble50).killChance = 0.5f; EditorUtility.SetDirty(gamble50);
        var gamble60 = MakeEffect<KillOrSelfHarmEffect>("Gamble_60"); ((KillOrSelfHarmEffect)gamble60).killChance = 0.6f; EditorUtility.SetDirty(gamble60);
        var gamble80 = MakeEffect<KillOrSelfHarmEffect>("Gamble_80"); ((KillOrSelfHarmEffect)gamble80).killChance = 0.8f; EditorUtility.SetDirty(gamble80);
        var cursedGamble = ScriptableObject.CreateInstance<CardData>();
        cursedGamble.cardName = "Cursed Gamble";
        cursedGamble.cursedEffects = new CardEffect[] { gamble50 };
        cursedGamble.purifiedEffects = new CardEffect[] { gamble60 };
        cursedGamble.blessedEffects = new CardEffect[] { gamble80 };
        AssetDatabase.CreateAsset(cursedGamble, $"{cardsFolder}/Cursed Gamble.asset"); EditorUtility.SetDirty(cursedGamble);

        var flip50 = MakeEffect<ChanceGainPowerOrSelfDamageEffect>("Flip_50_2p_16d"); ((ChanceGainPowerOrSelfDamageEffect)flip50).gainChance = 0.5f; ((ChanceGainPowerOrSelfDamageEffect)flip50).powerAmount = 2; ((ChanceGainPowerOrSelfDamageEffect)flip50).selfDamage = 16; EditorUtility.SetDirty(flip50);
        var flip70 = MakeEffect<ChanceGainPowerOrSelfDamageEffect>("Flip_70_2p_16d"); ((ChanceGainPowerOrSelfDamageEffect)flip70).gainChance = 0.7f; ((ChanceGainPowerOrSelfDamageEffect)flip70).powerAmount = 2; ((ChanceGainPowerOrSelfDamageEffect)flip70).selfDamage = 16; EditorUtility.SetDirty(flip70);
        var flip80 = MakeEffect<ChanceGainPowerOrSelfDamageEffect>("Flip_80_3p_16d"); ((ChanceGainPowerOrSelfDamageEffect)flip80).gainChance = 0.8f; ((ChanceGainPowerOrSelfDamageEffect)flip80).powerAmount = 3; ((ChanceGainPowerOrSelfDamageEffect)flip80).selfDamage = 16; EditorUtility.SetDirty(flip80);
        var cursedFlip = ScriptableObject.CreateInstance<CardData>();
        cursedFlip.cardName = "Cursed Flip";
        cursedFlip.cursedEffects = new CardEffect[] { flip50 };
        cursedFlip.purifiedEffects = new CardEffect[] { flip70 };
        cursedFlip.blessedEffects = new CardEffect[] { flip80 };
        AssetDatabase.CreateAsset(cursedFlip, $"{cardsFolder}/Cursed Flip.asset"); EditorUtility.SetDirty(cursedFlip);

        var pact50 = MakeEffect<ConditionalHealOnLowHpEffect>("BloodPact_50"); ((ConditionalHealOnLowHpEffect)pact50).damageToDeal = 10; ((ConditionalHealOnLowHpEffect)pact50).healAmount = 10; ((ConditionalHealOnLowHpEffect)pact50).hpThreshold = 0.5f; EditorUtility.SetDirty(pact50);
        var pact60 = MakeEffect<ConditionalHealOnLowHpEffect>("BloodPact_60"); ((ConditionalHealOnLowHpEffect)pact60).damageToDeal = 10; ((ConditionalHealOnLowHpEffect)pact60).healAmount = 10; ((ConditionalHealOnLowHpEffect)pact60).hpThreshold = 0.6f; EditorUtility.SetDirty(pact60);
        var pact80 = MakeEffect<ConditionalHealOnLowHpEffect>("BloodPact_80"); ((ConditionalHealOnLowHpEffect)pact80).damageToDeal = 12; ((ConditionalHealOnLowHpEffect)pact80).healAmount = 12; ((ConditionalHealOnLowHpEffect)pact80).hpThreshold = 0.8f; EditorUtility.SetDirty(pact80);
        var cursedPact = ScriptableObject.CreateInstance<CardData>();
        cursedPact.cardName = "Cursed Blood Pact";
        cursedPact.cursedEffects = new CardEffect[] { pact50 };
        cursedPact.purifiedEffects = new CardEffect[] { pact60 };
        cursedPact.blessedEffects = new CardEffect[] { pact80 };
        AssetDatabase.CreateAsset(cursedPact, $"{cardsFolder}/Cursed Blood Pact.asset"); EditorUtility.SetDirty(cursedPact);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

public static class EditorFluentExtensions
{
	public static T Also<T>(this T instance, Action<T> configure) where T : UnityEngine.Object
	{
		if (instance == null) return null;
		configure?.Invoke(instance);
		#if UNITY_EDITOR
		EditorUtility.SetDirty(instance);
		#endif
		return instance;
	}
}


