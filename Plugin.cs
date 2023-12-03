using BepInEx;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
                var start = DateTime.Now;
                Logger.LogInfo("Reloading custom tilesets");
                DoReload();
                Logger.LogInfo($"Done, took {(DateTime.Now - start).Milliseconds}ms");
            }
        }

        void DoReload()
        {
            ReloadTileset();
            ReloadTextureScreen();
        }

        void ReloadTextureScreen()
        {
            MethodInfo reload = typeof(EditorTextureBrowser).GetMethod(
                "OnStart",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            EditorTextureBrowser[] instances = FindObjectsOfType<EditorTextureBrowser>();

            Debug.Assert(instances.Length == 1, "Too many texture browsers");

            EditorTextureBrowser instance = instances.First();
            foreach (
                var selector in instance.transform.GetComponentsInChildren<EditorTileSelector>()
            )
            {
                Destroy(selector.gameObject);
            }

            reload.Invoke(instance, new object[] { });
        }

        void ReloadTileset()
        {
            string path = LiveData.AdventureSelected.path + "\\tileset.png";
            if (File.Exists(path))
            {
                byte[] array = File.ReadAllBytes(path);
                Texture2D texture2D = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture2D, array);
                texture2D.filterMode = FilterMode.Point;
                LiveData.AdventureSelected.customTilesetTexture = texture2D;
            }

            LevelEditor[] editors = FindObjectsOfType<LevelEditor>();

            Debug.Assert(editors.Length == 0, "Found multiple editors.");

            LevelEditor editor = editors.First();

            Game.Defines.materialTileset.mainTexture = LiveData
                .AdventureSelected
                .customTilesetTexture;
            editor.atlasTileset.texture = Game.Defines.materialTileset.mainTexture;
        }
    }
}
