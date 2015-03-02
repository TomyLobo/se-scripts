public static Color ColorFromHSV(double hue, double saturation, double value)
{
    int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
    double f = hue / 60 - Math.Floor(hue / 60);

    value = value * 255;
    int v = Convert.ToInt32(value);
    int p = Convert.ToInt32(value * (1 - saturation));
    int q = Convert.ToInt32(value * (1 - f * saturation));
    int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

    if (hi == 0)
        return new Color(v, t, p);
    else if (hi == 1)
        return new Color(q, v, p);
    else if (hi == 2)
        return new Color(p, v, t);
    else if (hi == 3)
        return new Color(p, q, v);
    else if (hi == 4)
        return new Color(t, p, v);
    else
        return new Color(v, p, q);
}

static int i = 0;
static int j = 0;

void Main() {
    IMyInteriorLight light = GridTerminalSystem.GetBlockWithName("TomyLight") as IMyInteriorLight;
    ++j;
    if (j%5==0) {
        i+=2;
        i %= 360;
        Color col = ColorFromHSV(i, 1, 1);
        light.SetValue( "Color", col );
    }
}
