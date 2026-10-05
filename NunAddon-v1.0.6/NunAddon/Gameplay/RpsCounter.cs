using UnityEngine;

namespace NunAddon.Gameplay;

public enum RpsChoice : byte { Rock, Paper, Scissors }

public static class RpsCounter
{
    public static bool Open;
    public static byte Counter;
    public static byte Nun;
    public static RpsChoice Choice;

    public static void Begin(byte counter, byte nun)
    {
        Open = true; Counter = counter; Nun = nun; Choice = RpsChoice.Rock;
    }

    public static void Cycle()
    {
        if (!Open) return;
        Choice = (RpsChoice)(((int)Choice + 1) % 3);
    }

    public static void Submit()
    {
        if (!Open) return;
        var enemy = (RpsChoice)((int)Time.time % 3);
        Open = false;
        if ((Choice == RpsChoice.Rock && enemy == RpsChoice.Scissors) ||
            (Choice == RpsChoice.Paper && enemy == RpsChoice.Rock) ||
            (Choice == RpsChoice.Scissors && enemy == RpsChoice.Paper))
            NunController.Parry();
    }

    public static void Reset()
    {
        Open = false;
        Counter = byte.MaxValue;
        Nun = byte.MaxValue;
        Choice = RpsChoice.Rock;
    }
}
