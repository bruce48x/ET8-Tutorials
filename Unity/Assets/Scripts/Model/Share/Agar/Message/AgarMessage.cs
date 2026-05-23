using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(AgarOuter.AgarCellInfo)]
    public partial class AgarCellInfo : MessageObject
    {
        public static AgarCellInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(AgarCellInfo), isFromPool) as AgarCellInfo;
        }

        [MemoryPackOrder(0)]
        public long CellId { get; set; }

        [MemoryPackOrder(1)]
        public long OwnerPlayerId { get; set; }

        [MemoryPackOrder(2)]
        public float X { get; set; }

        [MemoryPackOrder(3)]
        public float Y { get; set; }

        [MemoryPackOrder(4)]
        public float Radius { get; set; }

        [MemoryPackOrder(5)]
        public float Mass { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.CellId = default;
            this.OwnerPlayerId = default;
            this.X = default;
            this.Y = default;
            this.Radius = default;
            this.Mass = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.AgarPlayerScoreInfo)]
    public partial class AgarPlayerScoreInfo : MessageObject
    {
        public static AgarPlayerScoreInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(AgarPlayerScoreInfo), isFromPool) as AgarPlayerScoreInfo;
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }

        [MemoryPackOrder(1)]
        public float Score { get; set; }

        [MemoryPackOrder(2)]
        public bool Alive { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.PlayerId = default;
            this.Score = default;
            this.Alive = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.C2G_AgarMatch)]
    [ResponseType(nameof(G2C_AgarMatch))]
    public partial class C2G_AgarMatch : MessageObject, ISessionRequest
    {
        public static C2G_AgarMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(C2G_AgarMatch), isFromPool) as C2G_AgarMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.G2C_AgarMatch)]
    public partial class G2C_AgarMatch : MessageObject, ISessionResponse
    {
        public static G2C_AgarMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2C_AgarMatch), isFromPool) as G2C_AgarMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.Match2G_AgarMatchSuccess)]
    public partial class Match2G_AgarMatchSuccess : MessageObject, IMessage
    {
        public static Match2G_AgarMatchSuccess Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_AgarMatchSuccess), isFromPool) as Match2G_AgarMatchSuccess;
        }

        [MemoryPackOrder(0)]
        public long RoomId { get; set; }

        [MemoryPackOrder(1)]
        public long EndTime { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RoomId = default;
            this.EndTime = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.Match2G_AgarBattleState)]
    public partial class Match2G_AgarBattleState : MessageObject, IMessage
    {
        public static Match2G_AgarBattleState Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_AgarBattleState), isFromPool) as Match2G_AgarBattleState;
        }

        [MemoryPackOrder(0)]
        public long RoomId { get; set; }

        [MemoryPackOrder(1)]
        public long RemainingTime { get; set; }

        [MemoryPackOrder(2)]
        public long MyPlayerId { get; set; }

        [MemoryPackOrder(3)]
        public float MyScore { get; set; }

        [MemoryPackOrder(4)]
        public List<AgarCellInfo> Cells { get; set; } = new();

        [MemoryPackOrder(5)]
        public List<AgarPlayerScoreInfo> Rankings { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RoomId = default;
            this.RemainingTime = default;
            this.MyPlayerId = default;
            this.MyScore = default;
            this.Cells.Clear();
            this.Rankings.Clear();
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.Match2G_AgarBattleResult)]
    public partial class Match2G_AgarBattleResult : MessageObject, IMessage
    {
        public static Match2G_AgarBattleResult Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_AgarBattleResult), isFromPool) as Match2G_AgarBattleResult;
        }

        [MemoryPackOrder(0)]
        public long RoomId { get; set; }

        [MemoryPackOrder(1)]
        public long WinnerPlayerId { get; set; }

        [MemoryPackOrder(2)]
        public float WinnerScore { get; set; }

        [MemoryPackOrder(3)]
        public long MyPlayerId { get; set; }

        [MemoryPackOrder(4)]
        public float MyScore { get; set; }

        [MemoryPackOrder(5)]
        public bool IsWinner { get; set; }

        [MemoryPackOrder(6)]
        public List<AgarPlayerScoreInfo> Rankings { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RoomId = default;
            this.WinnerPlayerId = default;
            this.WinnerScore = default;
            this.MyPlayerId = default;
            this.MyScore = default;
            this.IsWinner = default;
            this.Rankings.Clear();
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarOuter.C2G_AgarMove)]
    public partial class C2G_AgarMove : MessageObject, ISessionMessage
    {
        public static C2G_AgarMove Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(C2G_AgarMove), isFromPool) as C2G_AgarMove;
        }

        [MemoryPackOrder(0)]
        public float DirectionX { get; set; }

        [MemoryPackOrder(1)]
        public float DirectionY { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.DirectionX = default;
            this.DirectionY = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarInner.G2Match_AgarMatch)]
    [ResponseType(nameof(Match2G_AgarMatch))]
    public partial class G2Match_AgarMatch : MessageObject, IRequest
    {
        public static G2Match_AgarMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_AgarMatch), isFromPool) as G2Match_AgarMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.PlayerId = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarInner.Match2G_AgarMatch)]
    public partial class Match2G_AgarMatch : MessageObject, IResponse
    {
        public static Match2G_AgarMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_AgarMatch), isFromPool) as Match2G_AgarMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(AgarInner.G2Match_AgarMove)]
    public partial class G2Match_AgarMove : MessageObject, IMessage
    {
        public static G2Match_AgarMove Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_AgarMove), isFromPool) as G2Match_AgarMove;
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }

        [MemoryPackOrder(1)]
        public float DirectionX { get; set; }

        [MemoryPackOrder(2)]
        public float DirectionY { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.PlayerId = default;
            this.DirectionX = default;
            this.DirectionY = default;
            ObjectPool.Instance.Recycle(this);
        }
    }

    public static class AgarOuter
    {
        public const ushort C2G_AgarMatch = 12002;
        public const ushort G2C_AgarMatch = 12003;
        public const ushort Match2G_AgarMatchSuccess = 12004;
        public const ushort AgarCellInfo = 12005;
        public const ushort AgarPlayerScoreInfo = 12006;
        public const ushort Match2G_AgarBattleState = 12007;
        public const ushort Match2G_AgarBattleResult = 12008;
        public const ushort C2G_AgarMove = 12009;
    }

    public static class AgarInner
    {
        public const ushort G2Match_AgarMatch = 22002;
        public const ushort Match2G_AgarMatch = 22003;
        public const ushort G2Match_AgarMove = 22004;
    }
}
