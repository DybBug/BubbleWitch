public static class Tag
{
    public static readonly string Wall = "Wall";
    public static readonly string Bubble = "Bubble";
}

public static class LayerMask
{
    public static readonly int Wall = 1 << 6;
    public static readonly int Bubble = 1 << 7;
}