/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Windows;

namespace QPP.Wpf.UI.Controls.Toolkit
{
  public class NumericUpDown : CommonNumericUpDown<double>
  {
    #region Constructors

    static NumericUpDown()
    {
      UpdateMetadata( typeof( NumericUpDown ), 1, double.MinValue, double.MaxValue );
    }

    public NumericUpDown()
        : base(Double.Parse, Decimal.ToDouble, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
    {
    }

    #endregion //Constructors

    #region Base Class Overrides

    protected override double IncrementValue(double value, double increment)
    {
      return value + increment;
    }

    protected override double DecrementValue(double value, double increment)
    {
      return value - increment;
    }

    #endregion //Base Class Overrides
  }
}
