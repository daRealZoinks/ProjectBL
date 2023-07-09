using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Menu
{
    public class GraphicsMenu : MonoBehaviour
    {
        private Button _backButton;

        private DropdownField _fullscreenModeDropdown;
        private VisualElement _graphicsMenu;
        private VisualElement _optionsMenu;
        private DropdownField _qualityDropdown;
        private DropdownField _refreshRateDropdown;
        private DropdownField _resolutionDropdown;

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
            root.Q<DropdownField>("motion-blur-dropdown");
            root.Q<DropdownField>("anti-aliasing-dropdown");

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
        }

        private Settings ReadSettings()
        {
            Settings settings = new();
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                           "/My Games/ProjectBL/graphics.xml";
            XmlSerializer serializer = new(typeof(Settings));

            if (File.Exists(filePath))
            {
                using var reader = XmlReader.Create(filePath);
                settings = (Settings)serializer.Deserialize(reader);
            }
            else
            {
                settings.fullscreenMode = Screen.fullScreenMode;
                settings.Resolution = Screen.currentResolution;
                settings.quality = QualitySettings.GetQualityLevel();

                SaveSettings();
            }

            return settings;
        }

        private static void ApplySettings(Settings settings)
        {
            Screen.SetResolution(settings.Resolution.width, settings.Resolution.height, settings.fullscreenMode,
                settings.Resolution.refreshRateRatio);
            QualitySettings.SetQualityLevel(settings.quality);
        }

        private void SaveSettings()
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/ProjectBL/";
            XmlSerializer serializer = new(typeof(Settings));

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            using var writer = XmlWriter.Create(filePath + "graphics.xml", new XmlWriterSettings { Indent = true });
            serializer.Serialize(writer, _settings);
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

            _fullscreenModeDropdown.value = _settings.fullscreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => exclusiveFullscreen,
                FullScreenMode.FullScreenWindow => fullscreenWindow,
                FullScreenMode.MaximizedWindow => maximizedWindow,
                FullScreenMode.Windowed => windowed,
                _ => _fullscreenModeDropdown.value
            };

            _fullscreenModeDropdown.RegisterValueChangedCallback(evt =>
            {
                var fullscreenModeString = evt.newValue;

                var fullscreenMode = fullscreenModeString switch
                {
                    exclusiveFullscreen => FullScreenMode.ExclusiveFullScreen,
                    fullscreenWindow => FullScreenMode.FullScreenWindow,
                    maximizedWindow => FullScreenMode.MaximizedWindow,
                    windowed => FullScreenMode.Windowed,
                    _ => throw new ArgumentOutOfRangeException()
                };

                Screen.fullScreenMode = fullscreenMode;
                _settings.fullscreenMode = fullscreenMode;

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
                _resolutionDropdown.choices.Add($"{screenRatio.Width}x{screenRatio.Height}");

            foreach (var refreshRate in refreshRates) _refreshRateDropdown.choices.Add($"{refreshRate.value}hz");

            var settingsResolution = _settings.Resolution;
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

                Resolution currentResolution = new()
                {
                    width = width,
                    height = height,
                    refreshRateRatio = Screen.currentResolution.refreshRateRatio
                };

                _settings.Resolution = currentResolution;

                SaveSettings();
            });

            _refreshRateDropdown.RegisterValueChangedCallback(evt =>
            {
                var refreshRateValue = evt.newValue;

                var value = int.Parse(refreshRateValue.Split('h')[0]);

                // find the refreshRate with the same value
                foreach (var refreshRate in refreshRates.Where(
                             refreshRate => Math.Abs(refreshRate.value - value) < 0.1f))
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,
                        Screen.fullScreenMode, refreshRate);

                    Resolution currentResolution = new()
                    {
                        width = Screen.currentResolution.width,
                        height = Screen.currentResolution.height,
                        refreshRateRatio = refreshRate
                    };

                    _settings.Resolution = currentResolution;

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
                var qualityLevelName = evt.newValue;
                var qualityLevel = Array.IndexOf(QualitySettings.names, qualityLevelName);
                QualitySettings.SetQualityLevel(qualityLevel);
                _settings.quality = qualityLevel;
                SaveSettings();
            });
        }

        [Serializable]
        public struct Settings
        {
            [XmlElement] public FullScreenMode fullscreenMode;
            [XmlElement] public Resolution Resolution;
            [XmlElement] public int quality;
        }

        private readonly struct ScreenRatio : IEquatable<ScreenRatio>
        {
            public readonly int Width;
            public readonly int Height;

            public ScreenRatio(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public bool Equals(ScreenRatio other)
            {
                return Width == other.Width && Height == other.Height;
            }
        }
    }
}