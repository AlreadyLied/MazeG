
public class Def
{
    public const int inventorySize = 4;
}

public enum Layer
{
    Player = 6,
    Wall = 7,
    Enemy = 8,
    PlayerTrigger = 9,
    Floor = 10,
    PlayerInteract = 11,
}

// public class Utils
// {
//     public static int GetLayerMask(params Layer[] layers)
//     {
//         int mask = 0;
//         foreach (Layer layer in layers)
//             mask |= (1 << (int)layer);
//         return mask;
//     }
// }