/**
 ******************************************************************************
 * @file    LED Display Control Example/View/MainWindow.cs
 * @author  Adrian Wojcik  adrian.wojcik@put.poznan.pl
 * @version V1.0
 * @date    07-May-2021
 * @brief   LED display controller: main window with display GUI
 ******************************************************************************
 */

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;


namespace LedDisplayExample.View
{
    using ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Page
    { 
        private readonly LED_RGBViewModel viewmodel;
        //public bool is_Equal_led = false;

        public MainWindow()
        {
            InitializeComponent();

            viewmodel = new LED_RGBViewModel(SetButtonColor);
            DataContext = viewmodel;

            /* Button matrix grid definition */
            for (int i = 0; i < viewmodel.DisplaySizeX; i++)
            {
                ButtonMatrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
                ButtonMatrixGrid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
            }

            for (int i = 0; i < viewmodel.DisplaySizeY; i++)
            {
                ButtonMatrixGrid.RowDefinitions.Add(new RowDefinition());
                ButtonMatrixGrid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
            }

            for (int i = 0; i < viewmodel.DisplaySizeX; i++)
            {
                for (int j = 0; j < viewmodel.DisplaySizeY; j++)
                {
                    // <Button
                    Button led = new Button()
                    {
                        // Name = "LEDij"
                        Name = viewmodel.LedIndexToTag(i, j),
                        // CommandParameter = "LEDij"
                        CommandParameter = viewmodel.LedIndexToTag(i, j),
                        // Style="{StaticResource LedButtonStyle}"
                        Style = (Style)FindResource("LedButtonStyle"),
                        // Bacground="{StaticResource ... }"
                        Background = new SolidColorBrush(viewmodel.DisplayOffColor),
                        // BorderThicness="2"
                        BorderThickness = new Thickness(2),
                    };
                    // Command="{Binding CommonButtonCommand}" 
                    led.SetBinding(Button.CommandProperty, new Binding("CommonButtonCommand"));
                    // Grid.Column="i" 
                    Grid.SetColumn(led, i);
                    // Grid.Row="j"
                    Grid.SetRow(led, j);
                    // />

                    ButtonMatrixGrid.Children.Add(led);
                    ButtonMatrixGrid.RegisterName(led.Name, led);

                    
                }
            }

            
            if (!LedDisplayExample.ViewModel.LED_RGBViewModel.is_equal_led) {
                btnSEND.Background = Brushes.Red;
            }
        }

        private void SetButtonColor(string name, Color color)
        {
             (ButtonMatrixGrid.FindName(name) as Button).Background = new SolidColorBrush(color);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LedDisplayExample.ViewModel.LED_RGBViewModel.is_equal_led = true;
            btnSEND.Background = Brushes.Green;
        }
  
    }
}
