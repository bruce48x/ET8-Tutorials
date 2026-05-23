namespace ET.Client
{
    public static class MatchingHelper
    {
        public static async ETTask Match(Scene root)
        {
            await root.GetComponent<ClientSenderComponent>().Call(C2G_AgarMatch.Create());
            await EventSystem.Instance.PublishAsync(root, new FindMatching());
        }
    }
}
