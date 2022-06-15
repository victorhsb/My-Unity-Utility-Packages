using System.IO;
using UnityEngine;
using UnityEditor;

namespace voidling {
    public static class ToolsMenu {
		[MenuItem("tools/setup/create default directories")]
        public static void CreateDefaultDirectories() {
            Dirs("_Project", "Scripts", "Art", "Scenes", "Materials", "Prefabs");
		}

        static void Dirs(string root, params string[] directories) {
            var fullPath = Path.Combine(Application.dataPath, root);
			foreach (var dir in directories) {
                Directory.CreateDirectory(Path.Combine(fullPath, dir));
			}
            AssetDatabase.Refresh();
		}
    }
}
