#ifndef PIECEWISEEASE_INCLUDED
#define PIECEWISEEASE_INCLUDED

float EaseInOutQuart(float t)
{
    return t < 0.5 ? 8.0 * t * t * t * t : 1.0 - pow(-2.0 * t + 2.0, 4.0) / 2.0;
}

void PiecewiseEase_float(float x, out float Out)
{
    if (x < -20.0)
    {
        Out = -1.0;
    }
    else if (x < -10.0)
    {
        Out = -1.0 + EaseInOutQuart((x + 20.0) / 10.0);
    }
    else if (x < 10.0)
    {
        Out = 0.0;
    }
    else if (x < 20.0)
    {
        Out = EaseInOutQuart((x - 10.0) / 10.0);
    }
    else if (x < 190.0)
    {
        Out = 1.0;
    }
    else if (x < 200.0)
    {
        Out = 1.0 - EaseInOutQuart((x - 190.0) / 10.0);
    }
    else if (x < 220.0)
    {
        Out = 0.0;
    }
    else if (x < 230.0)
    {
        Out = -EaseInOutQuart((x - 220.0) / 10.0);
    }
    else
    {
        Out = -1.0;
    }
}

#endif //PIECEWISEEASE_INCLUDED