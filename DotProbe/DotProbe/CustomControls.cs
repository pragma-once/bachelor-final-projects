using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace CustomControls
{
    public class Ptr<Type>
    {
        public Ptr(Type InitialValue) { this.Item = InitialValue; }
        public Type Item { get; set; }
    }

    public static class GlobalSettings
    {
        static GlobalSettings()
        {
            // Initializations
        }

        public static bool DarkMode
        {
            get { return _DarkMode; }
            set
            {
                if (_DarkMode != value)
                {
                    _DarkMode = value;
                    if (Animations)
                    {
                        Animators.AnimateNatural(
                            Token: DarkModeSwitchToken,
                            UpdateFunction: (double Value) =>
                            {
                                Darkness = (byte)Value;
                                foreach (Control control in _AllControls)
                                    control.UpdateDarkness(Darkness);
                                byte Brightness = (byte)(255 - Darkness);
                                BG.Color = Color.FromArgb(255, Brightness, Brightness, Brightness);
                                FG.Color = Color.FromArgb(255, Darkness, Darkness, Darkness);
                            },
                            Speed: AnimationSpeed,
                            A: Darkness,
                            B: value ? 255 : 0
                            );
                    }
                    else
                    {
                        Darkness = value ? (byte)255 : (byte)0;
                        foreach (Control control in _AllControls)
                            control.UpdateDarkness(Darkness);
                        byte Brightness = (byte)(255 - Darkness);
                        BG.Color = Color.FromArgb(255, Brightness, Brightness, Brightness);
                        FG.Color = Color.FromArgb(255, Darkness, Darkness, Darkness);
                    }
                }
            }
        }
        public static bool Animations = true;
        public static double AnimationSpeed = 1 / 0.4;
        public static double AnimationDuration
        {
            get { return 1 / AnimationSpeed; }
        }
        public static int AnimationDurationInMilliseconds
        {
            get { return (int)(1000 / AnimationSpeed); }
        }

        public static readonly SolidColorBrush BG = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        public static readonly SolidColorBrush FG = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        /// <summary>
        /// Don't touch this.
        /// </summary>
        public static List<Control> _AllControls = new List<Control>();

        static Ptr<int> DarkModeSwitchToken = new Ptr<int>(0);
        static bool _DarkMode = true;
        static byte Darkness = 255;
    }

    public static class Interpolators
    {
        public static double Linear(double t, double A, double B) { return A + (B - A) * t; }
        public static int Linear(double t, int A, int B) { return (int)(A + (B - A) * t); }
        public static byte Linear(double t, byte A, byte B) { return (byte)(A + (B - A) * t); }

        public static double Natural(double t)
        {
            if (t < 0 || t > 1) throw new ArgumentOutOfRangeException("t");
            if (t == 0) return 0;
            if (t == 1) return 1;
            if (t < 0.5) return t * t * 2; // (0, 0.5)
            t = 1 - t;
            return 1 - t * t * 2; // [0.5, 1)
        }

        public static double Natural(double t, double A, double B) { return A + (B - A) * Natural(t); }
        public static int Natural(double t, int A, int B) { return (int)(A + (B - A) * Natural(t)); }
        public static byte Natural(double t, byte A, byte B) { return (byte)(A + (B - A) * Natural(t)); }
    }

    public static class Animators
    {
        public static async Task Tail(Ptr<int> Token, Action<double> UpdateFunction, double Current, double Target, double Speed = 4, double AvoidableDistance = 0.01)
        {
            int this_token = Token.Item + 1;
            Token.Item++;
            await Task.Delay(1);
            if (this_token != Token.Item)
                return;
            while (Math.Abs(Target - Current) > AvoidableDistance)
            {
                Current = Current + (Target - Current) * 0.025 * Speed;
                UpdateFunction(Current);
                await Task.Delay(25);
                if (this_token != Token.Item)
                    return;
            }
            UpdateFunction(Target);
        }

        public static async Task Animate(Ptr<int> Token, Action<double> UpdateFunction, double Speed)
        {
            int this_token = Token.Item + 1;
            Token.Item++;
            double delta = 0.025 * Speed;
            for (double t = 0; t < 1; t += delta)
            {
                UpdateFunction(t);
                await Task.Delay(25);
                if (this_token != Token.Item)
                    return;
            }
            UpdateFunction(1);
        }

        public static async Task Animate(
            Ptr<int> Token,
            Action<double> UpdateFunction,
            double Speed,
            double A,
            double B
            )
        {
            await Animate(Token, (double t) => { UpdateFunction(Interpolators.Linear(t, A, B)); }, Speed);
        }

        public static async Task Animate(
            Ptr<int> Token,
            Action<double> UpdateFunction,
            double Speed,
            Func<double, double> Interpolator
            )
        {
            await Animate(Token, (double t) => { UpdateFunction(Interpolator(t)); }, Speed);
        }

        public static async Task Animate(
            Ptr<int> Token,
            Action<double> UpdateFunction,
            double Speed,
            Func<double, double, double, double> Interpolator,
            double A,
            double B
            )
        {
            await Animate(Token, (double t) => { UpdateFunction(Interpolator(t, A, B)); }, Speed);
        }

        public static async Task AnimateNatural(
            Ptr<int> Token,
            Action<double> UpdateFunction,
            double Speed
            )
        {
            await Animate(Token, UpdateFunction, Speed, Interpolators.Natural);
        }

        public static async Task AnimateNatural(
            Ptr<int> Token,
            Action<double> UpdateFunction,
            double Speed,
            double A,
            double B
            )
        {
            await Animate(Token, UpdateFunction, Speed, Interpolators.Natural, A, B);
        }
    }

    public abstract class Control
    {
        public Control()
        {
            GlobalSettings._AllControls.Add(this);
            UpdateDarkness(GlobalSettings.DarkMode ? (byte)255 : (byte)0);
        }
        ~Control()
        {
            GlobalSettings._AllControls.Remove(this);
        }
        public Grid MainGrid { get; } = new Grid();
        public abstract void UpdateDarkness(byte Darkness);
    }

    public class Toggle : Control
    {
        public Toggle(string Text = "", bool InitialValue = false)
        {
            this.Text = Text;
            ToggleState = InitialValue;
            ThumbPosition = ToggleState ? 1 : 0;

            MainGrid.Background = BGBrush;
            MainGrid.Focusable = true;

            Label.HorizontalAlignment = HorizontalAlignment.Stretch;
            Label.VerticalAlignment = VerticalAlignment.Stretch;
            Label.HorizontalContentAlignment = HorizontalAlignment.Center;
            Label.VerticalContentAlignment = VerticalAlignment.Center;
            Label.Foreground = FGBrush;

            MainGrid.Children.Add(Label);

            ThumbHolderGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            ThumbHolderGrid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            MainGrid.Children.Add(ThumbHolderGrid);

            ThumbGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            ThumbGrid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            ThumbGrid.Background = FGBrush;

            ThumbHolderGrid.Children.Add(ThumbGrid);

            UpdatePosition();
            UpdateSize();
            UpdateColors();

            MainGrid.SizeChanged += (object sender, System.Windows.SizeChangedEventArgs e) =>
            {
                UpdatePosition();
                UpdateSize();
                if (MainGrid.ActualHeight > 1)
                    Label.FontSize = MainGrid.ActualHeight * 0.4;
            };

            MainGrid.MouseMove += (object sender, System.Windows.Input.MouseEventArgs e) =>
            {
                Point pos = e.MouseDevice.GetPosition(MainGrid);
                pos.X /= MainGrid.ActualWidth / 2;
                pos.Y /= MainGrid.ActualHeight / 2;
                pos.X -= 1;
                pos.Y -= 1;

                pos.X *= pos.X;
                pos.Y *= pos.Y;

                pos.X = 1 - pos.X;
                pos.Y = 1 - pos.Y;

                if (GlobalSettings.Animations)
                    Animators.Tail(
                        ShrinkToken,
                        (double x) => { ThumbShrinkage = x; UpdateSize(); },
                        ThumbShrinkage,
                        1 - pos.X * pos.Y
                        );
                else
                {
                    ThumbShrinkage = 1 - pos.X * pos.Y;
                    UpdateSize();
                }
            };

            MainGrid.MouseLeave += (object sender, System.Windows.Input.MouseEventArgs e) =>
            {
                ClickPotential = false;
                if (GlobalSettings.Animations)
                    Animators.Tail(
                        ShrinkToken,
                        (double x) => { ThumbShrinkage = x; UpdateSize(); },
                        ThumbShrinkage,
                        1
                        );
                else
                {
                    ThumbShrinkage = 1;
                    UpdateSize();
                }
            };

            MainGrid.MouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                ClickPotential = true;
                MainGrid.Focus();
            };

            MainGrid.MouseUp += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                if (ClickPotential)
                {
                    ClickPotential = false;
                    Value = !Value;
                }
            };
        }

        public Action<bool> OnToggle;

        public string Text
        {
            get { return (string)Label.Content; }
            set { Label.Content = value; }
        }

        public bool Value
        {
            get { return ToggleState; }
            set
            {
                if (ToggleState != value)
                {
                    ToggleState = value;
                    OnToggle?.Invoke(ToggleState);
                    if (GlobalSettings.Animations)
                        Animators.AnimateNatural(
                            Token: ToggleToken,
                            UpdateFunction: (double Value) =>
                            {
                                ThumbPosition = Value;
                                UpdatePosition();
                                UpdateColors();
                            },
                            Speed: GlobalSettings.AnimationSpeed,
                            A: ThumbPosition,
                            B: ToggleState ? 1 : 0
                            );
                    else
                    {
                        ThumbPosition = ToggleState ? 1 : 0;
                        UpdatePosition();
                    }
                }
            }
        }

        Label Label = new Label();
        Grid ThumbHolderGrid = new Grid();
        Grid ThumbGrid = new Grid();

        SolidColorBrush BGBrush = new SolidColorBrush();
        SolidColorBrush FGBrush = new SolidColorBrush();

        byte FGBrightness  = 0;
        const byte MidBrightness = 127;
        byte BGBrightness  = 255;
        double ThumbPosition = 0; // [0, 1]
        double ThumbShrinkage = 1; // [0, 1]

        bool ToggleState = false;
        bool ClickPotential = false;

        Ptr<int> ToggleToken = new Ptr<int>(0);
        Ptr<int> ShrinkToken = new Ptr<int>(0);

        public override void UpdateDarkness(byte Darkness)
        {
            BGBrightness = (byte)(240 - Darkness * 0.75);
            FGBrightness = Darkness;
            UpdateColors();
        }

        void UpdateColors()
        {
            BGBrush.Color = Color.FromArgb(255, BGBrightness, BGBrightness, BGBrightness);

            byte thumb = (byte)(MidBrightness + (byte)((FGBrightness - MidBrightness) * ThumbPosition));
            FGBrush.Color = Color.FromArgb(255, thumb, thumb, thumb);
        }

        void UpdatePosition()
        {
            ThumbHolderGrid.Width = MainGrid.ActualWidth / 2;
            ThumbHolderGrid.Margin = new Thickness(ThumbPosition * MainGrid.ActualWidth / 2, 0, 0, 0);
            double Pow = 1 - Math.Abs(ThumbPosition * 2 - 1);
            Pow *= Pow;
            Pow *= Pow;
            Pow = 1 - Pow;
            Pow *= Math.Sign(ThumbPosition - 0.5);
            Pow = (Pow + 1) / 2;
            Label.Margin = new Thickness((1 - Pow) * MainGrid.ActualWidth / 2, 0, Pow * MainGrid.ActualWidth / 2, 0);
        }

        void UpdateSize()
        {
            double x = ThumbShrinkage * MainGrid.ActualWidth / 2 / 6;
            double y = ThumbShrinkage * MainGrid.ActualHeight / 6;
            ThumbGrid.Margin = new Thickness(x, y, x, y);
        }
    }

    public class Button : Control
    {
        public Button(string Text = "")
        {
            this.Text = Text;

            MainGrid.Background = BGBrush;
            MainGrid.Focusable = true;

            Label.HorizontalAlignment = HorizontalAlignment.Stretch;
            Label.VerticalAlignment = VerticalAlignment.Stretch;
            Label.HorizontalContentAlignment = HorizontalAlignment.Center;
            Label.VerticalContentAlignment = VerticalAlignment.Center;
            Label.Padding = new Thickness(0);

            Label.Foreground = FGBrush;

            MainGrid.Children.Add(Label);

            MainGrid.SizeChanged += (object sender, System.Windows.SizeChangedEventArgs e) =>
            {
                if (MainGrid.ActualHeight > 1) Label.FontSize = MainGrid.ActualHeight * 0.5;
            };

            MainGrid.MouseEnter += (object sender, System.Windows.Input.MouseEventArgs e) =>
            {
                if (GlobalSettings.Animations)
                    Animators.Animate(
                        HToken,
                        (double h) => { H = h; UpdateColors(); },
                        GlobalSettings.AnimationSpeed * 2,
                        H, 1);
                else H = 1;
            };

            MainGrid.MouseLeave += (object sender, System.Windows.Input.MouseEventArgs e) =>
            {
                if (GlobalSettings.Animations)
                {
                    Animators.Animate(
                        HToken,
                        (double h) => { H = h; UpdateColors(); },
                        GlobalSettings.AnimationSpeed,
                        H, 0);
                    if (ClickPotential) Animators.AnimateNatural(
                        PToken,
                        (double p) => { P = p; UpdateColors(); },
                        GlobalSettings.AnimationSpeed / 2,
                        P, 0);
                }
                else
                {
                    H = 0;
                    P = 0;
                }
                ClickPotential = false;
            };

            MainGrid.MouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                ClickPotential = true;
                if (GlobalSettings.Animations)
                    Animators.Animate(
                        PToken,
                        (double p) => { P = p; UpdateColors(); },
                        GlobalSettings.AnimationSpeed * 4,
                        P, 1);
                else P = 1;
                MainGrid.Focus();
            };

            MainGrid.MouseUp += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                if (ClickPotential)
                {
                    ClickPotential = false;
                    OnClick?.Invoke();
                }
                if (GlobalSettings.Animations)
                    Animators.Animate(
                        PToken,
                        (double p) => { P = p; UpdateColors(); },
                        PSpeed,
                        P, 0);
                else P = 0;
            };
        }

        public Action OnClick;
        public string Text
        {
            get { return (string)Label.Content; }
            set { Label.Content = value; }
        }

        Label Label = new Label();

        SolidColorBrush BGBrush = new SolidColorBrush();
        SolidColorBrush FGBrush = new SolidColorBrush();

        byte Normal = 0;
        const byte Mid = 140;
        const double HShade = 0.2;
        const double PShade = 0.2;
        byte FG = 255;

        const double PSpeed = 4;

        double H = 0;
        double P = 0;

        bool ClickPotential = false;

        Ptr<int> HToken = new Ptr<int>(0);
        Ptr<int> PToken = new Ptr<int>(0);

        public override void UpdateDarkness(byte Darkness)
        {
            Normal = (byte)(240 - Darkness * 0.75);
            FG = Darkness;
            UpdateColors();
        }

        void UpdateColors()
        {
            byte BG = (byte)(Normal + (Mid - Normal) * (H * HShade + P * PShade));
            BGBrush.Color = Color.FromArgb(255, BG, BG, BG);
            FGBrush.Color = Color.FromArgb(255, FG, FG, FG);
        }
    }
}

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
