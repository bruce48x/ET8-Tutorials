namespace ET.Server
{
    [EntitySystemOf(typeof(SessionPlayerComponent))]
    public static partial class SessionPlayerComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this SessionPlayerComponent self)
        {
        }

        [EntitySystem]
        private static void Awake(this SessionPlayerComponent self)
        {

        }
    }
}
