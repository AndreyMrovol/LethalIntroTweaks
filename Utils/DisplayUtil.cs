using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace IntroTweaks.Utils {
    internal class DisplayUtil {
        public static List<DisplayInfo> Displays { get; private set; } = [];

        internal static void Move(int displayIndex) {
            // Update `Displays` with current layout.
            Screen.GetDisplayLayout(Displays);

            Plugin.Logger.LogDebug($"Selected monitor: {displayIndex}");

            string layoutStr = string.Join("\n", Displays.Select(d => d.name + " - " + d.refreshRate));
            Plugin.Logger.LogDebug($"Current display layout:\n{layoutStr}");

            // No need to move if same monitor.
            int currentMonitor = Displays.IndexOf(Screen.mainWindowDisplayInfo);
            if (displayIndex == currentMonitor) return;

            MoveWindowAsync(displayIndex);
        }

        static async void MoveWindowAsync(int index) {
            await MoveWindowTask(index);
        }

        static async Task MoveWindowTask(int index) {
            // Out of bounds, end the task.
            if (index >= Displays.Count) {
                await Task.CompletedTask;

                Plugin.Logger.LogDebug("Display index out of bounds for current layout!");
                return;
            }

            #region Grab display and position
            DisplayInfo display = Displays[index];
            Vector2Int screenPos = new(0, 0); // Top left

            if (Screen.fullScreenMode != FullScreenMode.Windowed) {
                screenPos.x += display.width / 2;
                screenPos.y += display.height / 2;
            }
            #endregion

            AsyncOperation operation = Screen.MoveMainWindowTo(display, screenPos);
            while (operation.progress < 1f) {
                await Task.Yield();
            }
        }
    }
}
