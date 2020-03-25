using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace PlayTichu
{
    public static class Functions
    {
        [FunctionName("status")]
        public static IActionResult Status(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            return new OkObjectResult("The service is running!");
        }

        [FunctionName("reset")]
        public static async Task Reset(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reset/{userid}")]HttpRequest req,
            string userid,
            [SignalR(HubName = "tichu")] IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            await signalRGroupActions.AddAsync(
                new SignalRGroupAction
                {
                    UserId = userid,
                    Action = GroupAction.RemoveAll
                });
        }

        [FunctionName("pushState")]
        public static Task PushState(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "game/{gameid}/state")] object message,
            string gameid,
            [SignalR(HubName = "tichu")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newState",
                    GroupName = gameid,
                    Arguments = new[] { message }
                });
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "tichu", UserId = "{query.userid}")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("join")]
        public static async Task AddToGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "game/{gameid}/join")]HttpRequest req,
            string gameid,
            [SignalR(HubName = "tichu")] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            [SignalR(HubName = "tichu")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            string userid = req.Query["userid"];

            try
            {
                await signalRGroupActions.AddAsync(
                    new SignalRGroupAction
                    {
                        UserId = userid,
                        GroupName = gameid,
                        Action = GroupAction.Add
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "requestState",
                        GroupName = gameid,
                        Arguments = new[] { new { message = "Give me the state" } }
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [FunctionName("userPing")]
        public static Task DirectMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/{userid}")] object message,
            string userid,
            [SignalR(HubName = "tichu")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "ping",
                    UserId = userid,
                    Arguments = new[] { message }
                });
        }

        [FunctionName("groupPing")]
        public static Task GroupMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "group/{groupid}")] object message,
            string groupid,
            [SignalR(HubName = "tichu")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "ping",
                    GroupName = groupid,
                    Arguments = new[] { message }
                });
        }
    }
}
