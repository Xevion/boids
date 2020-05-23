﻿using System;
using System.Net;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public Canvas canvas;

    public BoidController boidController;

    // Main Title Screen Elements
    public Button startButton;
    public Button settingsButton;
    public Button aboutButton;
    public RectTransform titleText;

    // About Screen Elements
    public RectTransform aboutPanel;
    public Button aboutCloseButton;

    // Adjustment Panel Elements
    public RectTransform adjPanel;

    // Settings Panel Elements
    public RectTransform settingsPanel;
    public Button settingsCloseButton;

    // Element Groups
    private GameObject[] _titleElements;

    private UIStance _currentUI;
    private CanvasScaler _scaler;
    private Rect _canvasRect;
    private float _scaleFactor;
    private int _activeTweens;

    private bool _UILock => _activeTweens > 0;

    // Element Displacements
    private Vector3 _aboutDiff;
    private Vector3 _aboutButtonDiff;
    private Vector3 _titleDiff;
    private Vector3 _settingsDiff;
    private Vector3 _adjustmentsDiff;

    private enum UIStance {
        Title,
        Play,
        Settings,
        About
    }

    private enum UIGroup {
        TitleScreen,
        AdjustmentsScreen,
        SettingsScreen,
        AboutScreen
    }

    private void Start() {
        // Basic variable setup
        _currentUI = UIStance.Title;
        _scaler = canvas.GetComponent<CanvasScaler>();
        _canvasRect = canvas.GetComponent<RectTransform>().rect;
        _scaleFactor = Screen.width / _scaler.referenceResolution.x;

        // Add stance change functionality to buttons
        startButton.onClick.AddListener(() => ChangeStance(UIStance.Play));
        aboutButton.onClick.AddListener(() => ChangeStance(UIStance.About));
        aboutCloseButton.onClick.AddListener(() => ChangeStance(UIStance.Title));
        settingsButton.onClick.AddListener(() => ChangeStance(UIStance.Settings));
        settingsCloseButton.onClick.AddListener(() => ChangeStance(UIStance.Title));

        // Calculate UI position deltas
        _aboutDiff = new Vector3(_canvasRect.size.x * _scaleFactor, 0, 0);
        _aboutButtonDiff = new Vector3(75 * _scaleFactor, 0, 0);
        _titleDiff = new Vector3(0, 450 * _scaleFactor, 0);
        _adjustmentsDiff = new Vector3(adjPanel.rect.size.x * _scaleFactor, 0, 0);
        _settingsDiff = new Vector3(_canvasRect.size.x * _scaleFactor * -1, 0, 0);

        // Move UI elements into position
        Vector3 center = canvas.transform.position;
        aboutPanel.transform.position = center + _aboutDiff;
        adjPanel.transform.position = center + new Vector3((_canvasRect.size.x + 10) / 2f * _scaleFactor, 0, 0) +
                                      _adjustmentsDiff / 2f;
        settingsPanel.transform.position = center + _settingsDiff;

        // Group Element Instantiation
        _titleElements = new[] {titleText.gameObject, startButton.gameObject, settingsButton.gameObject};
    }

    private void Update() {
        // on Escape key, attempts to change UI stance to the Title Screen
        if (Input.GetKeyDown(KeyCode.Escape))
            ChangeStance(UIStance.Title);
    }

    /// <summary>
    /// Signals to the application that a Tween has ended.
    /// This will unlock UI stance changes (if it was the last Tween within the stack).
    /// </summary>
    /// <seealso cref="StartTween"/>
    private void TweenEnd() {
        _activeTweens -= 1;
        if (!_UILock)
            Debug.Log("Unlocking Stance Changes");
    }

    /// <summary>
    /// Signals to the application that a Tween is in progress.
    /// This locks UI stance changes until all Tweens have completed.
    /// This method returns the <c>TweenEnd</c> callback, which must be used in conjunction with <c>StartTween</c>.
    /// </summary>
    /// <returns>returns the <see cref="TweenEnd"/> action for a deconstructing callback</returns>
    /// <seealso cref="TweenEnd"/>
    private Action StartTween() {
        if (!_UILock)
            Debug.Log("Locking Stance Changes");
        _activeTweens += 1;
        return TweenEnd;
    }


    /// <summary>
    /// Switches the 'Stance' a UI is in by moving groups of elements around.
    /// </summary>
    /// <param name="stance">The stance that the UI should switch to next.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ChangeStance(UIStance stance) {
        // Quit early if UI is currently "locked" due to an active tween
        if (_UILock || stance == _currentUI)
            return;

        Debug.Log($"UI Transition: {_currentUI} -> {stance}");
        
        // Title -> Settings/About/Play
        if (_currentUI == UIStance.Title) {
            MoveElements(UIGroup.TitleScreen, true);
            if (stance == UIStance.Settings)
                MoveElements(UIGroup.SettingsScreen, false);
            else if (stance == UIStance.About)
                MoveElements(UIGroup.AboutScreen, false);
            else if (stance == UIStance.Play)
                MoveElements(UIGroup.AdjustmentsScreen, false);
        }
        // Settings/About/Play -> Title
        else if (stance == UIStance.Title) {
            MoveElements(UIGroup.TitleScreen, false);
            if (_currentUI == UIStance.Play)
                MoveElements(UIGroup.AdjustmentsScreen, true);
            else if (_currentUI == UIStance.Settings)
                MoveElements(UIGroup.SettingsScreen, true);
            else if (_currentUI == UIStance.About)
                MoveElements(UIGroup.AboutScreen, true);
        }

        _currentUI = stance;
    }

    /// <summary>
    /// Moves groups of elements back and forth using the LeanTween framework.
    /// <c>MoveElements</c> is not aware of the current position of the elements at any time,
    /// thus measures must be implemented to track the current 'state' of any elements.
    ///
    /// Used in conjunction with <see cref="ChangeStance"/> to manipulate the UI elements.
    /// </summary>
    /// <param name="group">the <c>UIGroup</c> associated with the elements you wish to move</param>
    /// <param name="away">moves elements away (<c></c>)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void MoveElements(UIGroup group, bool away) {
        switch (group) {
            case UIGroup.TitleScreen:
                // Main Title and Start/Settings Buttons
                foreach (GameObject element in _titleElements) {
                    LeanTween
                        .move(element, element.transform.position + _titleDiff * (away ? 1 : -1), away ? 0.65f : 1.15f)
                        .setDelay(away ? 0f : 0.35f)
                        .setEase(away ? LeanTweenType.easeInCubic : LeanTweenType.easeOutCubic)
                        .setOnComplete(StartTween());
                }

                // Bottom Right About Button
                LeanTween
                    .move(aboutButton.gameObject, aboutButton.transform.position + _aboutButtonDiff * (away ? 1 : -1),
                        0.55f)
                    .setEase(LeanTweenType.easeInOutCubic);
                break;
            case UIGroup.AdjustmentsScreen:
                GameObject adjPanelGo;
                LeanTween
                    .move((adjPanelGo = adjPanel.gameObject),
                        adjPanelGo.transform.position + _adjustmentsDiff * (away ? 1 : -1), 1.15f)
                    .setDelay(away ? 0f : 0.15f)
                    .setEase(LeanTweenType.easeInOutCubic)
                    .setOnComplete(StartTween());
                break;
            case UIGroup.AboutScreen:
                LeanTween
                    .move(aboutPanel.gameObject, aboutPanel.transform.position + _aboutDiff * (away ? 1 : -1),
                        away ? 0.6f : 1f)
                    .setDelay(away ? 0f : 0.40f)
                    .setEase(away ? LeanTweenType.easeInCubic : LeanTweenType.easeOutCubic)
                    .setOnComplete(StartTween());
                break;
            case UIGroup.SettingsScreen:
                LeanTween
                    .move(settingsPanel.gameObject, settingsPanel.transform.position + _settingsDiff * (away ? 1 : -1),
                        away ? 0.7f : 1.1f)
                    .setDelay(away ? 0.05f : 0.15f)
                    .setEase(away ? LeanTweenType.easeInCubic : LeanTweenType.easeOutCubic)
                    .setOnComplete(StartTween());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(@group), @group, null);
        }
    }
}