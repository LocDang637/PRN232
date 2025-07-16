using MassTransit;
using SmokeQuit.Common.Shared.LocDpx;

namespace SmokeQuit.CoachesLocDpx.Microservices.LocDpx.Consumer
{
    public class ChatConsumer : IConsumer<SmokeQuit.BusinessObject.Shared.LocDpx.Models.ChatsLocDpx>
    {
        private readonly ILogger<ChatConsumer> _logger;
        public ChatConsumer(ILogger<ChatConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SmokeQuit.BusinessObject.Shared.LocDpx.Models.ChatsLocDpx> context)
        {

            var data = context.Message;
            if (data != null)
            {
                string messageLog = string.Format("[{0}] RECEIVE data from RabbitMQ.chatQueue: {1}", DateTime.Now.ToString(), Utilities.ConvertObjectToJSONString(data));
                Utilities.WriteLoggerFile(messageLog);
                _logger.LogInformation(messageLog);

            }
        }
    }
}
