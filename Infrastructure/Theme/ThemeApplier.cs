using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Infrastructure.Theme
{
    /// <summary>
    /// A drop-in component that forces every control within its parent container to use
    /// a theme. Simply add it to any form in the designer to theme all existing and future controls.
    /// </summary>
    public sealed class ThemeApplier : Control
    {
        private Control? _currentParent;

        public ThemeApplier()
        {
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Visible = false;
            Enabled = false;
            Size = Size.Empty;
            ThemeManager.ThemeChanged += () => { ApplyThemeRecursive(_currentParent); };
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (_currentParent != null)
            {
                _currentParent.ControlAdded -= OnParentControlAdded;
            }

            _currentParent = Parent;

            if (_currentParent != null)
            {
                _currentParent.ControlAdded += OnParentControlAdded;
                ApplyThemeRecursive(_currentParent);
            }
        }

        private void OnParentControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control != null)
            {
                ApplyThemeRecursive(e.Control);
            }
        }

        public static void ApplyThemeRecursive(object? obj)
        {

            if (obj is Control control)
            {
                ApplyThemeColors(
                    control.Tag,
                    color => control.ForeColor = color,
                    color => control.BackColor = color,
                    font => control.Font = font);

                foreach (Control child in control.Controls)
                {
                    ApplyThemeRecursive(child);
                }

                if (control is MenuStrip menuStrip)
                {
                    foreach (ToolStripItem menuItem in menuStrip.Items)
                    {
                        ApplyThemeRecursive(menuItem);
                    }
                }

                return;
            }

            if (obj is ToolStripItem item)
            {
                ApplyThemeColors(
                    item.Tag,
                    color => item.ForeColor = color,
                    color => item.BackColor = color,
                    font => item.Font = font);

                if (item is ToolStripDropDownItem dropDown && dropDown.HasDropDownItems)
                {
                    foreach (ToolStripItem child in dropDown.DropDownItems)
                    {
                        ApplyThemeRecursive(child);
                    }
                }
            }
        }

        private static void ApplyThemeColors(
            object? tag,
            Action<Color> setForeColor,
            Action<Color> setBackColor,
            Action<Font> setFont)
        {
            ThemeData? theme = ThemeSerializer.Deserialize(tag);

            var foreColor = ThemeManager.SelectedTheme.GetColor(theme?.ForeColor);
            var backColor = ThemeManager.SelectedTheme.GetColor(theme?.BackColor);
            var font = ThemeManager.SelectedTheme.GetFont(theme?.Font, theme?.FontSize);

            if (foreColor.HasValue)
            {
                setForeColor(foreColor.Value);
            }

            if (backColor.HasValue)
            {
                setBackColor(backColor.Value);
            }

            if (font != null)
            {
                setFont(font!);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _currentParent != null)
            {
                _currentParent.ControlAdded -= OnParentControlAdded;
                _currentParent = null;
            }

            base.Dispose(disposing);
        }
    }
}
