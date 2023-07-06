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
    private DropdownField _antiAliasingDropdown;

    private Button _backButton;

    private DropdownField _fullscreenModeDropdown;
    private VisualElement _graphicsMenu;
    private DropdownField _motionBlurDropdown;
    private VisualElement _optionsMenu;
    private DropdownField _qualityDropdown;
    private DropdownField _refreshRateDropdown;
    private DropdownField _resolutionDropdown;

    private VisualElement _root;
    private Settings _settings;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _optionsMenu = _root.Q<VisualElement>("options-menu");
        _graphicsMenu = _root.Q<VisualElement>("graphics-menu");

        _backButton = _root.Q<Button>("graphics-menu-back-button");
        _backButton.clicked += () =>
        {
            _optionsMenu.style.display = DisplayStyle.Flex;
            _graphicsMenu.style.display = DisplayStyle.None;
        };

        _settings = ReadSettings();

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
            // settings.motionBlur = 0;
            // settings.antiAliasing = 0;

            SaveSettings();
        }
#endif
        return settings;
    }

    private void SaveSettings()
    {
        // settings.MotionBlur = 
        // settings.AntiAliasing = 

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
        _fullscreenModeDropdown = _root.Q<DropdownField>("fullscreen-mode-dropdown");

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
    }

    private void InitializeResolutionDropdown()
    {
        _resolutionDropdown = _root.Q<DropdownField>("resolution-dropdown");
        _refreshRateDropdown = _root.Q<DropdownField>("refresh-rate-dropdown");

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

        // Set the initial value of the resolution dropdown based on the value in the settings object
        var currentResolution = _settings.resolution;
        var resolutionString = $"{currentResolution.width}x{currentResolution.height}";
        _resolutionDropdown.value = resolutionString;

        // Set the initial value of the refresh rate dropdown based on the value in the settings object
        var currentRefreshRate = currentResolution.refreshRateRatio.value;
        var refreshRateString = $"{currentRefreshRate}hz";
        _refreshRateDropdown.value = refreshRateString;
    }

    private void InitializeQualityDropdown()
    {
        _qualityDropdown = _root.Q<DropdownField>("quality-dropdown");

        _qualityDropdown.choices.Clear();

        foreach (var quality in QualitySettings.names) _qualityDropdown.choices.Add(quality);

        _qualityDropdown.RegisterValueChangedCallback(evt =>
        {
            var qualityName = evt.newValue;

            var qualityLevel = Array.IndexOf(QualitySettings.names, qualityName);

            QualitySettings.SetQualityLevel(qualityLevel);

            _settings.quality = qualityLevel;

            SaveSettings();
        });

        // Set the initial value of the quality dropdown based on the value in the settings object
        var currentQuality = _settings.quality;
        var qualityName = QualitySettings.names[currentQuality];
        _qualityDropdown.value = qualityName;
    }

    private void InitializeMotionBlurDropdown()
    {
        _motionBlurDropdown = _root.Q<DropdownField>("motion-blur-dropdown");

        _motionBlurDropdown.choices.Clear();

        // TODO: get the urp volume profile 
        VolumeProfile urpVolumeProfile = null;

        var components = urpVolumeProfile.components;

        MotionBlur motionBlur = null;

        foreach (var component in components)
            if (component is MotionBlur)
            {
                motionBlur = component as MotionBlur;
                break;
            }

        _motionBlurDropdown.choices.Add("Off");
        _motionBlurDropdown.choices.Add("Low Quality");
        _motionBlurDropdown.choices.Add("Medium Quality");
        _motionBlurDropdown.choices.Add("High Quality");
    }

    private void InitializeAntiAliasingDropdown()
    {
        _antiAliasingDropdown = _root.Q<DropdownField>("anti-aliasing-dropdown");

        _antiAliasingDropdown.choices.Clear();

        // AntialiasingMode antialiasingMode = AntialiasingMode.None;
        // AntialiasingMode antialiasingMode = AntialiasingMode.FastApproximateAntialiasing;
        // AntialiasingMode antialiasingMode = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        // AntialiasingMode antialiasingMode = AntialiasingMode.TemporalAntiAliasing;

        // AntialiasingQuality antialiasingQuality = AntialiasingQuality.Low;
        // AntialiasingQuality antialiasingQuality = AntialiasingQuality.Medium;
        // AntialiasingQuality antialiasingQuality = AntialiasingQuality.High;

        // add none to the dropdown
        _antiAliasingDropdown.choices.Add("None");
        _antiAliasingDropdown.choices.Add("SMAA");
        _antiAliasingDropdown.choices.Add("FXAA");
        _antiAliasingDropdown.choices.Add("TAA");

        // add msaa to the dropdown
        _antiAliasingDropdown.choices.Add("Off");
        _antiAliasingDropdown.choices.Add("MSAA 2x");
        _antiAliasingDropdown.choices.Add("MSAA 4x");
        _antiAliasingDropdown.choices.Add("MSAA 8x");
    }

    [Serializable]
    public struct Settings
    {
        [XmlElement] public FullScreenMode fullscreenMode;
        [XmlElement] public Resolution resolution;
        [XmlElement] public int quality;
        [XmlElement] public int motionBlur;
        [XmlElement] public int antiAliasing;
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