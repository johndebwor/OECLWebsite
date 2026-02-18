using MudBlazor;

namespace OECLWebsite.Shared;

public static class OvalTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0A2463",           // Deep Blue
            Secondary = "#F97316",         // Engineering Orange
            Tertiary = "#3E92CC",          // Ocean Blue
            AppbarBackground = "#0A2463",
            AppbarText = "#FFFFFF",
            Background = "#F8FAFC",        // Neutral
            Surface = "#FFFFFF",
            DrawerBackground = "#0F172A",  // Deep Slate
            DrawerText = "#E2E8F0",
            DrawerIcon = "#94A3B8",
            TextPrimary = "#0F172A",
            TextSecondary = "#475569",
            ActionDefault = "#3E92CC",
            Success = "#16A34A",
            Warning = "#F59E0B",
            Error = "#DC2626",
            Info = "#3E92CC",
            LinesDefault = "#E2E8F0",
            TableLines = "#E2E8F0",
            Divider = "#E2E8F0",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#3E92CC",           // Ocean Blue
            Secondary = "#F97316",         // Engineering Orange
            Tertiary = "#0A2463",          // Deep Blue
            AppbarBackground = "#0F172A",
            AppbarText = "#F8FAFC",
            Background = "#0F172A",
            Surface = "#1E293B",
            DrawerBackground = "#0F172A",
            DrawerText = "#F8FAFC",
            TextPrimary = "#F8FAFC",
            TextSecondary = "#94A3B8",
            ActionDefault = "#3E92CC",
            Success = "#22C55E",
            Warning = "#FB923C",
            Error = "#EF4444",
            Info = "#3E92CC",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "system-ui", "-apple-system", "sans-serif"],
                FontSize = "0.9375rem",
                LineHeight = "1.6"
            },
            H1 = new H1Typography
            {
                FontFamily = ["Rajdhani", "Inter", "sans-serif"],
                FontWeight = "700",
                FontSize = "3rem",
                LetterSpacing = "-0.02em"
            },
            H2 = new H2Typography
            {
                FontFamily = ["Rajdhani", "Inter", "sans-serif"],
                FontWeight = "700",
                FontSize = "2.25rem",
                LetterSpacing = "-0.01em"
            },
            H3 = new H3Typography
            {
                FontFamily = ["Rajdhani", "Inter", "sans-serif"],
                FontWeight = "600",
                FontSize = "1.875rem",
                LetterSpacing = "-0.01em"
            },
            H4 = new H4Typography
            {
                FontFamily = ["Rajdhani", "Inter", "sans-serif"],
                FontWeight = "600",
                FontSize = "1.5rem"
            },
            H5 = new H5Typography
            {
                FontFamily = ["Rajdhani", "Inter", "sans-serif"],
                FontWeight = "600",
                FontSize = "1.25rem"
            },
            H6 = new H6Typography
            {
                FontFamily = ["Rajdhani", "Inter", "sans-serif"],
                FontWeight = "600",
                FontSize = "1.1rem"
            },
            Body1 = new Body1Typography
            {
                FontSize = "0.9375rem",
                LineHeight = "1.7"
            },
            Body2 = new Body2Typography
            {
                FontSize = "0.875rem",
                LineHeight = "1.6"
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontWeight = "600",
                FontSize = "1rem"
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontWeight = "600",
                FontSize = "0.875rem"
            }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px"
        }
    };
}
