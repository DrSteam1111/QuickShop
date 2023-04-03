using UnityEngine;
using Harmony12;
using System.Reflection;
using UnityModManagerNet;

namespace QuickShop
{
    static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            modEntry.OnToggle = OnToggle;

            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            modEntry.Logger.Log("Starting QuickShop");

            return true;
        }
    }

    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("Update")]
    public class RotationInput_Update_Patcher
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            if (Input.GetKeyUp(KeyCode.B))
            {
                string id = GameScript.Get().GetPartMouseOver().GetIDWithTuned();
                if (id != null)
                {
                    Inventory.Get().Add(id, 1f, Color.black, true, true);
                    int price = Singleton<GameInventory>.Instance.GetItemProperty(id).Price;
                    GlobalData.AddPlayerMoney(-price);
                    UIManager.Get().ShowPopup("QiuckShop:", "Part cost: " + Helper.MoneyToString((float)price), PopupType.Buy);
                }
            }
        }
    }
}
