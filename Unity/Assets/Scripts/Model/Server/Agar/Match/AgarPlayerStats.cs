namespace ET.Server.Agar
{
    [ChildOf(typeof(DBComponent))]
    public class AgarPlayerStats : Entity, IAwake<string>
    {
        public string Account { get; set; }
        public long TotalMatches { get; set; }
        public long WinMatches { get; set; }
        public long UpdatedTime { get; set; }
    }
}
