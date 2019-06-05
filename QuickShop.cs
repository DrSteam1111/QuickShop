using UnityEngine;
using Harmony12;
using System.Reflection;
using UnityModManagerNet;

namespace QuickShop
{
    public static class Main
    {
        public static UnityModManager.ModEntry ModEntry;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;

            HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }
    }

    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("Update")]
    public class GameScript_Update_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!Input.GetKeyUp(KeyCode.B)) return;

            var id = GameScript.Get().GetPartMouseOver().GetIDWithTuned();
            if (id == null) return;

            BuyItem(id);
        }

        private static void BuyItem(string id)
        {
            Inventory.Get().Add(id, 1f, Color.black, true, true);
            var price = (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(id).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
            GlobalData.AddPlayerMoney(-price);
            Main.ModEntry.Logger.Log($"QuickShop: bought id: {id} for {price}.");
            UIManager.Get().ShowPopup("QuickShop:", "Part cost: " + Helper.MoneyToString((float) price), PopupType.Buy);
        }
    }
}
