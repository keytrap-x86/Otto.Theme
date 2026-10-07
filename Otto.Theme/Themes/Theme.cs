using Otto.Theme.Data.Enum;
using Otto.Theme.Tools.Helper;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Otto.Theme.Themes;

/// <summary>Loads Otto resources and switches between the Light and Dark palettes.</summary>
public class Theme : ResourceDictionary, ISupportInitialize
{
    private SkinType _skin;
    private ResourceDictionary _colorOverrides = new();

    public Theme() => UpdateResources();

    // XAML populates dictionary properties in place. Reimplement the interface
    // so color overrides are applied after all child resources have been read.
    public new void BeginInit() => base.BeginInit();
    public new void EndInit()
    {
        base.EndInit();
        UpdateResources();
    }

    // Kept for source compatibility with the original Theme dictionary API.
    public new Uri Source { get; set; }
    public string Name { get; set; }

    public virtual SkinType Skin
    {
        get => _skin;
        set
        {
            if (!System.Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (_skin == value) return;
            _skin = value;
            UpdateResources();
        }
    }

    /// <summary>Color token overrides applied to every palette. Call Refresh after editing the dictionary directly.</summary>
    public ResourceDictionary ColorOverrides
    {
        get => _colorOverrides;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _colorOverrides = value;
            UpdateResources();
        }
    }

    /// <summary>Updates a color token immediately and preserves it when Skin changes.</summary>
    public void SetColor(string key, Color color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        if (GetSkin(_skin)[key] is not Color)
            throw new ArgumentException($"Unknown color token: {key}", nameof(key));
        _colorOverrides[key] = color;
        UpdateResources();
    }

    public void Refresh() => UpdateResources();

    public virtual ResourceDictionary GetSkin(SkinType skinType) => ResourceHelper.GetSkin(skinType);
    public virtual ResourceDictionary GetTheme() => new()
    {
        Source = new Uri("pack://application:,,,/Otto.Theme;component/Themes/Theme.xaml")
    };

    private void UpdateResources()
    {
        var styles = GetTheme();
        var palette = GetSkin(_skin);
        foreach (System.Collections.DictionaryEntry entry in _colorOverrides)
        {
            if (entry.Key is not string key || palette[key] is not Color || entry.Value is not Color)
                throw new ArgumentException("ColorOverrides must contain existing Color token keys and Color values.");
            palette[entry.Key] = entry.Value;
        }
        styles.MergedDictionaries[0] = palette;
        // Recreate resource Freezables so their lookup context uses the active
        // palette. Controls and data remain in place when resources refresh.
        if (MergedDictionaries.Count == 0) MergedDictionaries.Add(styles);
        else MergedDictionaries[0] = styles;
    }
}
