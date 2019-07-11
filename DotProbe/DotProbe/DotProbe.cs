using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace DotProbe
{

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    public class DotProbeGUI
    {
        public DotProbeGUI(MainWindow MainWindow, Action<bool> ToggleFullscreen)
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

            TextBox SequenceFilenameTextBox = new TextBox
            {
                Text = "sequence.xml",
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
                Text = "20",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Background = CustomControls.GlobalSettings.BG,
                Foreground = CustomControls.GlobalSettings.FG
            };
            StartGrid.Children.Add(SequenceLengthTextBox);

            AllowNumbersOnly(SequenceLengthTextBox);

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
                Content = "Seq Items Count",
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

            // DotProbe --------------------------------

            Grid DotProbeGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Height = 0,
                Margin = new System.Windows.Thickness(0),
                Background = CustomControls.GlobalSettings.BG,
                Opacity = 0,
                Visibility = System.Windows.Visibility.Collapsed
            };

            Label CenterLabel = new Label
            {
                Content = "Press any key to start",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0),
                FontSize = Behavior.Settings.FontSize
            };
            if (Behavior.Settings.FontFamily != "")
                CenterLabel.FontFamily = new System.Windows.Media.FontFamily(Behavior.Settings.FontFamily);
            DotProbeGrid.Children.Add(CenterLabel);

            Label Word1 = new Label
            {
                Content = "",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0),
                Margin = Behavior.Settings.VerticalSides ?
                            new System.Windows.Thickness(0, -Behavior.Settings.SidesOffset, 0, 0)
                            : new System.Windows.Thickness(-Behavior.Settings.SidesOffset, 0, 0, 0),
                FontSize = Behavior.Settings.FontSize
            };
            if (Behavior.Settings.FontFamily != "")
                Word1.FontFamily = new System.Windows.Media.FontFamily(Behavior.Settings.FontFamily);
            DotProbeGrid.Children.Add(Word1);

            Label Word2 = new Label
            {
                Content = "",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = CustomControls.GlobalSettings.FG,
                Padding = new System.Windows.Thickness(0),
                Margin = Behavior.Settings.VerticalSides ?
                            new System.Windows.Thickness(0, 0, 0, -Behavior.Settings.SidesOffset)
                            : new System.Windows.Thickness(0, 0, -Behavior.Settings.SidesOffset, 0),
                FontSize = Behavior.Settings.FontSize
            };
            if (Behavior.Settings.FontFamily != "")
                Word2.FontFamily = new System.Windows.Media.FontFamily(Behavior.Settings.FontFamily);
            DotProbeGrid.Children.Add(Word2);

            System.Windows.Shapes.Ellipse Dot1 = new System.Windows.Shapes.Ellipse
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                StrokeThickness = 0,
                Stroke = CustomControls.GlobalSettings.FG,
                Fill = CustomControls.GlobalSettings.FG,
                Margin = Behavior.Settings.VerticalSides ?
                            new System.Windows.Thickness(0, -Behavior.Settings.SidesOffset, 0, 0)
                            : new System.Windows.Thickness(-Behavior.Settings.SidesOffset, 0, 0, 0),
                Width = Behavior.Settings.DotSize,
                Height = Behavior.Settings.DotSize
            };
            DotProbeGrid.Children.Add(Dot1);

            System.Windows.Shapes.Ellipse Dot2 = new System.Windows.Shapes.Ellipse
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                StrokeThickness = 0,
                Stroke = CustomControls.GlobalSettings.FG,
                Fill = CustomControls.GlobalSettings.FG,
                Margin = Behavior.Settings.VerticalSides ?
                            new System.Windows.Thickness(0, 0, 0, -Behavior.Settings.SidesOffset)
                            : new System.Windows.Thickness(0, 0, -Behavior.Settings.SidesOffset, 0),
                Width = Behavior.Settings.DotSize,
                Height = Behavior.Settings.DotSize
            };
            DotProbeGrid.Children.Add(Dot2);

            MainGrid.Children.Add(DotProbeGrid);

            if (Behavior.Settings.CustomBeepFilename != "") try
            {
                CustomBeep = new MediaPlayer();
                CustomBeep.Open(new Uri(Behavior.Settings.CustomBeepFilename, UriKind.RelativeOrAbsolute));
                UseCustomBeep = true;
            }
            catch (Exception) { }

            // Message (Error/Warning) --------------------------------

            Grid MessageGrid = new Grid
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
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
                    Behavior.LoadSequence(SequenceFilenameTextBox.Text);
                }
                catch (DotProbeBehavior.MessageException e)
                {
                    ShowMessage(e.Message, e.Message == "Sequence loaded!" ? CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 200 : 0);
                }
            };

            GenerateSequenceButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    if (SequenceLengthTextBox.Text == "")
                        throw new DotProbeBehavior.MessageException("Enter the sequence count!");
                    Behavior.GenerateSequence(SequenceFilenameTextBox.Text, System.Convert.ToInt32(SequenceLengthTextBox.Text));
                }
                catch (DotProbeBehavior.MessageException e)
                {
                    ShowMessage(e.Message, e.Message == "Sequence generated!" ? CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 200 : 0 );
                }
            };

            StartButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    Behavior.StartSequence(UsernameTextBox.Text, System.Convert.ToInt32(AgeTextBox.Text));
                    CenterLabel.Content = "Press Enter/Spacebar/" + Behavior.Settings.KeyForDotPosition1 + "/" + Behavior.Settings.KeyForDotPosition2 + " to start.\n"
                                        + "Then:\n"
                                        + "Press the " + Behavior.Settings.KeyForDotPosition1 + " key when the dot is at the "
                                            + (Behavior.Settings.VerticalSides ? "top.\n" : "left side.\n")
                                        + "Press the " + Behavior.Settings.KeyForDotPosition2 + " key when the dot is at the "
                                            + (Behavior.Settings.VerticalSides ? "bottom." : "right side.");
                    Word1.Content = "";
                    Word2.Content = "";
                    Dot1.Visibility = System.Windows.Visibility.Hidden;
                    Dot2.Visibility = System.Windows.Visibility.Hidden;
                    ShowGrid(DotProbeGrid, DotProbeToken);
                }
                catch (DotProbeBehavior.MessageException e) { ShowMessage(e.Message, 0); }
            };

            SaveRecordsButton.OnClick += () =>
            {
                if (Animating) return;
                try
                {
                    Behavior.SaveRecords();
                }
                catch (DotProbeBehavior.MessageException e)
                {
                    ShowMessage(e.Message, e.Message == "Records saved!" ? CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 200 : 0);
                }
            };

            AboutButton.OnClick += () =>
            {
                if (Animating) return;
                ShowMessage("Developer: Majidzadeh (github.com/pragma-once)\n"
                            + "Developed as Bachelor's Degree Final Project.\n\n"
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
                catch (DotProbeBehavior.MessageException e)
                {
                    ShowMessage(e.Message, 0);
                    await Task.Delay(CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 500);
                    MainWindow.Close();
                }
            };

            // Layout --------------------------------
            MainGrid.SizeChanged += (object sender, System.Windows.SizeChangedEventArgs e) =>
            {
                SquareGrid.Width = SquareGrid.Height = System.Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight);

                double Half = SquareGrid.Height / 2;
                double Quarter = SquareGrid.Height / 4;
                double Eighth = SquareGrid.Height / 8;
                double HalfAndQuarter = SquareGrid.Height * 0.75;
                double Gap = SquareGrid.Width / 32;
                double HalfGap = SquareGrid.Width / 64;
                double HalfG = Half + HalfGap;
                double QuarterG = Quarter + HalfGap;
                double HalfAndQuarterG = HalfAndQuarter + HalfGap;
                double H = SquareGrid.Height / 12;
                double HHalf = SquareGrid.Height / 24;

                // Start: Sequence options

                SequenceFilenameLabel.Margin = new System.Windows.Thickness(Gap, 0, HalfG, HalfAndQuarter + Eighth);
                SequenceFilenameLabel.Height = H;
                if (HHalf > 1) SequenceFilenameLabel.FontSize = HHalf;

                SequenceLengthLabel.Margin = new System.Windows.Thickness(HalfG, 0, Gap, HalfAndQuarter + Eighth);
                SequenceLengthLabel.Height = H;
                if (HHalf > 1) SequenceLengthLabel.FontSize = HHalf;

                SequenceFilenameTextBox.Margin = new System.Windows.Thickness(Gap, Eighth, HalfG, HalfAndQuarter);
                SequenceFilenameTextBox.Height = H;
                if (HHalf > 1) SequenceFilenameTextBox.FontSize = HHalf;

                SequenceLengthTextBox.Margin = new System.Windows.Thickness(HalfG, Eighth, Gap, HalfAndQuarter);
                SequenceLengthTextBox.Height = H;
                if (HHalf > 1) SequenceLengthTextBox.FontSize = HHalf;

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
            };

            // PreviewKeyDown instead of KeyDown to be able to handle arrow keys.
            MainWindow.PreviewKeyDown += async (object sender, System.Windows.Input.KeyEventArgs e) =>
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

                if (DotProbeGrid.Visibility == System.Windows.Visibility.Visible && !DisplayingWords)
                {
                    if (CurrentItem == null ?
                        (e.Key == System.Windows.Input.Key.Enter
                            || e.Key == System.Windows.Input.Key.Space
                            || e.Key.ToString().ToUpper() == Behavior.Settings.KeyForDotPosition1.ToUpper()
                            || e.Key.ToString().ToUpper() == Behavior.Settings.KeyForDotPosition2.ToUpper())
                        : ((e.Key.ToString().ToUpper() == Behavior.Settings.KeyForDotPosition1.ToUpper()
                                && CurrentItem.Item3 == DotProbeBehavior.DotPosition.Position1)
                            || (e.Key.ToString().ToUpper() == Behavior.Settings.KeyForDotPosition2.ToUpper()
                                && CurrentItem.Item3 == DotProbeBehavior.DotPosition.Position2)))
                    {
                        DisplayingWords = true;
                        if ((string)CenterLabel.Content == Behavior.Settings.CenterText)
                        {
                            Behavior.StopTimer();

                            Dot1.Visibility = System.Windows.Visibility.Hidden;
                            Dot2.Visibility = System.Windows.Visibility.Hidden;
                            CenterLabel.Content = "";

                            CurrentItem = Behavior.GetNext();
                            if (CurrentItem == null)
                            {
                                ShowMessage("Test done.", 0);
                                bool Animations = CustomControls.GlobalSettings.Animations;
                                await Task.Delay(CustomControls.GlobalSettings.Animations ?
                                                    CustomControls.GlobalSettings.AnimationDurationInMilliseconds + 100
                                                    : 100);
                                CustomControls.GlobalSettings.Animations = false;
                                HideGrid(DotProbeGrid, DotProbeToken);
                                CustomControls.GlobalSettings.Animations = Animations;
                            }
                            else
                            {
                                await Task.Delay(Behavior.Settings.WaitTime);

                                CenterLabel.Content = Behavior.Settings.CenterText;
                                Word1.Content = CurrentItem.Item1.Item2;
                                Word2.Content = CurrentItem.Item2.Item2;

                                await Task.Delay(Behavior.Settings.WordsDisplayTime);

                                Behavior.StartTimer();
                                Word1.Content = "";
                                Word2.Content = "";
                                if (CurrentItem.Item3 == DotProbeBehavior.DotPosition.Position1)
                                    Dot1.Visibility = System.Windows.Visibility.Visible;
                                else if (CurrentItem.Item3 == DotProbeBehavior.DotPosition.Position2)
                                    Dot2.Visibility = System.Windows.Visibility.Visible;
                            }
                        }
                        else // First
                        {
                            CenterLabel.Content = "";

                            await Task.Delay(Behavior.Settings.WaitTime);

                            CurrentItem = Behavior.GetNext();
                            CenterLabel.Content = Behavior.Settings.CenterText;
                            Word1.Content = CurrentItem.Item1.Item2;
                            Word2.Content = CurrentItem.Item2.Item2;

                            await Task.Delay(Behavior.Settings.WordsDisplayTime);

                            Behavior.StartTimer();
                            Word1.Content = "";
                            Word2.Content = "";
                            if (CurrentItem.Item3 == DotProbeBehavior.DotPosition.Position1)
                                Dot1.Visibility = System.Windows.Visibility.Visible;
                            else if (CurrentItem.Item3 == DotProbeBehavior.DotPosition.Position2)
                                Dot2.Visibility = System.Windows.Visibility.Visible;
                        }
                        DisplayingWords = false;
                    }
                    else if (e.Key.ToString().ToUpper() == Behavior.Settings.KeyForDotPosition1.ToUpper()
                            || e.Key.ToString().ToUpper() == Behavior.Settings.KeyForDotPosition2.ToUpper())
                    {
                        if (UseCustomBeep) try
                        {
                            CustomBeep.Stop();
                            CustomBeep.Play();
                            return;
                        }
                        catch (Exception) { UseCustomBeep = false; }

                        if (Behavior.Settings.BeepFromSystemSounds)
                            SystemSounds.Beep.Play();
                        else
                            Console.Beep();
                    }
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
            };
        }

        public Grid MainGrid { get; } = new Grid();

        public DotProbeBehavior Behavior { get; } = new DotProbeBehavior();

        bool Animating = false;
        bool Exiting = false;
        bool DisplayingWords = false;

        Tuple<Tuple<bool, string>, Tuple<bool, string>, DotProbeBehavior.DotPosition> CurrentItem = null;

        CustomControls.Ptr<int> StartToken = new CustomControls.Ptr<int>(0);
        //CustomControls.Ptr<int> SettingsToken = new CustomControls.Ptr<int>(0);
        CustomControls.Ptr<int> DotProbeToken = new CustomControls.Ptr<int>(0);
        CustomControls.Ptr<int> MessageToken = new CustomControls.Ptr<int>(0);

        MediaPlayer CustomBeep;
        bool UseCustomBeep = false;
    }

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    public class DotProbeBehavior
    {
        public class MessageException : Exception
        {
            public MessageException(string Message) : base(Message) { }
        }

        public Settings Settings { get; } = new Settings();

        public void GenerateSequence(string SequenceFilename, int SequenceLength)
        {
            // Check list files' validity
            if (!System.IO.File.Exists(Settings.NeutralWordsListFilename))
                throw new MessageException("No neutral words list file exists in:\n" + Settings.NeutralWordsListFilename);
            if (!System.IO.File.Exists(Settings.ThreateningWordsListFilename))
                throw new MessageException("No threatening words list file exists in:\n" + Settings.NeutralWordsListFilename);

            // Load
            string[] NeutralWords = System.IO.File.ReadAllLines(Settings.NeutralWordsListFilename);
            string[] ThreateningWords = System.IO.File.ReadAllLines(Settings.ThreateningWordsListFilename);
            int NeutralWordsLength = NeutralWords.Length;
            int ThreateningWordsLength = ThreateningWords.Length;

            if (NeutralWordsLength == 0)
                throw new MessageException("Neutral words list is empty! Please edit:\n" + Settings.NeutralWordsListFilename);
            if (ThreateningWordsLength == 0)
                throw new MessageException("Threatening words list is empty! Please edit:\n" + Settings.NeutralWordsListFilename);

            if (!Settings.AllowWordRepeat)
            {
                int Max = Math.Min(ThreateningWordsLength, NeutralWordsLength / 2);
                if (SequenceLength > Max)
                {
                    int SoftMax = Math.Min(
                        (int)(ThreateningWordsLength * (100.0 / Settings.ThreateningWordsProbability)),
                        (int)(NeutralWordsLength * (100.0 / (
                                Settings.ThreateningWordsProbability // 1 neutral word for each
                                + 2 * (100 - Settings.ThreateningWordsProbability) // 2 neutral words for each
                             )))
                    );
                    if (SequenceLength > SoftMax)
                            throw new MessageException("The maximum seq items count for the current set of words is:\n"
                                                        + SoftMax.ToString());
                }
            }

            // Save unsaved records
            bool UnsavedRecords = Records.Count > 0;
            if (UnsavedRecords)
                SaveRecords();

            bool InsufficientWords = false;

            // Generate
            Sequence.Clear();
            Random Rand = new Random();
            for (int i = 0; i < SequenceLength; i++)
            {
                bool HasThreatening = Rand.NextDouble() < Settings.ThreateningWordsProbability / 100;
                bool IsWord1Threatening = Rand.NextDouble() < 0.5;
                if (!Settings.AllowWordRepeat)
                {
                    bool InsufficientThreatening = ThreateningWordsLength < 1;
                    bool InsufficientNeutral = NeutralWordsLength <= SequenceLength - Sequence.Count;
                    if (NeutralWordsLength < 2 && InsufficientThreatening) // Unlikely or impossible
                    {
                        InsufficientWords = true;
                        break;
                    }
                    else if (InsufficientThreatening) HasThreatening = false; // Normal if !InsufficientNeutral
                    else if (InsufficientNeutral) HasThreatening = true; // Normal
                }

                int Word1Index = IsWord1Threatening && HasThreatening ?
                                Rand.Next(0, ThreateningWordsLength)
                                : Rand.Next(0, NeutralWordsLength);

                string Word1 = IsWord1Threatening && HasThreatening ?
                                ThreateningWords[Word1Index]
                                : NeutralWords[Word1Index];

                int Word2Index = !IsWord1Threatening && HasThreatening ?
                                Rand.Next(0, ThreateningWordsLength)
                                : Rand.Next(0, NeutralWordsLength);

                string Word2 = !IsWord1Threatening && HasThreatening ?
                                ThreateningWords[Word2Index]
                                : NeutralWords[Word2Index];

                if (!Settings.AllowWordRepeat)
                {
                    Action<int, int> SwapN = (int Index1, int Index2) =>
                    {
                        string temp = NeutralWords[Index1];
                        NeutralWords[Index1] = NeutralWords[Index2];
                        NeutralWords[Index2] = temp;
                    };
                    Action<int, int> SwapT = (int Index1, int Index2) =>
                    {
                        string temp = ThreateningWords[Index1];
                        ThreateningWords[Index1] = ThreateningWords[Index2];
                        ThreateningWords[Index2] = temp;
                    };

                    // For each related array, swap the chosen word with the last accepted word, and reduce their accepted lengths.
                    if (IsWord1Threatening && HasThreatening)  { SwapT(Word1Index, ThreateningWordsLength - 1); ThreateningWordsLength--; }
                    else                                       { SwapN(Word1Index, NeutralWordsLength     - 1); NeutralWordsLength--; }
                    if (!IsWord1Threatening && HasThreatening) { SwapT(Word2Index, ThreateningWordsLength - 1); ThreateningWordsLength--; }
                    else                                       { SwapN(Word2Index, NeutralWordsLength     - 1); NeutralWordsLength--; }
                }

                DotPosition Dot = (Rand.NextDouble() < Settings.OpositeSideDotProbability / 100) ?
                                    (IsWord1Threatening ? DotPosition.Position2 : DotPosition.Position1)
                                    : (IsWord1Threatening ? DotPosition.Position1 : DotPosition.Position2);

                Sequence.Add(new Tuple<Tuple<bool, string>, Tuple<bool, string>, DotPosition>(
                        new Tuple<bool, string>(IsWord1Threatening && HasThreatening, Word1),
                        new Tuple<bool, string>(!IsWord1Threatening && HasThreatening, Word2),
                        Dot
                    ));
            }

            // Save
            XElement SequenceXML = new XElement("Sequence");
            foreach (var Item in Sequence)
            {
                XElement ItemXML = new XElement("Item",
                        new XElement("Threatening1", Item.Item1.Item1),
                        new XElement("Word1", Item.Item1.Item2),
                        new XElement("Threatening2", Item.Item2.Item1),
                        new XElement("Word2", Item.Item2.Item2),
                        new XElement("DotPosition", (int)Item.Item3)
                    );
                SequenceXML.Add(ItemXML);
            }
            SequenceXML.Save(SequenceFilename);

            string SideNotes = InsufficientWords ? "\nInsufficient word lists, generated seq length=" + Sequence.Count : "";

            if (UnsavedRecords)
                throw new MessageException("Saved the records before generating the new sequence." + SideNotes);
            throw new MessageException("Sequence generated!" + SideNotes);
        }

        public void LoadSequence(string SequenceFilename)
        {
            if (!System.IO.File.Exists(SequenceFilename))
                throw new MessageException("Sequence file doesn't exist in:\n" + SequenceFilename);

            bool UnsavedRecords = Records.Count > 0;
            if (UnsavedRecords)
                SaveRecords();

            Sequence.Clear();
            try
            {
                XElement SequenceXML = XElement.Load(SequenceFilename);
                foreach (XElement ItemXML in SequenceXML.Elements("Item"))
                {
                    Sequence.Add(new Tuple<Tuple<bool, string>, Tuple<bool, string>, DotPosition>(
                            new Tuple<bool, string>(
                                System.Convert.ToBoolean(ItemXML.Element("Threatening1").Value),
                                ItemXML.Element("Word1").Value
                            ),
                            new Tuple<bool, string>(
                                System.Convert.ToBoolean(ItemXML.Element("Threatening2").Value),
                                ItemXML.Element("Word2").Value
                            ),
                            (DotPosition)System.Convert.ToInt32(ItemXML.Element("DotPosition").Value)
                        ));
                }
            }
            catch (Exception e) { throw new MessageException("Couldn't read the XML file: " + e.Message); }

            if (UnsavedRecords)
                throw new MessageException("Saved the records before loading the new sequence.");
            throw new MessageException("Sequence loaded!");
        }

        public void StartSequence(string UserName, int Age)
        {
            if (Sequence.Count == 0)
                throw new MessageException("No sequence is loaded!\nGenerate or Load to start.");
            SequenceIndex = -1;
            Records.Add(new Tuple<string, int, List<double>>(UserName, Age, new List<double>()));
        }

        public Tuple<Tuple<bool, string>, Tuple<bool, string>, DotPosition> GetNext()
        {
            StartTime = DateTime.Now;
            SequenceIndex++;
            if (SequenceIndex >= Sequence.Count)
            {
                StopSequence();
                return null;
            }
            return Sequence[SequenceIndex];
        }

        public void StartTimer()
        {
            StartTime = DateTime.Now;
        }

        public void StopTimer()
        {
            if (Records[Records.Count - 1].Item3.Count > SequenceIndex)
                Records[Records.Count - 1].Item3[SequenceIndex] = (DateTime.Now - StartTime).TotalSeconds;
            else
                Records[Records.Count - 1].Item3.Add((DateTime.Now - StartTime).TotalSeconds);
        }

        void StopSequence()
        {
            SequenceIndex = -1;
        }

        public void SaveRecords()
        {
            if (Records.Count == 0)
                throw new MessageException("No records to save");
            if (!System.IO.Directory.Exists("results"))
                System.IO.Directory.CreateDirectory("results");
            string Filename = "results/result_"
                            + DateTime.Now.Year.ToString() + "_"
                            + DateTime.Now.Month.ToString() + "_"
                            + DateTime.Now.Day.ToString() + "_"
                            + DateTime.Now.Hour.ToString() + "_"
                            + DateTime.Now.Minute.ToString() + "_"
                            + DateTime.Now.Second.ToString() + "_"
                            + DateTime.Now.Millisecond.ToString();
            if (System.IO.File.Exists(Filename + ".csv"))
            {
                int i;
                for (i = 0; System.IO.File.Exists(Filename + "_" + i.ToString() + ".csv"); i++) ;
                Filename += i.ToString();
            }
            Filename += ".csv";

            System.IO.FileStream file = System.IO.File.Create(Filename);
            file.Close();

            string Contents = "";

            Contents += "Name,,,,,";
            for (int j = 0; j < Records.Count; j++)
                Contents += "," + Records[j].Item1;
            Contents += "\n";

            Contents += "Age,,,,,";
            for (int j = 0; j < Records.Count; j++)
                Contents += "," + Records[j].Item2;
            Contents += "\n";

            List<CombinationType> Types = new List<CombinationType>(Sequence.Count);
            for (int i = 0; i < Sequence.Count; i++)
                if (!Sequence[i].Item1.Item1 && !Sequence[i].Item2.Item1)
                    Types.Add(CombinationType.Neutral);
                else if (Sequence[i].Item1.Item1 && Sequence[i].Item3 == DotPosition.Position1)
                    Types.Add(CombinationType.DotOnThreatening);
                else if (Sequence[i].Item2.Item1 && Sequence[i].Item3 == DotPosition.Position2)
                    Types.Add(CombinationType.DotOnThreatening);
                else
                    Types.Add(CombinationType.DotOnNeutral);

            Contents += "Attention Bias,,,,,";
            for (int j = 0; j < Records.Count; j++)
            {
                double UP_LE = 0; // Upper-dot Lower-emotional-word
                double UP_UE = 0; // Upper-dot Upper-emotional word
                double LP_UE = 0; // Lower-dot Upper-emotional word
                double LP_LE = 0; // Lower-dot Lower-emotional-word

                int UP_LE_C = 0;
                int UP_UE_C = 0;
                int LP_UE_C = 0;
                int LP_LE_C = 0;

                for (int i = 0; i < Sequence.Count; i++)
                {
                    if (Records[j].Item3.Count <= i)
                        break;
                    switch (Types[i])
                    {
                        case CombinationType.Neutral:
                            break;
                        case CombinationType.DotOnNeutral:
                            if (Sequence[i].Item1.Item1)
                            {
                                LP_UE += Records[j].Item3[i];
                                LP_UE_C += 1;
                            }
                            else
                            {
                                UP_LE += Records[j].Item3[i];
                                UP_LE_C += 1;
                            }
                            break;
                        case CombinationType.DotOnThreatening:
                            if (Sequence[i].Item1.Item1)
                            {
                                UP_UE += Records[j].Item3[i];
                                UP_UE_C += 1;
                            }
                            else
                            {
                                LP_LE += Records[j].Item3[i];
                                LP_LE_C += 1;
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (UP_LE_C != 0) UP_LE /= UP_LE_C;
                if (UP_UE_C != 0) UP_UE /= UP_UE_C;
                if (LP_UE_C != 0) LP_UE /= LP_UE_C;
                if (LP_LE_C != 0) LP_LE /= LP_LE_C;
                Contents += "," + (((UP_LE - UP_UE) + (LP_UE - LP_LE)) / 2).ToString();
            }
            Contents += "\n";

            Contents += "Word1,IsThreatening1,Word2,IsThreatening2,DotPosition,Type";
            for (int j = 0; j < Records.Count; j++)
                Contents += ",ReactionTime";
            Contents += "\n";

            for (int i = 0; i < Sequence.Count; i++)
            {
                Contents += Sequence[i].Item1.Item2
                        + "," + Sequence[i].Item1.Item1
                        + "," + Sequence[i].Item2.Item2
                        + "," + Sequence[i].Item2.Item1
                        + "," + ((int)Sequence[i].Item3).ToString()
                        + "," + Types[i].ToString();

                for (int j = 0; j < Records.Count; j++)
                {
                    if (Records[j].Item3.Count > i)
                        Contents += "," + Records[j].Item3[i];
                    else
                        Contents += ",N/A";
                }
                Contents += '\n';
            }

            System.IO.File.WriteAllText(Filename, Contents);

            Records.Clear();

            throw new MessageException("Records saved!");
        }

        public enum DotPosition : byte { Position1 = 1, Position2 = 2 };
        enum CombinationType : byte { Neutral = 0, DotOnNeutral = 1, DotOnThreatening = 2 };

        // Tuple<Tuple<IsThreatening, Word>, Tuple<IsThreatening, Word>, DotPosition>
        List<Tuple<Tuple<bool, string>, Tuple<bool, string>, DotPosition>> Sequence = new List<Tuple<Tuple<bool, string>, Tuple<bool, string>, DotPosition>>();
        int SequenceIndex = -1;
        // Tuple<UserName, Age, List<ReactionTime>>
        List<Tuple<string, int, List<double>>> Records = new List<Tuple<string, int, List<double>>>();

        DateTime StartTime;
    }

    public class Settings
    {
        const string SettingsFilename = "settings.xml";

        public bool Fullscreen;

        public bool DarkMode;
        public bool Animations;

        public string NeutralWordsListFilename;
        public string ThreateningWordsListFilename;

        public double ThreateningWordsProbability;
        public double OpositeSideDotProbability;
        public bool AllowWordRepeat;

        public bool VerticalSides;
        public double SidesOffset;
        public double FontSize;
        public double DotSize;

        public string FontFamily;

        public int WaitTime;
        public int WordsDisplayTime;

        public string CenterText;

        public string KeyForDotPosition1;
        public string KeyForDotPosition2;

        public bool BeepFromSystemSounds;
        public string CustomBeepFilename;

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

            NeutralWordsListFilename = "data/neutral-list.txt";
            ThreateningWordsListFilename = "data/threatening-list.txt";

            ThreateningWordsProbability = 66;
            OpositeSideDotProbability = 50;
            AllowWordRepeat = false;

            VerticalSides = true;
            SidesOffset = 200;
            FontSize = 40;
            DotSize = 8;

            FontFamily = "";

            WaitTime = 250;
            WordsDisplayTime = 500;

            CenterText = "+";

            KeyForDotPosition1 = "Up";
            KeyForDotPosition2 = "Down";

            BeepFromSystemSounds = false;
            CustomBeepFilename = "";
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
                                case "NeutralWordsListFilename":
                                    NeutralWordsListFilename = element.Value;
                                    break;
                                case "ThreateningWordsListFilename":
                                    ThreateningWordsListFilename = element.Value;
                                    break;
                                case "ThreateningWordsProbability":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD >= 0 && tempD <= 100)
                                        ThreateningWordsProbability = tempD;
                                    break;
                                case "OpositeSideDotProbability":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD >= 0 && tempD <= 100)
                                        OpositeSideDotProbability = tempD;
                                    break;
                                case "AllowWordRepeat":
                                    AllowWordRepeat = System.Convert.ToBoolean(element.Value);
                                    break;
                                case "VerticalSides":
                                    VerticalSides = System.Convert.ToBoolean(element.Value);
                                    break;
                                case "SidesOffset":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    SidesOffset = tempD;
                                    break;
                                case "FontSize":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    FontSize = tempD;
                                    break;
                                case "DotSize":
                                    tempD = System.Convert.ToDouble(element.Value);
                                    if (tempD < 0)
                                        tempD = -tempD;
                                    DotSize = tempD;
                                    break;
                                case "FontFamily":
                                    FontFamily = element.Value;
                                    break;
                                case "WaitTime":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    WaitTime = tempI;
                                    break;
                                case "WordsDisplayTime":
                                    tempI = System.Convert.ToInt32(element.Value);
                                    if (tempI < 0)
                                        tempI = -tempI;
                                    WordsDisplayTime = tempI;
                                    break;
                                case "CenterText":
                                    CenterText = element.Value;
                                    break;
                                case "KeyForDotPosition1":
                                    KeyForDotPosition1 = element.Value;
                                    break;
                                case "KeyForDotPosition2":
                                    KeyForDotPosition2 = element.Value;
                                    break;
                                case "BeepFromSystemSounds":
                                    BeepFromSystemSounds = System.Convert.ToBoolean(element.Value);
                                    break;
                                case "CustomBeepFilename":
                                    CustomBeepFilename = element.Value;
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
                new XElement("NeutralWordsListFilename", NeutralWordsListFilename),
                new XElement("ThreateningWordsListFilename", ThreateningWordsListFilename),
                new XElement("ThreateningWordsProbability", ThreateningWordsProbability),
                new XElement("OpositeSideDotProbability", OpositeSideDotProbability),
                new XElement("AllowWordRepeat", AllowWordRepeat),
                new XElement("VerticalSides", VerticalSides),
                new XElement("SidesOffset", SidesOffset),
                new XElement("FontSize", FontSize),
                new XElement("DotSize", DotSize),
                new XElement("FontFamily", FontFamily),
                new XElement("WaitTime", WaitTime),
                new XElement("WordsDisplayTime", WordsDisplayTime),
                new XElement("CenterText", CenterText),
                new XElement("KeyForDotPosition1", KeyForDotPosition1),
                new XElement("KeyForDotPosition2", KeyForDotPosition2),
                new XElement("BeepFromSystemSounds", BeepFromSystemSounds),
                new XElement("CustomBeepFilename", CustomBeepFilename)
                );
            xml_settings.Save(SettingsFilename);
        }
    }
}
