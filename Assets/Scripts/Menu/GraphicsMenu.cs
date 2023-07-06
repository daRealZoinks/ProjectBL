using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class GraphicsMenu : MonoBehaviour
{
    private VisualElement _graphicsMenu;
    private VisualElement _optionsMenu;

    private DropdownField _fullscreenModeDropdown;
    private DropdownField _resolutionDropdown;
    private DropdownField _refreshRateDropdown;
    private DropdownField _qualityDropdown;
    private DropdownField _motionBlurDropdown;
    private DropdownField _antiAliasingDropdown;

    private Button _backButton;

    private Settings _settings;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _optionsMenu = root.Q<VisualElement>("options-menu");
        _graphicsMenu = root.Q<VisualElement>("graphics-menu");

        _fullscreenModeDropdown = root.Q<DropdownField>("fullscreen-mode-dropdown");
        _resolutionDropdown = root.Q<DropdownField>("resolution-dropdown");
        _refreshRateDropdown = root.Q<DropdownField>("refresh-rate-dropdown");
        _qualityDropdown = root.Q<DropdownField>("quality-dropdown");
        _motionBlurDropdown = root.Q<DropdownField>("motion-blur-dropdown");
        _antiAliasingDropdown = root.Q<DropdownField>("anti-aliasing-dropdown");

        _backButton = root.Q<Button>("graphics-menu-back-button");
        _backButton.clicked += () =>
        {
            _optionsMenu.style.display = DisplayStyle.Flex;
            _graphicsMenu.style.display = DisplayStyle.None;
        };

        _settings = ReadSettings();

        ApplySettings(_settings);

        InitializeFullscreenModeDropdown();
        InitializeResolutionDropdown();
        InitializeQualityDropdown();
        // InitializeMotionBlurDropdown();
        // InitializeAntiAliasingDropdown();
    }

    private Settings ReadSettings()
    {
        Settings settings = new();
        string filePath;
#if UNITY_STANDALONE_WIN
        filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                   "/My Games/ProjectBL/graphics.xml";
        XmlSerializer serializer = new(typeof(Settings));

        if (File.Exists(filePath))
        {
            using (var reader = XmlReader.Create(filePath))
            {
                settings = (Settings)serializer.Deserialize(reader);
            }
        }
        else
        {
            settings.fullscreenMode = Screen.fullScreenMode;
            settings.resolution = Screen.currentResolution;
            settings.quality = QualitySettings.GetQualityLevel();

            SaveSettings();
        }
#endif
        return settings;
    }

    private void ApplySettings(Settings settings)
    {
        Screen.SetResolution(settings.resolution.width, settings.resolution.height, settings.fullscreenMode, settings.resolution.refreshRateRatio);
        QualitySettings.SetQualityLevel(settings.quality);
    }

    private void SaveSettings()
    {
        string filePath;
#if UNITY_STANDALONE_WIN
        filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/ProjectBL/";
        XmlSerializer serializer = new(typeof(Settings));

        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

        using (var writer = XmlWriter.Create(filePath + "graphics.xml", new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(writer, _settings);
        }
#endif
    }

    private void InitializeFullscreenModeDropdown()
    {
        const string exclusiveFullscreen = "Exclusive Fullscreen";
        const string fullscreenWindow = "Fullscreen Window";
        const string maximizedWindow = "Maximized Window";
        const string windowed = "Windowed";

        _fullscreenModeDropdown.choices.Clear();

#if UNITY_STANDALONE_WIN
        _fullscreenModeDropdown.choices.Add(exclusiveFullscreen);
#endif
        _fullscreenModeDropdown.choices.Add(fullscreenWindow);
#if UNITY_STANDALONE_OSX
        _fullscreenModeDropdown.choices.Add(maximizedWindow);
#endif
        _fullscreenModeDropdown.choices.Add(windowed);

        switch (_settings.fullscreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                _fullscreenModeDropdown.value = exclusiveFullscreen;
                break;
            case FullScreenMode.FullScreenWindow:
                _fullscreenModeDropdown.value = fullscreenWindow;
                break;
            case FullScreenMode.MaximizedWindow:
                _fullscreenModeDropdown.value = maximizedWindow;
                break;
            case FullScreenMode.Windowed:
                _fullscreenModeDropdown.value = windowed;
                break;
        }

        _fullscreenModeDropdown.RegisterValueChangedCallback(evt =>
        {
            var fullscreenMode = evt.newValue;

            switch (fullscreenMode)
            {
                case exclusiveFullscreen:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    _settings.fullscreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case fullscreenWindow:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    _settings.fullscreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case maximizedWindow:
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    _settings.fullscreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case windowed:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    _settings.fullscreenMode = FullScreenMode.Windowed;
                    break;
            }

            SaveSettings();
        });
    }

    private void InitializeResolutionDropdown()
    {
        var resolutions = Screen.resolutions;

        HashSet<RefreshRate> refreshRates = new();
        HashSet<ScreenRatio> screenRatios = new();

        foreach (var resolution in resolutions)
        {
            refreshRates.Add(resolution.refreshRateRatio);
            screenRatios.Add(new ScreenRatio(resolution.width, resolution.height));
        }

        _resolutionDropdown.choices.Clear();
        _refreshRateDropdown.choices.Clear();

        foreach (var screenRatio in screenRatios)
            _resolutionDropdown.choices.Add($"{screenRatio.width}x{screenRatio.height}");

        foreach (var refreshRate in refreshRates) _refreshRateDropdown.choices.Add($"{refreshRate.value}hz");

        var settingsResolution = _settings.resolution;
        var resolutionString = $"{settingsResolution.width}x{settingsResolution.height}";
        _resolutionDropdown.value = resolutionString;

        var settingsRefreshRate = settingsResolution.refreshRateRatio.value;
        var refreshRateString = $"{settingsRefreshRate}hz";
        _refreshRateDropdown.value = refreshRateString;

        _resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            var resolution = evt.newValue;

            var width = int.Parse(resolution.Split('x')[0]);
            var height = int.Parse(resolution.Split('x')[1]);

            Screen.SetResolution(width, height, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);

            Resolution currentResolution = new();

            currentResolution.width = width;
            currentResolution.height = height;
            currentResolution.refreshRateRatio = Screen.currentResolution.refreshRateRatio;

            _settings.resolution = currentResolution;

            SaveSettings();
        });

        _refreshRateDropdown.RegisterValueChangedCallback(evt =>
        {
            var refreshRateValue = evt.newValue;

            var value = int.Parse(refreshRateValue.Split('h')[0]);

            // find the refreshRate with the same value
            foreach (var refreshRate in refreshRates)
                if (refreshRate.value == value)
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,
                        Screen.fullScreenMode, refreshRate);

                    Resolution currentResolution = new();

                    currentResolution.width = Screen.currentResolution.width;
                    currentResolution.height = Screen.currentResolution.height;
                    currentResolution.refreshRateRatio = refreshRate;

                    _settings.resolution = currentResolution;

                    break;
                }

            SaveSettings();
        });
    }

    private void InitializeQualityDropdown()
    {
        _qualityDropdown.choices.Clear();

        foreach (var quality in QualitySettings.names) _qualityDropdown.choices.Add(quality);

        var currentQuality = _settings.quality;
        var qualityName = QualitySettings.names[currentQuality];
        _qualityDropdown.value = qualityName;

        _qualityDropdown.RegisterValueChangedCallback(evt =>
        {
            var qualityName = evt.newValue;

            var qualityLevel = Array.IndexOf(QualitySettings.names, qualityName);

            QualitySettings.SetQualityLevel(qualityLevel);

            _settings.quality = qualityLevel;

            SaveSettings();
        });
    }

    [Serializable]
    public struct Settings
    {
        [XmlElement] public FullScreenMode fullscreenMode;
        [XmlElement] public Resolution resolution;
        [XmlElement] public int quality;
    }

    private struct ScreenRatio : IEquatable<ScreenRatio>
    {
        public readonly int width;
        public readonly int height;

        public ScreenRatio(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool Equals(ScreenRatio other)
        {
            return width == other.width && height == other.height;
        }
    }
}