using UnityEngine;

public class SafeInteger
{
    private int key1;
    private int key2;

    private int part1;
    private int part2;

    private int f1;
    private int f2;

    private const int m_f2_param1 = 23;
    private const int m_f2_param2 = 67;

    public SafeInteger()
    {
        key1 = Random.Range(1000, 9999);
        key2 = Random.Range(1000, 9999);
        part1 = Random.Range(1, 5000);
        part2 = Random.Range(1, 5000);
        UpdateChecksums();
    }

    public SafeInteger(int value) : this()
    {
        Set(value);
    }

    private void UpdateChecksums()
    {
        int val = GetRaw();
        f1 = key1 * val + key2;
        f2 = m_f2_param1 * val * val + m_f2_param2;
    }

    private int GetRaw()
    {
        return part1 - part2;
    }

    public int Get()
    {
        int val = GetRaw();

        if (f1 != key1 * val + key2 || f2 != m_f2_param1 * val * val + m_f2_param2)
        {
            part1 = Random.Range(100, 5000) + val;
            part2 = part1 - val;
            UpdateChecksums();
        }

        key1 = key1 ^ (Random.Range(1, 255));
        key2 = key2 ^ (Random.Range(1, 255));
        UpdateChecksums();

        return GetRaw();
    }

    public void Set(int value)
    {
        part1 = Random.Range(1, 5000) + value;
        part2 = part1 - value;
        UpdateChecksums();
    }

    public static implicit operator SafeInteger(int value)
    {
        return new SafeInteger(value);
    }

    public static implicit operator int(SafeInteger s)
    {
        if (s == null) return 0; // or some safe default
        return s.Get();
    }

    public static SafeInteger operator +(SafeInteger a, SafeInteger b)
    {
        int valA = (a != null) ? a.Get() : 0;
        int valB = (b != null) ? b.Get() : 0;
        return new SafeInteger(valA + valB);
    }

    public static SafeInteger operator -(SafeInteger a, SafeInteger b)
    {
        int valA = (a != null) ? a.Get() : 0;
        int valB = (b != null) ? b.Get() : 0;
        return new SafeInteger(valA - valB);
    }

    public static SafeInteger operator *(SafeInteger a, SafeInteger b)
    {
        int valA = (a != null) ? a.Get() : 0;
        int valB = (b != null) ? b.Get() : 0;
        return new SafeInteger(valA * valB);
    }

    public static SafeInteger operator /(SafeInteger a, SafeInteger b)
    {
        int valA = (a != null) ? a.Get() : 0;
        int valB = (b != null) ? b.Get() : 1;
        return new SafeInteger(valA / valB);
    }

    public static SafeInteger operator ++(SafeInteger a)
    {
        if (a == null) return new SafeInteger(1);
        return new SafeInteger(a.Get() + 1);
    }

    public static SafeInteger operator --(SafeInteger a)
    {
        if (a == null) return new SafeInteger(0);
        return new SafeInteger(a.Get() - 1);
    }

    public override bool Equals(object obj)
    {
        if (obj is SafeInteger)
        {
            return Get() == ((SafeInteger)obj).Get();
        }
        if (obj is int)
        {
            return Get() == (int)obj;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Get().GetHashCode();
    }

    public override string ToString()
    {
        return Get().ToString();
    }

    public string ToString(string format)
    {
        return Get().ToString(format);
    }
}