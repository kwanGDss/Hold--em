using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace SimplePoker.Systems
{
    // Ensures the correct EventSystem input module is enabled based on project input settings.
    public static class EventSystemInputFix
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ApplyAfterSceneLoad()
        {
            FixAllEventSystems();
            SceneManager.sceneLoaded += (_, __) => FixAllEventSystems();
        }

        private static void FixAllEventSystems()
        {
            var eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var eventSystem in eventSystems)
            {
                FixEventSystem(eventSystem);
            }
        }

        private static void FixEventSystem(EventSystem eventSystem)
        {
            if (eventSystem == null)
                return;

            var standalone = eventSystem.GetComponent<StandaloneInputModule>();
#if ENABLE_INPUT_SYSTEM
            var inputSystem = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputSystem == null)
                inputSystem = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
#endif

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (standalone != null)
                standalone.enabled = false;
#if ENABLE_INPUT_SYSTEM
            if (inputSystem != null)
                inputSystem.enabled = true;
#endif
#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            if (standalone == null)
                standalone = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            standalone.enabled = true;
#if ENABLE_INPUT_SYSTEM
            if (inputSystem != null)
                inputSystem.enabled = false;
#endif
#else
            // If both are enabled, prefer StandaloneInputModule to match existing UI setup.
            if (standalone == null)
                standalone = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            standalone.enabled = true;
#if ENABLE_INPUT_SYSTEM
            if (inputSystem != null)
                inputSystem.enabled = false;
#endif
#endif
        }
    }
}
