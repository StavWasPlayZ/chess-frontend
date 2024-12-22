using Avalonia.Layout;

namespace chess_frontend.Util;

public static class LayoutableExtensions
{
    public static void SetSize(this Layoutable layout, double size)
    {
        layout.Width = layout.Height = size;
    }
}