using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using SmokeQuit.BusinessObject.Shared.LocDpx.Models;
using SmokeQuit.Common.Shared.LocDpx;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmokeQuit.ChatsLocDpx.Microservices.LocDpx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsLocDpxController : ControllerBase
    {
        private readonly ILogger<ChatsLocDpxController> _logger;
        private readonly IBus _bus;
        public ChatsLocDpxController(ILogger<ChatsLocDpxController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        private List<SmokeQuit.BusinessObject.Shared.LocDpx.Models.ChatsLocDpx> Chat;
        // GET: api/<ChatsLocDpxController>
        [HttpGet]
        public IEnumerable<SmokeQuit.BusinessObject.Shared.LocDpx.Models.ChatsLocDpx> Get()
        {
            return Chat;
        }

        // GET api/<ChatsLocDpxController>/5
        [HttpGet("{id}")]
        public SmokeQuit.BusinessObject.Shared.LocDpx.Models.ChatsLocDpx Get(int id)
        {
            return Chat.Find(x => x.ChatsLocDpxid == id);
        }

        // POST api/<ChatsLocDpxController>
        [HttpPost]
        public async Task<IActionResult> Post(SmokeQuit.BusinessObject.Shared.LocDpx.Models.ChatsLocDpx input)
        {
            if (input != null)
            {


                Uri uri = new Uri("rabbitmq://localhost/chatQueue");

                var endPoint = await _bus.GetSendEndpoint(uri);

                await endPoint.Send(input);

                string messageLog = string.Format("[{0}] PUBLISH data to RabbitMQ.chatQueue: {1}", DateTime.Now, Utilities.ConvertObjectToJSONString(input));


                Utilities.WriteLoggerFile(messageLog);


                _logger.LogInformation(messageLog);




                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<ChatsLocDpxController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ChatsLocDpxController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
