using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Binary
{
    public static int Trailing_Zeros(ulong num)
    {
        if (num == 0) return 64;

        int count = 0;
        while ((num & 1) == 0)
        {
            count++;
            num >>= 1;
        }

        return count;
    }

    public static int Trailing_Ones(ulong num)
    {
        if (num == 0) return 0;

        int count = 0;
        while ((num & 1) == 1)
        {
            count++;
            num >>= 1;
        }

        return count;
    }
}
