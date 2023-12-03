using BepInEx;
using BepInEx.Configuration;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace TextureReloader
{
    [BepInPlugin("org.otterbee.texturereloader", "Texture Reloader", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
        ConfigEntry<KeyboardShortcut> reloadShortcut;

        void Awake()
        {
            reloadShortcut = Config.Bind(
                "Keybinds",
                "ReloadKeys",
                new KeyboardShortcut(KeyCode.R, KeyCode.LeftShift, KeyCode.LeftControl),
                "The keybind to reload the custom tileset"
            );

            Debug.Log("Loaded texture reloader");

            foreach (var go in FindObjectsOfType<GameObject>())
            {
                Logger.LogInfo(go.name);
            }
        }

        void Update()
        {
            if (reloadShortcut.Value.IsDown())
            {
                Logger.LogInfo("Reloading custom tilesets");
            }
        }

        void ReloadTileset() { }
    }
}
