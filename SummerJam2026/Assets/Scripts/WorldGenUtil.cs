/// <summary>
/// Deterministic hashing helpers for procedural world generation. Given the same
/// inputs they always return the same value, so a column can be regenerated
/// identically after it has been cleared (and after a floating-origin rebase).
/// Pure integer math — no allocation, safe to call thousands of times per frame.
/// </summary>
public static class WorldGenUtil
{
    // Three odd constants from the integer-hash literature (xxHash / Murmur style).
    const uint Prime1 = 0x9E3779B1u;
    const uint Prime2 = 0x85EBCA77u;
    const uint Prime3 = 0xC2B2AE3Du;

    /// <summary>Hashes an integer with a seed into a well-mixed 32-bit value.</summary>
    public static uint Hash(int x, uint seed)
    {
        uint h = (uint)x * Prime1;
        h ^= seed;
        h ^= h >> 15;
        h *= Prime2;
        h ^= h >> 13;
        h *= Prime3;
        h ^= h >> 16;
        return h;
    }

    /// <summary>Deterministic value in [0, 1) from one integer + seed.</summary>
    public static float Hash01(int x, uint seed)
    {
        // Top 24 bits give a float with full mantissa precision in [0, 1).
        return (Hash(x, seed) >> 8) * (1f / 16777216f);
    }

    /// <summary>Deterministic value in [0, 1) from two integers + seed.</summary>
    public static float Hash01(int x, int y, uint seed)
    {
        // Fold y into the seed so (x, y) pairs decorrelate from plain (x).
        return Hash01(x, seed ^ (Hash(y, seed) | 1u));
    }
}
