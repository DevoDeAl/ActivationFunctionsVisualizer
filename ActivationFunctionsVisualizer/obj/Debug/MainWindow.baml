   M S B A M L   `   `   `  ????U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf  5   	   ! ActivationFunctionsVisualizer.   (ActivationFunctionsVisualizer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	     ? Title$G  AГрафик сравнения функций активации??5   	   $	??500??64   $
??1000??6A   .??5	      ? -   5         ?Name$
 grid??5	       ? RowDefinitions 5
   
   ?? 5      $
??235* ?6   ?? 5      $	??50* ?6   .+?5   
   #   LiveCharts.Wpf.CartesianChart  -   5        $??_chart??5      $??0??5      $??10q?5      
???????$??2q?5      &    LiveCharts.Wpf.Charts.Base.Chart   LegendLocation$	 Top=?5         Zoom$ Xy=?5         Series 5      ?? *Series+5      ? 5   
   -   5         ?Name$ 
stackPanel??5   -    ? Orientation$ 
Horizontal=?6   $??10q?6A   $??1??6M   5      5      