using UnityEngine;
using Harmony12;
using System.Reflection;
using UnityModManagerNet;

namespace QuickShop
{
    public static class Main
    {
        public static bool Enabled;
        public static UnityModManager.ModEntry Mod;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Mod = modEntry;
            modEntry.OnToggle = OnToggle;

            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            modEntry.Logger.Log("Starting QuickShop");

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
            var price = Singleton<GameInventory>.Instance.GetItemProperty(id).Price;
            GlobalData.AddPlayerMoney(-price);
            UIManager.Get().ShowPopup("QuickShop:", "Part cost: " + Helper.MoneyToString((float) price), PopupType.Buy);
        }
    }
}
