using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Helper behavior which manages the debug UI. </summary>
public class DebugMenuUI : MonoBehaviour
{
#region Editor

    [ Header("Global") ]
    [Tooltip("Display the debug UI?")]
    public bool displayUI;
    
#endregion // Editor

#region Internal

    /// <summary> Dimensions of the main window. </summary>
    private static Vector2 WINDOW_DIMENSION = new Vector2(320.0f, 240.0f);
    /// <summary> Base padding used within the UI. </summary>
    private static float BASE_PADDING = 8.0f;

    /// <summary> Rectangle representing the screen drawing area. </summary>
    private Rect mScreenRect;
    /// <summary> Rectangle representing the main window. </summary>
    private Rect mMainWindowRect;

    /// <summary> Dummy value used for demonstration. </summary>
    private float mDummyValue = 0.0f;
    
#endregion // Internal

#region Interface

#endregion // Interface

    /// <summary> Called when the script instance is first loaded. </summary>
    private void Awake()
    { }

    /// <summary> Called before the first frame update. </summary>
    void Start()
    {
        // Deduce the drawing screen area from the main camera.
        var mainCamera = GameSettings.Instance.mainCamera;
        mScreenRect = new Rect(
            mainCamera.rect.x * Screen.width, 
            mainCamera.rect.y * Screen.height, 
            mainCamera.rect.width * Screen.width, 
            mainCamera.rect.height * Screen.height
        );
        // Initially place the debug window into the top right corner.
        mMainWindowRect = new Rect(
            mScreenRect.x + mScreenRect.width - WINDOW_DIMENSION.x, mScreenRect.y, 
            WINDOW_DIMENSION.x, WINDOW_DIMENSION.y
        );
    }

    /// <summary> Update called once per frame. </summary>
    void Update()
    { }

    /// <summary> Called when GUI drawing should be happening. </summary>
    private void OnGUI()
    {
        if (displayUI)
        { // Only draw the window if we are currently displaying it.
            mMainWindowRect = GUI.Window(0, mMainWindowRect, MainWindowUI, "Cheat Console");
            // Limit the window position to the screen area.
            mMainWindowRect.x = Mathf.Clamp(
                mMainWindowRect.x, mScreenRect.x, 
                mScreenRect.x + mScreenRect.width - WINDOW_DIMENSION.x
            );
            mMainWindowRect.y = Mathf.Clamp(
                mMainWindowRect.y, mScreenRect.y, 
                mScreenRect.y + mScreenRect.height - WINDOW_DIMENSION.y
            );
        }
    }

    /// <summary> Function used for drawing the main window. </summary>
    private void MainWindowUI(int windowId)
    {
        // Start the window drawing area, starting with some padding.
        GUILayout.BeginArea(new Rect(
            BASE_PADDING, 2.0f * BASE_PADDING, 
            WINDOW_DIMENSION.x - 2.0f * BASE_PADDING, 
            WINDOW_DIMENSION.y - 3.0f * BASE_PADDING
        ));
        { // Main Window Area
            
            
            // GUILayout allows us to automatically place UI elements after each other.
            // BeginVertical starts a vertical group, while BeginHorizontal a horizontal one.
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Currency: ", GUILayout.Width(WINDOW_DIMENSION.x / 4.0f));
                    int currency = InventoryManager.Instance != null ? InventoryManager.Instance.availableCurrency : 0;
                    currency = (int)GUILayout.HorizontalSlider(currency, 0.0f, 1000.0f, GUILayout.ExpandWidth(true));
                    if (GUI.changed && InventoryManager.Instance != null)
                    {
                        InventoryManager.Instance.availableCurrency = currency;
                    }
                }
                GUILayout.EndHorizontal();
            
                
                
                
                
                /*
                 * Task 3c: The Tool
                 *
                 * In this final task, you will be creating some utility debugging
                 * functions within our Cheat Console. In this case, you will have
                 * a little more autonomy.
                 *
                 * Create appropriate controls for the following handles:
                 *  * GameManager.Instance.interactiveMode: Allows mouse interaction
                 *    with the scene. Try it out - you can left-click scene objects
                 *    and drag.
                 *  * SoundManager.Instance.masterVolume: Master volume control for
                 *    all sounds. Its value is in dB, appropriate range <-80.0f, 20.0f>.
                 *  * SoundManager.Instance.masterMuted: Allows muting of the sound.
                 *
                 * For this task, it may be useful to look at what elements are available:
                 * https://docs.unity3d.com/2021.2/Documentation/Manual/gui-Controls.html
                 * , but nota that you will probably want to use GUILayout instead of "GUI".
                 *
                 * This task can be considered as completed once all three handles can
                 * be controlled from the Cheat Console.
                 */

                // Interactive mode toggle
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Interact", GUILayout.Width(WINDOW_DIMENSION.x / 4.0f));
                    bool interactive = GameManager.Instance != null ? GameManager.Instance.interactiveMode : false;
                    bool newInteractive = GUILayout.Toggle(interactive, "Enable Mouse", GUILayout.ExpandWidth(true));
                    if (GameManager.Instance != null &&
                    newInteractive != GameManager.Instance.interactiveMode)
                    {
                        GameManager.Instance.interactiveMode = newInteractive;
                    }
                }
                GUILayout.EndHorizontal();

                // Master volume slider (dB) with small 'm' mute toggle button
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Master Volume (dB):", GUILayout.Width(WINDOW_DIMENSION.x / 4.0f));
                    float volume = SoundManager.Instance != null ? SoundManager.Instance.masterVolume : 0.0f;
                    float newVolume = GUILayout.HorizontalSlider(volume, -80.0f, 20.0f, GUILayout.ExpandWidth(true));

                    // Apply volume change
                    if (SoundManager.Instance != null && !Mathf.Approximately(newVolume, SoundManager.Instance.masterVolume))
                    { SoundManager.Instance.masterVolume = newVolume; }

                    // Small mute button labeled 'm' (toggles masterMuted)
                    if (SoundManager.Instance != null)
                    {
                        GUILayout.Space(4);
                        if (GUILayout.Button(SoundManager.Instance.masterMuted ? "M" : "m", GUILayout.Width(24)))
                        { SoundManager.Instance.masterMuted = !SoundManager.Instance.masterMuted; }
                    }

                    GUILayout.Label(string.Format("{0:0.0} dB", newVolume), GUILayout.Width(56));
                }
                GUILayout.EndHorizontal();
                
                // Enable Dummy Character button
                if (GUILayout.Button("Enable Dummy Character", GUILayout.ExpandWidth(true)))
                {
                    if (GameManager.Instance != null)
                    { GameManager.Instance.TogglePlayerCharacter(); }
                }
                // Do not forget to end each group in the correct order!
            }
            GUILayout.EndVertical();
            // End the group!
            
            
        } // End of Main Window Area
        GUILayout.EndArea();
        
        // Allow dragging of the window by grabbing any part of it.
        GUI.DragWindow(new Rect(
            2.0f * BASE_PADDING,0.0f,
            WINDOW_DIMENSION.x - 4.0f * BASE_PADDING, 
            WINDOW_DIMENSION.y
        ));
    }
}
