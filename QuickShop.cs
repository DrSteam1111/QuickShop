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

            var id = IdOfSelectedItem();
            if (id == null) return;
            BuyItem(id);
        }

        private static string IdOfSelectedItem()
        {
            var id = GameScript.Get().GetPartMouseOver().GetID();
            if (id == null) return null;
            var possibleTunedId = "t_" + id;
            if (IdExists(possibleTunedId)) 
            {
                id = possibleTunedId;
            }
            return id;
        }

        private static bool IdExists(string possibleTunedId)
        {
            //TODO: is it a good enough indication that part doesn't exist?
            return Singleton<GameInventory>.Instance.GetItemProperty(possibleTunedId).Price != 0;
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
