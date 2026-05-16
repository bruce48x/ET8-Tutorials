using System.Collections.Generic;

namespace ET.Game.Agar
{
    public class CellComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, Cell> Cells = new();
    }
}