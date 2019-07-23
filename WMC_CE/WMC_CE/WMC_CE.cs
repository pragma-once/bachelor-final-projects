using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WMC_CE
{

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    class GUI
    {
        public GUI(MainWindow MainWindow, Action<bool> ToggleFullscreen)
        {
            ToggleFullscreen?.Invoke(Behavior.Settings.Fullscreen);
            CustomControls.GlobalSettings.Animations = false;
            CustomControls.GlobalSettings.DarkMode = Behavior.Settings.DarkMode;
            CustomControls.GlobalSettings.Animations = Behavior.Settings.Animations;

            MainGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new System.Windows.Thickness(20)
            };

            Grid SquareGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
            SquareGrid.Width = SquareGrid.Height = System.Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight);
            MainGrid.Children.Add(SquareGrid);

            Action<TextBox> AllowNumbersOnly = (TextBox textbox) =>
            {
                textbox.PreviewTextInput += (object sender, System.Windows.Input.TextCompositionEventArgs e) =>
                {
                    e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(
                            e.Text,
                            "^[0123456789]+$"
                        );
                };

                System.Windows.DataObject.AddPastingHandler(textbox, (object sender, System.Windows.DataObjectPastingEventArgs e) =>
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(
                            e.DataObject.GetData(typeof(string)).ToString(),
                            "^[0123456789]+$"
                        ))
                        e.CancelCommand();
                });
            };

            // Start --------------------------------

            Grid StartGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new System.Windows.Thickness(0)
            };

            TextBox MinSquaresTextBox = new TextBox
            {
                Text = Behavior.Settings.SquaresMinimumCount.ToString(),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(MinSquaresTextBox);

            AllowNumbersOnly(MinSquaresTextBox);

            TextBox MaxSquaresTextBox = new TextBox
            {
                Text = Behavior.Settings.SquaresMaximumCount.ToString(),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(MaxSquaresTextBox);

            AllowNumbersOnly(MaxSquaresTextBox);

            TextBox SequenceFilenameTextBox = new TextBox
            {
                Text = Behavior.Settings.SequenceFilename,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(SequenceFilenameTextBox);

            SequenceFilenameTextBox.PreviewTextInput += (object sender, System.Windows.Input.TextCompositionEventArgs e) =>
            {
                // List grabbed from https://en.wikipedia.org/wiki/Filename / \ ? % * : | " < >
                e.Handled = System.Text.RegularExpressions.Regex.IsMatch(
                        e.Text,
                        "^.*[/\\\\?%\\*:\\|\"\\<\\>]+.*$"
                    );
            };

            System.Windows.DataObject.AddPastingHandler(SequenceFilenameTextBox, (object sender, System.Windows.DataObjectPastingEventArgs e) =>
            {
                // List grabbed from https://en.wikipedia.org/wiki/Filename / \ ? % * : | " < >
                if (System.Text.RegularExpressions.Regex.IsMatch(
                        e.DataObject.GetData(typeof(string)).ToString(),
                        "^.*[/\\\\?%\\*:\\|\"\\<\\>]+.*$"
                    ))
                    e.CancelCommand();
            });

            TextBox SequenceLengthTextBox = new TextBox
            {
                Text = Behavior.Settings.SequenceLength.ToString(),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(SequenceLengthTextBox);

            AllowNumbersOnly(SequenceLengthTextBox);

            Label MinSquaresLabel = new Label()
            {
                Content = "Min Squares Count",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            StartGrid.Children.Add(MinSquaresLabel);

            Label MaxSquaresLabel = new Label()
            {
                Content = "Max Squares Count",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            StartGrid.Children.Add(MaxSquaresLabel);

            Label SequenceFilenameLabel = new Label()
            {
                Content = "Sequence Filename",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            StartGrid.Children.Add(SequenceFilenameLabel);

            Label SequenceLengthLabel = new Label()
            {
                Content = "Sequence Length",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            StartGrid.Children.Add(SequenceLengthLabel);

            CustomControls.Button LoadSequenceButton = new CustomControls.Button
            {
                Text = "Load Sequence"
            };
            LoadSequenceButton.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LoadSequenceButton.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(LoadSequenceButton.MainGrid);

            CustomControls.Button GenerateSequenceButton = new CustomControls.Button
            {
                Text = "Generate Sequence"
            };
            GenerateSequenceButton.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            GenerateSequenceButton.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(GenerateSequenceButton.MainGrid);

            CustomControls.Toggle DarkModeToggle = new CustomControls.Toggle("DarkMode", Behavior.Settings.DarkMode);
            DarkModeToggle.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            DarkModeToggle.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(DarkModeToggle.MainGrid);

            DarkModeToggle.OnToggle += (bool Value) =>
            {
                Behavior.Settings.DarkMode = Value;
                CustomControls.GlobalSettings.DarkMode = Value;
                Behavior.Settings.Save();
            };

            CustomControls.Toggle FullscreenToggle = new CustomControls.Toggle("Fullscreen", Behavior.Settings.Fullscreen);
            FullscreenToggle.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            FullscreenToggle.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(FullscreenToggle.MainGrid);

            FullscreenToggle.OnToggle += (bool Value) =>
            {
                Behavior.Settings.Fullscreen = Value;
                ToggleFullscreen?.Invoke(Value);
                Behavior.Settings.Save();
            };

            Label UsernameLabel = new Label()
            {
                Content = "Name",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            StartGrid.Children.Add(UsernameLabel);

            TextBox UsernameTextBox = new TextBox
            {
                Text = "N/A",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(UsernameTextBox);

            Label AgeLabel = new Label()
            {
                Content = "Age",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            StartGrid.Children.Add(AgeLabel);

            TextBox AgeTextBox = new TextBox
            {
                Text = "20",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(AgeTextBox);

            AllowNumbersOnly(AgeTextBox);

            CustomControls.Button StartButton = new CustomControls.Button
            {
                Text = "Start as " + UsernameTextBox.Text
            };
            StartButton.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            StartButton.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(StartButton.MainGrid);

            UsernameTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                if (UsernameTextBox.Text.Length > 8)
                    StartButton.Text = "Start as " + UsernameTextBox.Text.Substring(0, 8) + "...";
                else
                    StartButton.Text = "Start as " + UsernameTextBox.Text;
            };

            CustomControls.Button SaveRecordsButton = new CustomControls.Button
            {
                Text = "Save Records"
            };
            SaveRecordsButton.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            SaveRecordsButton.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(SaveRecordsButton.MainGrid);

            CustomControls.Button AboutButton = new CustomControls.Button
            {
                Text = "About"
            };
            AboutButton.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            AboutButton.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(AboutButton.MainGrid);

            CustomControls.Button ExitButton = new CustomControls.Button
            {
                Text = "Exit"
            };
            ExitButton.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            ExitButton.MainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            StartGrid.Children.Add(ExitButton.MainGrid);

            SquareGrid.Children.Add(StartGrid);

            // Testing --------------------------------

            Grid TestingGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Height = 0,
                Margin = new System.Windows.Thickness(0),
                Background = CustomControls.GlobalSettings.BG,
                Opacity = 0,
                Visibility = System.Windows.Visibility.Collapsed
            };

            Label CenterLabel = new Label
            {
                Content = "",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0),
                FontSize = Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight) > 10 ? Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight) / 24 : 1
            };
            TestingGrid.Children.Add(CenterLabel);

            List<Square> Squares = new List<Square>();

            void HideSquares()
            {
                foreach (var item in Squares)
                {
                    item.Rectangle.StrokeThickness = 0;
                    item.Brush.Color = Color.FromArgb(0, 0, 0, 0);
                    item.Stroke = SquareStrokeStyle.None;
                    item.Rectangle.Visibility = Visibility.Collapsed;
                }
            }

            void ExpandSquaresPool()
            {
                if (CurrentItem == null)
                    return;
                while (Squares.Count < CurrentItem.Item1.Count)
                {
                    Rectangle rec = new Rectangle
                    {
                        StrokeThickness = 0,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    SolidColorBrush brush = new SolidColorBrush();
                    rec.Fill = brush;
                    rec.Stroke = CustomControls.GlobalSettings.FG;
                    Squares.Add(new Square(rec, brush, new Point(0, 0), SquareStrokeStyle.None));
                    TestingGrid.Children.Add(rec);
                }
            }

            void UpdateSquareSizes()
            {
                if (CurrentItem == null)
                    return;
                double size =
                    CurrentItem.Item2
                    * (Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight) / 2)
                    * Behavior.Settings.SquaresSizeCoefficient;
                double thin_stroke_size = size / 24;
                double thick_stroke_size = size / 8;
                double W = MainGrid.ActualWidth / 2;
                double H = MainGrid.ActualHeight / 2;
                foreach (var item in Squares)
                {
                    if (item.Stroke == SquareStrokeStyle.None && item.Brush.Color.A == 0)
                        item.Rectangle.Visibility = Visibility.Collapsed;
                    else
                    {
                        item.Rectangle.Visibility = Visibility.Visible;
                        item.Rectangle.Margin = new Thickness(
                            -item.Position.X * W,
                            -item.Position.Y * H,
                            item.Position.X * W,
                            item.Position.Y * H
                        );
                        item.Rectangle.Width = size;
                        item.Rectangle.Height = size;
                        if (item.Stroke == SquareStrokeStyle.Thin) item.Rectangle.StrokeThickness = thin_stroke_size;
                        else if (item.Stroke == SquareStrokeStyle.Thick) item.Rectangle.StrokeThickness = thick_stroke_size;
                    }
                }
            }

            void ShowSolidSquares()
            {
                if (CurrentItem == null) { HideSquares(); return; }

                ExpandSquaresPool();

                for (int i = 0; i < CurrentItem.Item1.Count; i++)
                {
                    Squares[i].Stroke = SquareStrokeStyle.None;
                    Squares[i].Position = CurrentItem.Item1[i].Item1;
                    Squares[i].Brush.Color = CustomControls.ColorPicker.HueToColor(CurrentItem.Item1[i].Item2);
                }

                UpdateSquareSizes();
            }

            void ShowOutlinedSquares()
            {
                if (CurrentItem == null) { HideSquares(); return; }

                ExpandSquaresPool();

                for (int i = 0; i < CurrentItem.Item1.Count; i++)
                {
                    if (i == CurrentItem.Item3)
                        Squares[i].Stroke = SquareStrokeStyle.Thick;
                    else
                        Squares[i].Stroke = SquareStrokeStyle.Thin;
                    Squares[i].Position = CurrentItem.Item1[i].Item1;
                    Squares[i].Brush.Color = Color.FromArgb(0, 0, 0, 0);
                }

                UpdateSquareSizes();
            }

            CustomControls.ColorPicker ColorPicker = new CustomControls.ColorPicker();
            ColorPicker.MainGrid.HorizontalAlignment = HorizontalAlignment.Center;
            ColorPicker.MainGrid.VerticalAlignment = VerticalAlignment.Center;
            ColorPicker.MainGrid.Background = CustomControls.GlobalSettings.BG;
            TestingGrid.Children.Add(ColorPicker.MainGrid);

            double ColorPickerSize = 0.75;
            double ColorPickerOpacity = 0;

            void UpdateColorPicker()
            {
                ColorPicker.MainGrid.Width = ColorPicker.MainGrid.Height = ColorPickerSize * Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight) / 2;
                ColorPicker.MainGrid.Opacity = ColorPickerOpacity;
            }

            UpdateColorPicker();

            ColorPicker.MainGrid.Visibility = Visibility.Collapsed;

            CustomControls.Ptr<int> ColorPickerToken = new CustomControls.Ptr<int>(0);
            async Task ShowColorPicker()
            {
                ColorPicker.MainGrid.Visibility = Visibility.Visible;
                if (CustomControls.GlobalSettings.Animations)
                    await CustomControls.Animators.AnimateNatural(
                        ColorPickerToken,
                        (double t) =>
                        {
                            if (t >= 0)
                            {
                                ColorPickerOpacity = t;
                                ColorPickerSize = 0.9 + t * 0.1;
                                UpdateColorPicker();
                                CenterLabel.Opacity = 1 - t;
                            }
                        },
                        4, -1, 1
                    );
                else
                {
                    ColorPickerOpacity = 1;
                    ColorPickerSize = 1;
                    UpdateColorPicker();
                }
            }

            async Task HideColorPicker()
            {
                if (CustomControls.GlobalSettings.Animations)
                    await CustomControls.Animators.AnimateNatural(
                        ColorPickerToken,
                        (double t) =>
                        {
                            if (t >= 0)
                            {
                                ColorPickerOpacity = t;
                                ColorPickerSize = 0.9 + t * 0.1;
                                UpdateColorPicker();
                                CenterLabel.Opacity = 1 - t;
                            }
                        },
                        4, 1, -1
                    );
                else
                {
                    ColorPickerOpacity = 0;
                    ColorPickerSize = 0;
                    UpdateColorPicker();
                }
                ColorPicker.MainGrid.Visibility = Visibility.Collapsed;
                ColorPicker.SelectedHue = null;
            }

            MainGrid.Children.Add(TestingGrid);

            // Message (Error/Warning) --------------------------------

            Grid MessageGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Height = 0,
                Margin = new System.Windows.Thickness(0),
                Background = CustomControls.GlobalSettings.BG,
                Opacity = 0,
                Visibility = System.Windows.Visibility.Collapsed
            };

            Label MessageLabel = new Label()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new System.Windows.Thickness(0),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0)
            };
            MessageGrid.Children.Add(MessageLabel);

            MainGrid.Children.Add(MessageGrid);

            // Events --------------------------------

            Action<Grid, CustomControls.Ptr<int>> ShowGrid = (Grid grid, CustomControls.Ptr<int> Token) =>
            {
                grid.Visibility = System.Windows.Visibility.Visible;
                if (CustomControls.GlobalSettings.Animations)
                {
                    Animating = true;
                    CustomControls.Animators.AnimateNatural(Token, (double x) =>
                    {
                        grid.Height = MainGrid.ActualHeight * x;
                        grid.Opacity = x;
                        if (x == 1)
                        {
                            grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                            grid.Height = double.NaN;
                            Animating = false;
                        }
                    }, CustomControls.GlobalSettings.AnimationSpeed, grid.Opacity, 1);
                }
                else
                {
                    grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    grid.Height = double.NaN;
                    grid.Opacity = 1;
                }
            };

            Action<Grid, CustomControls.Ptr<int>> HideGrid = (Grid grid, CustomControls.Ptr<int> Token) =>
            {
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                grid.Height = MainGrid.ActualHeight;
                if (CustomControls.GlobalSettings.Animations)
                {
                    Animating = true;
                    CustomControls.Animators.AnimateNatural(Token, (double x) =>
                    {
                        grid.Height = MainGrid.ActualHeight * x;
                        grid.Opacity = x;
                        if (x == 0)
                        {
                            MessageGrid.Visibility = System.Windows.Visibility.Collapsed;
                            Animating = false;
                        }
                    }, CustomControls.GlobalSettings.AnimationSpeed, grid.Opacity, 0);
                }
                else
                {
                    grid.Height = 0;
                    grid.Opacity = 0;
                    grid.Visibility = System.Windows.Visibility.Collapsed;
                }
            };

            Action<string, int> ShowMessage = async (string Message, int Timeout) =>
            {
                MessageLabel.Content = Message;
                ShowGrid(MessageGrid, MessageToken);
                if (Timeout == 0) return;
                await Task.Delay(Timeout);
                if (MessageGrid.Visibility == System.Windows.Visibility.Visible)
                    HideGrid(MessageGrid, MessageToken);
            };

            LoadSequenceButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    if (SequenceFilenameTextBox.Text != "")
                    {
                        Behavior.Settings.SequenceFilename = SequenceFilenameTextBox.Text;
                        Behavior.Settings.Save();
                    }

                    Behavior.LoadSequence(SequenceFilenameTextBox.Text);
                }
                catch (Behavior.MessageException e)
                {
                    ShowMessage(e.Message, e.Message == "Sequence loaded!" ? CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 200 : 0);
                }
            };

            GenerateSequenceButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    if (MinSquaresTextBox.Text == "")
                        throw new Behavior.MessageException("Enter the min squares count!");
                    if (MaxSquaresTextBox.Text == "")
                        throw new Behavior.MessageException("Enter the max squares count!");
                    if (SequenceLengthTextBox.Text == "")
                        throw new Behavior.MessageException("Enter the sequence length!");

                    if (SequenceFilenameTextBox.Text != "")
                    {
                        Behavior.Settings.SquaresMinimumCount = Convert.ToInt32(MinSquaresTextBox.Text);
                        Behavior.Settings.SquaresMaximumCount = Convert.ToInt32(MaxSquaresTextBox.Text);
                        Behavior.Settings.SequenceFilename = SequenceFilenameTextBox.Text;
                        Behavior.Settings.SequenceLength = Convert.ToInt32(SequenceLengthTextBox.Text);
                        Behavior.Settings.Save();
                    }

                    Behavior.GenerateSequence(
                        SequenceFilenameTextBox.Text,
                        Convert.ToInt32(MinSquaresTextBox.Text),
                        Convert.ToInt32(MaxSquaresTextBox.Text),
                        Convert.ToInt32(SequenceLengthTextBox.Text));
                }
                catch (Behavior.MessageException e)
                {
                    ShowMessage(e.Message, e.Message == "Sequence generated!" ? CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 200 : 0);
                }
            };

            StartButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    Behavior.StartSequence(UsernameTextBox.Text, System.Convert.ToInt32(AgeTextBox.Text));
                    CenterLabel.Content = "Click anywhere or press enter to start.\n"
                                        + "Then find the color of the selected squares\n"
                                        + "when the color picker is shown.";
                    HideSquares();
                    ShowGrid(TestingGrid, TestingToken);
                }
                catch (Behavior.MessageException e) { ShowMessage(e.Message, 0); }
            };

            SaveRecordsButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    Behavior.SaveRecords();
                }
                catch (Behavior.MessageException e)
                {
                    ShowMessage(e.Message, e.Message == "Records saved!" ? CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 200 : 0);
                }
            };

            AboutButton.OnClick += () =>
            {
                if (Animating) return;
                ShowMessage("Developer: Majidzadeh (github.com/pragma-once)\n"
                            + "Developed as Bachelor's Degree Final Project.\n\n"
                            + "Test design grabbed from: DOI: 10.1038/nn.3655\n\n"
                            + "Instructors: Dr. Pedram, Dr. Moradi\n\n"
                            + "Kharazmi University, Spring 2019", 0);
            };

            ExitButton.OnClick += async () =>
            {
                if (Animating) return;
                try
                {
                    Exiting = true;
                    Behavior.SaveRecords();
                }
                catch (Behavior.MessageException e)
                {
                    ShowMessage(e.Message, 0);
                    await Task.Delay(CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 500);
                    MainWindow.Close();
                }
            };

            async void NextItem()
            {
                if (Animating) return;
                if (!Testing)
                {
                    Testing = true;
                    CenterLabel.Content = Behavior.Settings.CenterText;
                }
                if (ColorPicker.MainGrid.Visibility == Visibility.Visible)
                {
                    if (ColorPicker.SelectedHue == null)
                        ShowMessage("This is a bug! Please report.", 0);
                    else
                        Behavior.Respond((double)ColorPicker.SelectedHue);
                    await HideColorPicker();
                }

                CurrentItem = Behavior.GetNext();
                if (CurrentItem == null)
                {
                    ShowMessage("Test done.", 0);
                    bool Animations = CustomControls.GlobalSettings.Animations;
                    await Task.Delay(CustomControls.GlobalSettings.Animations ?
                                        CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 100
                                        : 100);
                    CustomControls.GlobalSettings.Animations = false;
                    HideGrid(TestingGrid, TestingToken);
                    CustomControls.GlobalSettings.Animations = Animations;
                    Testing = false;
                }
                else
                {
                    await Task.Delay(Behavior.Settings.WaitTime);
                    ShowSolidSquares();
                    await Task.Delay(Behavior.Settings.SampleTime);
                    HideSquares();
                    await Task.Delay(Behavior.Settings.DelayTime);
                    ShowOutlinedSquares();
                    await Task.Delay(Behavior.Settings.ProbeTime);
                    HideSquares();
                    ShowColorPicker();
                    Behavior.StartTimer();
                }
            }

            ColorPicker.OnClick += () =>  NextItem();

            // Layout --------------------------------
            MainGrid.SizeChanged += (object sender, System.Windows.SizeChangedEventArgs e) =>
            {
                SquareGrid.Width = SquareGrid.Height = System.Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight);

                double Half = SquareGrid.Height / 2;
                double Quarter = SquareGrid.Height / 4;
                double Eighth = SquareGrid.Height / 8;
                double Sixteenth = SquareGrid.Height / 16;
                double HalfAndQuarter = SquareGrid.Height * 0.75;
                double Gap = SquareGrid.Width / 32;
                double HalfGap = SquareGrid.Width / 64;
                double HalfG = Half + HalfGap;
                double QuarterG = Quarter + HalfGap;
                double HalfAndQuarterG = HalfAndQuarter + HalfGap;
                double H = SquareGrid.Height / 12;
                double HHalf = SquareGrid.Height / 18;
                double HQuarter = SquareGrid.Height / 32;

                // Start: Sequence options

                MinSquaresLabel.Margin = new System.Windows.Thickness(Gap, 0, HalfG, HalfAndQuarter + Eighth + Sixteenth);
                MinSquaresLabel.Height = HHalf;
                if (HHalf > 1) MinSquaresLabel.FontSize = HQuarter;

                MaxSquaresLabel.Margin = new System.Windows.Thickness(HalfG, 0, Gap, HalfAndQuarter + Eighth + Sixteenth);
                MaxSquaresLabel.Height = HHalf;
                if (HHalf > 1) MaxSquaresLabel.FontSize = HQuarter;

                MinSquaresTextBox.Margin = new System.Windows.Thickness(Gap, Sixteenth, HalfG, HalfAndQuarter + Eighth);
                MinSquaresTextBox.Height = HHalf;
                if (HHalf > 1) MinSquaresTextBox.FontSize = HQuarter;

                MaxSquaresTextBox.Margin = new System.Windows.Thickness(HalfG, Sixteenth, Gap, HalfAndQuarter + Eighth);
                MaxSquaresTextBox.Height = HHalf;
                if (HHalf > 1) MaxSquaresTextBox.FontSize = HQuarter;

                SequenceFilenameLabel.Margin = new System.Windows.Thickness(Gap, Eighth, HalfG, HalfAndQuarter + Sixteenth);
                SequenceFilenameLabel.Height = HHalf;
                if (HHalf > 1) SequenceFilenameLabel.FontSize = HQuarter;

                SequenceLengthLabel.Margin = new System.Windows.Thickness(HalfG, Eighth, Gap, HalfAndQuarter + Sixteenth);
                SequenceLengthLabel.Height = HHalf;
                if (HHalf > 1) SequenceLengthLabel.FontSize = HQuarter;

                SequenceFilenameTextBox.Margin = new System.Windows.Thickness(Gap, Eighth + Sixteenth, HalfG, HalfAndQuarter);
                SequenceFilenameTextBox.Height = HHalf;
                if (HHalf > 1) SequenceFilenameTextBox.FontSize = HQuarter;

                SequenceLengthTextBox.Margin = new System.Windows.Thickness(HalfG, Eighth + Sixteenth, Gap, HalfAndQuarter);
                SequenceLengthTextBox.Height = HHalf;
                if (HHalf > 1) SequenceLengthTextBox.FontSize = HQuarter;

                GenerateSequenceButton.MainGrid.Margin = new System.Windows.Thickness(HalfG, Quarter, Gap, Half + Eighth);
                GenerateSequenceButton.MainGrid.Height = H;

                LoadSequenceButton.MainGrid.Margin = new System.Windows.Thickness(Gap, Quarter, HalfG, Half + Eighth);
                LoadSequenceButton.MainGrid.Height = H;

                // Start: Toggles

                DarkModeToggle.MainGrid.Margin = new System.Windows.Thickness(HalfG, Quarter + Eighth, Gap, Half);
                DarkModeToggle.MainGrid.Height = H;

                FullscreenToggle.MainGrid.Margin = new System.Windows.Thickness(Gap, Quarter + Eighth, HalfG, Half);
                FullscreenToggle.MainGrid.Height = H;

                // Start: Name and Age

                UsernameTextBox.Margin = new System.Windows.Thickness(Gap, Half + Eighth, QuarterG, Quarter);
                UsernameTextBox.Height = H;
                if (HHalf > 1) UsernameTextBox.FontSize = HHalf;

                AgeTextBox.Margin = new System.Windows.Thickness(HalfAndQuarterG, Half + Eighth, Gap, Quarter);
                AgeTextBox.Height = H;
                if (HHalf > 1) AgeTextBox.FontSize = HHalf;

                UsernameLabel.Margin = new System.Windows.Thickness(Gap, Half, QuarterG, Quarter + Eighth);
                UsernameLabel.Height = H;
                if (HHalf > 1) UsernameLabel.FontSize = HHalf;

                AgeLabel.Margin = new System.Windows.Thickness(HalfAndQuarterG, Half, Gap, Quarter + Eighth);
                AgeLabel.Height = H;
                if (HHalf > 1) AgeLabel.FontSize = HHalf;

                // Start: Bottom buttons

                StartButton.MainGrid.Margin = new System.Windows.Thickness(Gap, HalfAndQuarter, HalfG, Eighth);
                StartButton.MainGrid.Height = H;

                SaveRecordsButton.MainGrid.Margin = new System.Windows.Thickness(HalfG, HalfAndQuarter, Gap, Eighth);
                SaveRecordsButton.MainGrid.Height = H;

                AboutButton.MainGrid.Margin = new System.Windows.Thickness(Gap, HalfAndQuarter + Eighth, HalfG, 0);
                AboutButton.MainGrid.Height = H;

                ExitButton.MainGrid.Margin = new System.Windows.Thickness(HalfG, HalfAndQuarter + Eighth, Gap, 0);
                ExitButton.MainGrid.Height = H;

                // Message

                if (HHalf > 1) MessageLabel.FontSize = HHalf;

                // Testing

                CenterLabel.FontSize = HHalf;
                UpdateSquareSizes();
                UpdateColorPicker();
            };

            MainWindow.KeyDown += (object sender, System.Windows.Input.KeyEventArgs e) =>
            {
                if (Animating) return;
                if (e.Key == System.Windows.Input.Key.F11
                        || (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)
                            && e.Key == System.Windows.Input.Key.F))
                {
                    FullscreenToggle.Value = !FullscreenToggle.Value;
                }
                if ((System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)
                        && e.Key == System.Windows.Input.Key.D))
                {
                    DarkModeToggle.Value = !DarkModeToggle.Value;
                }

                if (e.Key == System.Windows.Input.Key.Enter
                        && TestingGrid.Visibility == Visibility.Visible
                        && !Testing)
                {
                    NextItem();
                }
            };

            MainWindow.KeyUp += (object sender, System.Windows.Input.KeyEventArgs e) =>
            {
                if (Animating) { return; }
                if (Exiting)
                    MainWindow.Close();
                if (MessageGrid.Visibility == System.Windows.Visibility.Visible
                        && (e.Key == System.Windows.Input.Key.Enter
                            || e.Key == System.Windows.Input.Key.Space
                            || e.Key == System.Windows.Input.Key.Escape))
                    HideGrid(MessageGrid, MessageToken);
            };

            MainWindow.MouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                if (Animating) return;
                if (Exiting)
                    MainWindow.Close();
                if (MessageGrid.Visibility == System.Windows.Visibility.Visible)
                    HideGrid(MessageGrid, MessageToken);
                if (TestingGrid.Visibility == Visibility.Visible && !Testing)
                    NextItem();
            };
        }

        public Grid MainGrid { get; } = new Grid();

        public Behavior Behavior { get; } = new Behavior();

        bool Animating = false;
        bool Exiting = false;
        bool Testing = false;

        CustomControls.Ptr<int> StartToken = new CustomControls.Ptr<int>(0);
        CustomControls.Ptr<int> TestingToken = new CustomControls.Ptr<int>(0);
        CustomControls.Ptr<int> MessageToken = new CustomControls.Ptr<int>(0);

        Tuple<List<Tuple<Point, double>>, double, int, double> CurrentItem = null;
        enum SquareStrokeStyle : byte { None = 0, Thin = 1, Thick = 2 };
        class Square
        {
            public Square(Rectangle rec, SolidColorBrush brush, Point pos, SquareStrokeStyle stroke)
            {
                Rectangle = rec;
                Brush = brush;
                Position = pos;
                Stroke = stroke;
            }
            public Rectangle Rectangle;
            public SolidColorBrush Brush;
            public Point Position;
            public SquareStrokeStyle Stroke;
        }
    }

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    class Behavior
    {
        public class MessageException : Exception
        {
            public MessageException(string Message) : base(Message) { }
        }

        public Settings Settings { get; } = new Settings();

        public void GenerateSequence(string SequenceFilename, int MinSquares, int MaxSquares, int SequenceLength)
        {
            if (SequenceLength < 1) throw new MessageException("Minimum squares count should be more than zero.");
            if (MinSquares < 1) throw new MessageException("Minimum squares count should be more than zero.");
            if (MaxSquares < 1) throw new MessageException("Maximum squares count should be more than zero.");
            if (MinSquares > MaxSquares) throw new MessageException("Maximum squares count should be more than or equal to\n"
                                                                    + "the minimum squares count.");

            // Save the unsaved records
            bool UnsavedRecords = Records.Count > 0;
            if (UnsavedRecords)
                try
                {
                    SaveRecords();
                }
                catch (MessageException) { }

            // Generate
            int Seed = SeedGenerator.Next();

            // Save
            try
            {
                new XElement("Sequence",
                    new XElement("MinSquares", MinSquares),
                    new XElement("MaxSquares", MaxSquares),
                    new XElement("SequenceLength", SequenceLength),
                    new XElement("Seed", Seed)
                    ).Save(SequenceFilename);
            }
            catch (Exception e) { throw new MessageException("Could not save the generated sequence.\n" + e.Message); }

            // Apply
            this.MinSquares = MinSquares;
            this.MaxSquares = MaxSquares;
            this.SequenceLength = SequenceLength;
            this.Seed = Seed;

            // Cache
            CacheSequence();

            if (UnsavedRecords)
                throw new MessageException("Saved the records before generating the new sequence.");
            throw new MessageException("Sequence generated!");
        }

        public void LoadSequence(string SequenceFilename)
        {
            if (!System.IO.File.Exists(SequenceFilename))
                throw new MessageException("No sequence file with the name:\n" + SequenceFilename);

            // Save the unsaved records
            bool UnsavedRecords = Records.Count > 0;
            if (UnsavedRecords)
                try
                {
                    SaveRecords();
                }
                catch (MessageException) { }

            // Load
            int? MinSquares = null;
            int? MaxSquares = null;
            int? SequenceLength = null;
            int? Seed = null;
            try
            {
                XElement SequenceXML = XElement.Load(SequenceFilename);
                foreach (XElement element in SequenceXML.Elements())
                {
                    try
                    {
                        switch (element.Name.LocalName)
                        {
                            case "MinSquares":
                                MinSquares = Convert.ToInt32(element.Value);
                                break;
                            case "MaxSquares":
                                MaxSquares = Convert.ToInt32(element.Value);
                                break;
                            case "SequenceLength":
                                SequenceLength = Convert.ToInt32(element.Value);
                                break;
                            case "Seed":
                                Seed = Convert.ToInt32(element.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception e) { throw new MessageException("Couldn't read the XML file: " + e.Message); }

            if (MinSquares == null || MaxSquares == null || SequenceLength == null || Seed == null)
                throw new MessageException("Couldn't load, the sequence file is incomplete.");

            // Apply
            this.MinSquares = (int)MinSquares;
            this.MaxSquares = (int)MaxSquares;
            this.SequenceLength = (int)SequenceLength;
            this.Seed = (int)Seed;

            // Cache
            CacheSequence();

            if (UnsavedRecords)
                throw new MessageException("Saved the records before loading the new sequence.");
            throw new MessageException("Sequence loaded!");
        }

        void CacheSequence()
        {
            {
                int j = 2;
                for (int i = MinSquares; i <= MaxSquares; i++)
                    while (true)
                        if (j * j < i) j++;
                        else
                        {
                            // The space:
                            //     x: [-1, 1]
                            //     y: [-1, 1]
                            SquareSizes[i] = 1.0 / j;
                            break;
                        }
            }
            Sequence.Clear();
            Random SequenceGenerator = new Random(Seed);
            for (int i = 0; i < SequenceLength; i++)
            {
                int Count = SequenceGenerator.Next(MinSquares, MaxSquares + 1);
                int TargetIndex = SequenceGenerator.Next(0, Count);
                // The space:
                //     x: [-1, 1]
                //     y: [-1, 1]
                double Size = SquareSizes[Count];
                double BezelP = 1 - Size / 2;
                double BezelN = -BezelP;
                double FullArea = 2 - Size;
                double HalfArea = 1 - Size - 0.1; // 0.1 => The space for the middle '+' sign

                List<Tuple<Point, double>> Squares = new List<Tuple<Point, double>>(Count);
                double Hue = SequenceGenerator.NextDouble() * 6;
                double HueStep = 6 / Count;
                for (int j = 0; j < Count; j++)
                {
                    Squares.Add(new Tuple<Point, double>(new Point(), Hue));
                    Hue += HueStep;
                    if (Hue > 6) Hue -= 6;
                }
                for (int j = 0; j < Count; j++)
                    while (true)
                    {
                        bool HalfOnY = SequenceGenerator.NextDouble() < 0.5; // Or X axis
                        bool HalftOnNegativeSide = SequenceGenerator.NextDouble() < 0.5; // Or positive
                        Point pos = new Point(
                            HalftOnNegativeSide ?
                            BezelN + SequenceGenerator.NextDouble() * HalfArea
                            : BezelP - SequenceGenerator.NextDouble() * HalfArea
                            ,
                            BezelN + SequenceGenerator.NextDouble() * FullArea
                        );
                        if (HalfOnY)
                            pos = new Point(pos.Y, pos.X);
                        bool OK = true;
                        for (int k = 0; k < j; k++)
                            if (Math.Abs(pos.X - Squares[k].Item1.X) < Size
                                && Math.Abs(pos.Y - Squares[k].Item1.Y) < Size)
                            { OK = false; break; }
                        if (OK)
                        {
                            Squares[j] = new Tuple<Point, double>(pos, Squares[j].Item2);
                            break;
                        }
                    }

                Sequence.Add(new Tuple<List<Tuple<Point, double>>, double, int, double>(
                        Squares,
                        Size,
                        TargetIndex,
                        Squares[TargetIndex].Item2
                    ));
            }
        }

        public void StartSequence(string UserName, int Age)
        {
            if (Sequence.Count == 0)
                throw new MessageException("No sequence is loaded!\nGenerate or Load to start.");
            SequenceIndex = -1;
            if (Records.Count == 0 || Records[Records.Count - 1].Item3.Count == Sequence.Count)
                Records.Add(new Tuple<string, int, List<Tuple<double, double>>>(UserName, Age, new List<Tuple<double, double>>(Sequence.Count)));
        }

        public Tuple<List<Tuple<Point, double>>, double, int, double> GetNext()
        {
            if (Records[Records.Count - 1].Item3.Count <= SequenceIndex)
                throw new Exception("There is no given response for the last item to continue.\nThis is a bug! Please report.");
            StartTime = DateTime.Now;
            SequenceIndex++;
            if (SequenceIndex >= Sequence.Count)
                return null;
            return Sequence[SequenceIndex];
        }

        public void StartTimer()
        {
            StartTime = DateTime.Now;
        }

        public void Respond(double Hue)
        {
            if (Records[Records.Count - 1].Item3.Count > SequenceIndex)
                Records[Records.Count - 1].Item3[SequenceIndex] = new Tuple<double, double>(
                    Hue,
                    (DateTime.Now - StartTime).TotalSeconds
                );
            else Records[Records.Count - 1].Item3.Add(new Tuple<double, double>(
                    Hue,
                    (DateTime.Now - StartTime).TotalSeconds
                ));
        }

        public void SaveRecords()
        {
            if (Records.Count != 0 && Records[Records.Count - 1].Item3.Count != Sequence.Count)
                Records.RemoveAt(Records.Count - 1);
            if (Records.Count == 0)
                throw new MessageException("No records to save");

            // Prepare the files

            if (!System.IO.Directory.Exists("results"))
                System.IO.Directory.CreateDirectory("results");

            string SeqInfoFilename = DateTime.Now.Year.ToString() + "_"
                                + DateTime.Now.Month.ToString() + "_"
                                + DateTime.Now.Day.ToString() + "_"
                                + DateTime.Now.Hour.ToString() + "_"
                                + DateTime.Now.Minute.ToString() + "_"
                                + DateTime.Now.Second.ToString() + "_"
                                + DateTime.Now.Millisecond.ToString();
            string ResultFilename = SeqInfoFilename;

            SeqInfoFilename = "results/seq_info_" + SeqInfoFilename;
            ResultFilename = "results/result_" + ResultFilename;

            if (System.IO.File.Exists(SeqInfoFilename + ".csv"))
            {
                int i;
                for (i = 0; System.IO.File.Exists(SeqInfoFilename + "_" + i.ToString() + ".csv"); i++) ;
                SeqInfoFilename += i.ToString();
            }
            SeqInfoFilename += ".csv";

            if (System.IO.File.Exists(ResultFilename + ".csv"))
            {
                int i;
                for (i = 0; System.IO.File.Exists(ResultFilename + "_" + i.ToString() + ".csv"); i++) ;
                ResultFilename += i.ToString();
            }
            ResultFilename += ".csv";

            System.IO.FileStream file_s = System.IO.File.Create(SeqInfoFilename);
            file_s.Close();
            System.IO.FileStream file_r = System.IO.File.Create(ResultFilename);
            file_r.Close();

            // Prepare data
            List<double> AllowedErrors = new List<double>();
            double Lerp(double A, double B, double t) { return A + (B - A) * t; }
            foreach (var item in Sequence)
            {
                AllowedErrors.Add(
                    item.Item4 < 1 ? Lerp(Settings.UserErrorForRedHue,     Settings.UserErrorForYellowHue,  item.Item4) :
                    item.Item4 < 2 ? Lerp(Settings.UserErrorForYellowHue,  Settings.UserErrorForGreenHue,   item.Item4) :
                    item.Item4 < 3 ? Lerp(Settings.UserErrorForGreenHue,   Settings.UserErrorForAquaHue,    item.Item4) :
                    item.Item4 < 4 ? Lerp(Settings.UserErrorForAquaHue,    Settings.UserErrorForBlueHue,    item.Item4) :
                    item.Item4 < 5 ? Lerp(Settings.UserErrorForBlueHue,    Settings.UserErrorForMagentaHue, item.Item4) :
                                     Lerp(Settings.UserErrorForMagentaHue, Settings.UserErrorForRedHue,     item.Item4)
                );
            }

            // Prepare the content

            // Sequence info:
            // Squares Count, Target Hue
            string[,] SeqContentArray = new string[1 + Sequence.Count, 2];

            SeqContentArray[0, 0] = "SquaresCount";
            SeqContentArray[0, 1] = "TargetHue";
            for (int i = 0; i < Sequence.Count; i++)
            {
                SeqContentArray[i + 1, 0] = Sequence[i].Item1.Count.ToString();
                SeqContentArray[i + 1, 1] = Sequence[i].Item4.ToString();
            }

            // Responses report:
            // Name, Age, AcceptedCount, Accepted1, Accepted2, ..., Error1, Error2, ..., ReactionTime1, ...
            string[,] RepContentArray = new string[1 + Records.Count, 3 + Sequence.Count * 3];

            RepContentArray[0, 0] = "Name";
            RepContentArray[0, 1] = "Age";
            RepContentArray[0, 2] = "AcceptedCount";
            int offset1 = 3;
            int offset2 = 3 + Sequence.Count;
            int offset3 = 3 + Sequence.Count * 2;
            for (int j = 1; j <= Sequence.Count; j++)
            {
                RepContentArray[0, offset1 + j - 1] = "Accepted" + j.ToString();
                RepContentArray[0, offset2 + j - 1] = "Error" + j.ToString();
                RepContentArray[0, offset3 + j - 1] = "ReactionTime" + j.ToString();
            }
            int row = 1;
            for (int i = 0; i < Records.Count; i++)
            {
                RepContentArray[row, 0] = Records[i].Item1; // Name
                RepContentArray[row, 1] = Records[i].Item2.ToString(); // Age

                int count = 0;
                for (int j = 0; j < Sequence.Count; j++)
                {
                    double user_selection = Records[i].Item3[j].Item1;
                    double target = Sequence[j].Item4;
                    if (Math.Abs(user_selection + 6 - target) < Math.Abs(user_selection - target))
                        user_selection += 6;
                    else if (Math.Abs(user_selection - 6 - target) < Math.Abs(user_selection - target))
                        user_selection -= 6;
                    double user_error = user_selection - target;
                    bool accepted = Math.Abs(user_error) < AllowedErrors[j];

                    RepContentArray[row, offset1 + j] = accepted.ToString();
                    RepContentArray[row, offset2 + j] = user_error.ToString();
                    RepContentArray[row, offset3 + j] = Records[i].Item3[j].Item2.ToString();

                    if (accepted) count++;
                }

                RepContentArray[row, 2] = count.ToString();

                row++;
            }

            // Write

            string Content = "";

            if (Settings.RowPerRecordResultsOrientation)
                for (int i = 0; i < SeqContentArray.GetLength(0); i++)
                {
                    Content += SeqContentArray[i, 0];
                    for (int j = 1; j < SeqContentArray.GetLength(1); j++)
                        Content += ',' + SeqContentArray[i, j];
                    Content += '\n';
                }
            else
                for (int j = 0; j < SeqContentArray.GetLength(1); j++)
                {
                    Content += SeqContentArray[0, j];
                    for (int i = 1; i < SeqContentArray.GetLength(0); i++)
                        Content += ',' + SeqContentArray[i, j];
                    Content += '\n';
                }

            System.IO.File.WriteAllText(SeqInfoFilename, Content);

            Content = "";

            if (Settings.RowPerRecordResultsOrientation)
                for (int i = 0; i < RepContentArray.GetLength(0); i++)
                {
                    Content += RepContentArray[i, 0];
                    for (int j = 1; j < RepContentArray.GetLength(1); j++)
                        Content += ',' + RepContentArray[i, j];
                    Content += '\n';
                }
            else
                for (int j = 0; j < RepContentArray.GetLength(1); j++)
                {
                    Content += RepContentArray[0, j];
                    for (int i = 1; i < RepContentArray.GetLength(0); i++)
                        Content += ',' + RepContentArray[i, j];
                    Content += '\n';
                }

            System.IO.File.WriteAllText(ResultFilename, Content);

            Records.Clear();

            throw new MessageException("Records saved!");
        }

        Random SeedGenerator = new Random();

        int MinSquares = 0;
        int MaxSquares = 0;
        int Seed = 0;
        int SequenceLength = 0;

        int SequenceIndex = 0;
        DateTime StartTime;

        // Dictionary<SquaresCount, SquareSize>
        Dictionary<int, double> SquareSizes = new Dictionary<int, double>();
        // List<Tuple<List<Tuple<Point, Hue>>, SquaresSize, TargetIndex, TargetHue>>
        List<Tuple<List<Tuple<Point, double>>, double, int, double>> Sequence = new List<Tuple<List<Tuple<Point, double>>, double, int, double>>();
        // List<Tuple<name, Age, List<Tuple<Hue, ReactionTime>>>>
        List<Tuple<string, int, List<Tuple<double, double>>>> Records = new List<Tuple<string, int, List<Tuple<double, double>>>>();
    }

    public class Settings
    {
        const string SettingsFilename = "settings.xml";

        public bool Fullscreen;

        public bool DarkMode;
        public bool Animations;

        public int SquaresMinimumCount;
        public int SquaresMaximumCount;

        public string SequenceFilename;
        public int SequenceLength;

        public double UserErrorForRedHue;
        public double UserErrorForYellowHue;
        public double UserErrorForGreenHue;
        public double UserErrorForAquaHue;
        public double UserErrorForBlueHue;
        public double UserErrorForMagentaHue;

        public double SquaresSizeCoefficient;

        public int WaitTime;
        public int SampleTime;
        public int DelayTime;
        public int ProbeTime;

        public string CenterText;

        public bool RowPerRecordResultsOrientation;

        public Settings()
        {
            Reset();
            Load();
            Save();
        }

        public void Reset()
        {
            Fullscreen = true;

            DarkMode = false;
            Animations = true;

            SquaresMinimumCount = 4;
            SquaresMaximumCount = 5;

            SequenceFilename = "sequence_info.xml";
            SequenceLength = 10;

            UserErrorForRedHue = 0.5;
            UserErrorForYellowHue = 0.25;
            UserErrorForGreenHue = 0.5;
            UserErrorForAquaHue = 0.25;
            UserErrorForBlueHue = 0.5;
            UserErrorForMagentaHue = 0.25;

            SquaresSizeCoefficient = 1;

            WaitTime = 250;
            SampleTime = 750;
            DelayTime = 250;
            ProbeTime = 750;

            CenterText = "+";

            RowPerRecordResultsOrientation = true;
        }

        public void Load()
        {
            if (System.IO.File.Exists(SettingsFilename))
            {
                try
                {
                    XElement xml_settings = XElement.Load(SettingsFilename);
                    foreach (XElement element in xml_settings.Elements())
                        try
                        {
                            double tempD;
                            int tempI;
                            switch (element.Name.LocalName)
                            {
                                case "Fullscreen":
                                    Fullscreen = System.Convert.ToBoolean(element.Value);
                                    break;
                                case "DarkMode":
                                    DarkMode = System.Convert.ToBoolean(element.Value);
                                    break;
                                case "Animations":
                                    Animations = System.Convert.ToBoolean(element.Value);
                                    break;
                                case "SquaresMinimumCount":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    SquaresMinimumCount = tempI;
                                    break;
                                case "SquaresMaximumCount":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    SquaresMaximumCount = tempI;
                                    break;
                                case "SequenceFilename": // / \ ? % * : | " < >
                                    SequenceFilename = element.Value.Replace("/", "")
                                                                    .Replace("\\", "")
                                                                    .Replace("?", "")
                                                                    .Replace("%", "")
                                                                    .Replace("*", "")
                                                                    .Replace(":", "")
                                                                    .Replace("|", "")
                                                                    .Replace("\"", "")
                                                                    .Replace("<", "")
                                                                    .Replace(">", "");
                                    break;
                                case "SequenceLength":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    SequenceLength = tempI;
                                    break;
                                case "UserErrorForRedHue":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    if (tempD > 3)
                                        tempD = 3;
                                    UserErrorForRedHue = tempD;
                                    break;
                                case "UserErrorForYellowHue":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    if (tempD > 3)
                                        tempD = 3;
                                    UserErrorForYellowHue = tempD;
                                    break;
                                case "UserErrorForGreenHue":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    if (tempD > 3)
                                        tempD = 3;
                                    UserErrorForGreenHue = tempD;
                                    break;
                                case "UserErrorForAquaHue":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    if (tempD > 3)
                                        tempD = 3;
                                    UserErrorForAquaHue = tempD;
                                    break;
                                case "UserErrorForBlueHue":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    if (tempD > 3)
                                        tempD = 3;
                                    UserErrorForBlueHue = tempD;
                                    break;
                                case "UserErrorForMagentaHue":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    if (tempD > 3)
                                        tempD = 3;
                                    UserErrorForMagentaHue = tempD;
                                    break;
                                case "SquaresSizeCoefficient":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    SquaresSizeCoefficient = tempD;
                                    break;
                                case "WaitTime":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    WaitTime = tempI;
                                    break;
                                case "SampleTime":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    SampleTime = tempI;
                                    break;
                                case "DelayTime":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    DelayTime = tempI;
                                    break;
                                case "ProbeTime":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    ProbeTime = tempI;
                                    break;
                                case "CenterText":
                                    CenterText = element.Value;
                                    break;
                                case "RowPerRecordResultsOrientation":
                                    RowPerRecordResultsOrientation = System.Convert.ToBoolean(element.Value);
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception) { }
                }
                catch (Exception) { }
            }
        }

        public void Save()
        {
            XElement xml_settings = new XElement("Settings",
                new XElement("Fullscreen", Fullscreen),
                new XElement("DarkMode", DarkMode),
                new XElement("Animations", Animations),
                new XElement("SquaresMinimumCount", SquaresMinimumCount),
                new XElement("SquaresMaximumCount", SquaresMaximumCount),
                new XElement("SequenceFilename", SequenceFilename),
                new XElement("SequenceLength", SequenceLength),
                new XElement("UserErrorForRedHue", UserErrorForRedHue),
                new XElement("UserErrorForYellowHue", UserErrorForYellowHue),
                new XElement("UserErrorForGreenHue", UserErrorForGreenHue),
                new XElement("UserErrorForAquaHue", UserErrorForAquaHue),
                new XElement("UserErrorForBlueHue", UserErrorForBlueHue),
                new XElement("UserErrorForMagentaHue", UserErrorForMagentaHue),
                new XElement("SquaresSizeCoefficient", SquaresSizeCoefficient),
                new XElement("WaitTime", WaitTime),
                new XElement("SampleTime", SampleTime),
                new XElement("DelayTime", DelayTime),
                new XElement("ProbeTime", ProbeTime),
                new XElement("CenterText", CenterText),
                new XElement("RowPerRecordResultsOrientation", RowPerRecordResultsOrientation)
                );
            xml_settings.Save(SettingsFilename);
        }
    }
}
