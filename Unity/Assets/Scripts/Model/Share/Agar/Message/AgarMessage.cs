using MemoryPack;

namespace ET
{
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

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RoomId = default;
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

    public static class AgarOuter
    {
        public const ushort C2G_AgarMatch = 12002;
        public const ushort G2C_AgarMatch = 12003;
        public const ushort Match2G_AgarMatchSuccess = 12004;
    }

    public static class AgarInner
    {
        public const ushort G2Match_AgarMatch = 22002;
        public const ushort Match2G_AgarMatch = 22003;
    }
}
