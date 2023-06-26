/// <summary>
/// 	This file contains a simple way to customize actions on play mode changes.
/// </summary>
/// <remarks>
/// 	Nothing to change in this file.
/// </remarks>

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UnityUnboared.Editor
{
    /// Defines the PlayMode enumeration
    public enum PlayModeState
    {
        Stopped,
        Playing,
        Paused,
        AboutToStop,
        AboutToPlay
    }

    /// <summary>
    /// 	This class allows developers to configure new behaviors
    /// 	when the play mode state changes.
    /// </summary>
    /// <example>
    /// 	<code>
    /// 		void OnCustomPlayModeStateChanged(PlayModeState currentMode, PlayModeState changedMode) {
    /// 			if ( currentMode == PlayModeState.Stopped && changedMode == PlayModeState.Playing ) {
    /// 				doSomethingOnStart();
    /// 			}
    /// 		}
    ///			
    /// 		PlayMode.PlayModeChanged += OnCustomPlayModeStateChanged
    /// 	</code>
    /// </example>
    [InitializeOnLoad]
    public class PlayMode
    {
        // The current play mode
        private static PlayModeState _currentState = PlayModeState.Stopped;

        // Event called when changing the play mode 
        public static event Action<PlayModeState, PlayModeState> PlayModeChanged;

        /// <summary>
        /// 	Instanciates PlayMode manager
        /// </summary>
        static PlayMode()
        {
            // Add event to the default playModeStateChanged event
            EditorApplication.playModeStateChanged += OnUnityPlayModeChanged;
            if (EditorApplication.isPaused)
                _currentState = PlayModeState.Paused;
        }

		/// <summary>
		/// 	Calls the custom function.
		/// </summary>
        private static void OnPlayModeChanged(
            PlayModeState currentState,
            PlayModeState changedState
        )
        {
            if (PlayModeChanged != null)
                PlayModeChanged(currentState, changedState);
        }

        //
        private static void OnUnityPlayModeChanged(
            PlayModeStateChange playModeState
        )
        {
            var changedState = PlayModeState.Stopped;

            int state = GetEditorAppStateBoolComb();

            switch (state)
            {
                case (22112):
                    changedState = PlayModeState.Playing;
                    break;
                case (21112):
                    changedState = PlayModeState.Paused;
                    break;
                case (22222):
                    changedState = PlayModeState.Stopped;
                    break;
                case (22122):
                    changedState = PlayModeState.AboutToStop;
                    break;
                case (21122):
                    changedState = PlayModeState.AboutToStop;
                    break;
                case (21222):
                    changedState = PlayModeState.Stopped;
                    break;
                case 22212:
                    changedState = PlayModeState.Stopped;
                    break;
                case 21212:
                    changedState = PlayModeState.Paused;
                    break;
                default:
                    // Debug.Log("No such state combination defined: " + state);
                    break;
            }

            // Fire PlayModeChanged event.
            if (_currentState != changedState)
                OnPlayModeChanged(_currentState, changedState);

            // Set current state.
            _currentState = changedState;
        }

        // Transform a boolean to an integer
        static int Bool2Int(bool b)
        {
            if (b)
                return 1;
            else
                return 2;
        }

        /// Get the application status as an integer.
        static int GetEditorAppStateBoolComb()
        {
            int b1 = Bool2Int(EditorApplication.isUpdating);
            int b2 = Bool2Int(EditorApplication.isPlayingOrWillChangePlaymode);
            int b3 = Bool2Int(EditorApplication.isPlaying);
            int b4 = Bool2Int(EditorApplication.isPaused);
            int b5 = Bool2Int(EditorApplication.isCompiling);
            return b1 + b2 * 10 + b3 * 100 + b4 * 1000 + b5 * 10000;
        }
    }
}
