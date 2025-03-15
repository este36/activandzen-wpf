using System.Windows;

namespace ActivAndZen.Styles;

public static class Spacings
{
    public const double FontFactor = 0.5;

    public static class FontSize
    {
        public const double Small = 12;
        public const double Medium = 14;
        public const double Large = 18; 
    }

    public static class Margin
    {
        public const double Small = Spacings.FontSize.Small * Spacings.FontFactor;
        public const double Medium = Spacings.FontSize.Medium * Spacings.FontFactor;
        public const double Large = Spacings.FontSize.Large * Spacings.FontFactor; 
    }

    public static class Padding
    {
        public const double Small = Spacings.FontSize.Small * Spacings.FontFactor;
        public const double Medium = Spacings.FontSize.Medium * Spacings.FontFactor;
        public const double Large = Spacings.FontSize.Large * Spacings.FontFactor;
    }

    public static class Border
    {
        public const double Small = 0.5;
        public const double Medium = 1;
        public const double Large = 2; 
    }
}

public static class BorderRadius
{
    public static CornerRadius r0 = new(4);
    public static CornerRadius r1 = new(8);
    public static CornerRadius r2 = new(12);
    public static CornerRadius r3 = new(18);
}

