using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls
{
    public interface IDataGridSettings
    {
        DataGridSettings DataGridSettings { get; set; }
        void Reload();
        void Save();
        void Reset();        
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct DataGridColumnInfo
    {
        public DataGridColumnInfo(DataGridColumn column)
        {
            IsVisible = column.Visibility == System.Windows.Visibility.Visible;
            DisplayIndex = column.DisplayIndex;
            if (column.Width.IsAuto)
                WidthValue = null;
            else
                WidthValue = column.Width.DisplayValue;
        }
        public int DisplayIndex;
        public bool IsVisible;
        public double? WidthValue;
        public void Apply(DataGridColumn column, int gridColumnCount)
        {
            column.Visibility = IsVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            var maxIndex = (gridColumnCount == 0) ? 0 : gridColumnCount - 1;
            column.DisplayIndex = (DisplayIndex <= maxIndex) ? DisplayIndex : maxIndex;
            if (column.CanUserResize)
            {
                if (WidthValue.HasValue)
                    column.Width = new DataGridLength(WidthValue.Value);
            }
            
        }
    }

    [Serializable]
    public class DataGridSettings
    {
        public Dictionary<string, DataGridColumnInfo[]> ColumnSettings
        {
            get;
            set;
        }

        public DataGridSettings()
        {
            ColumnSettings = new Dictionary<string, DataGridColumnInfo[]>();
        }
    }

    public class DataGridApplicationSettings : ApplicationSettingsBase, IDataGridSettings
    {
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public DataGridSettings DataGridSettings
        {
            get
            {
                if (this["DataGridSettings"] != null)
                {
                    return ((DataGridSettings)this["DataGridSettings"]);
                }
                return null;
            }
            set
            {
                this["DataGridSettings"] = value;
            }
        }
    }
}
