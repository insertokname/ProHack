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
            ThemeManager.ThemeChanged += () => { ApplyThemeToChildrenRecursive(_currentParent); };
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
                ApplyThemeToChildrenRecursive(_currentParent);
            }
        }

        private void OnParentControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control != null)
            {
                ApplyThemeToChildrenRecursive(e.Control);
            }
        }

        public static void ApplyThemeToChildrenRecursive(object? obj, ThemeData? parentTheme = null)
        {
            if (obj is Control control)
            {
                ThemeData theme = MergeThemeData(parentTheme, ThemeSerializer.Deserialize(control.Tag));
                ApplyTheme(theme, control);

                foreach (Control child in control.Controls)
                {
                    ApplyThemeToChildrenRecursive(child, theme);
                }

                if (control is MenuStrip menuStrip)
                {
                    foreach (ToolStripItem menuItem in menuStrip.Items)
                    {
                        ApplyThemeToChildrenRecursive(menuItem, theme);
                    }
                }
                return;
            }

            if (obj is ToolStripItem item)
            {
                ThemeData theme = MergeThemeData(parentTheme, ThemeSerializer.Deserialize(item.Tag));
                ApplyTheme(
                    theme,
                    item);

                if (item is ToolStripDropDownItem dropDown && dropDown.HasDropDownItems)
                {
                    foreach (ToolStripItem child in dropDown.DropDownItems)
                    {
                        ApplyThemeToChildrenRecursive(child, theme);
                    }
                }
                return;
            }
        }

        private static ThemeData MergeThemeData(ThemeData? parentTheme, ThemeData childTheme)
        {
            return new ThemeData()
            {
                BackColor = parentTheme?.BackColor ?? childTheme.BackColor,
                ForeColor = parentTheme?.ForeColor ?? childTheme.ForeColor,
                Font = parentTheme?.Font ?? childTheme.Font,
                FontSize = parentTheme?.FontSize ?? childTheme.FontSize
            };
        }

        private static void ApplyTheme(
            ThemeData theme,
            dynamic target)
        {
            if (target is Control || target is ToolStripItem)
            {
                target.ForeColor = ThemeManager.SelectedTheme.GetColor(theme.ForeColor ?? ThemeData.DEFAULT_FORE_COLOR) ?? target.ForeColor;
                target.BackColor = ThemeManager.SelectedTheme.GetColor(theme.BackColor ?? ThemeData.DEFAULT_BACK_COLOR) ?? target.BackColor;

                var fontSize = ThemeManager.SelectedTheme.GetFontSize(theme.FontSize ?? ThemeData.DEFAULT_FONT_SIZE) ?? target.Font.Size;
                var font = ThemeManager.SelectedTheme.GetFont(theme.Font ?? ThemeData.DEFAULT_FONT) ?? target.Font;
                target.Font = new Font(font.FontFamily, fontSize, font.Style);
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
