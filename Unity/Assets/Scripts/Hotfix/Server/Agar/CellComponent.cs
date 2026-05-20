using System.Collections.Generic;

namespace ET.Server.Agar
{
    public class CellComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, Cell> Cells = new();
    }
}