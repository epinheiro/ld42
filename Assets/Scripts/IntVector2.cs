public class IntVector2
{
    public int x;
    public int y;

    public IntVector2(int ax, int ay)
    {
        x = ax;
        y = ay;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        IntVector2 p = (IntVector2)obj;
        return (x == p.x) && (y == p.y);
    }
    public override int GetHashCode()
    {
        return x ^ y;
    }
}