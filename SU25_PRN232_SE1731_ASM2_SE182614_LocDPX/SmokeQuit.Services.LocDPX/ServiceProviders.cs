using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Services.LocDPX
{
    public class JwtSettings
    {
        public string Secret { get; set; } = "YourSuperSecretJwtKeyThatShouldBeAtLeast32CharactersLong!";
        public string Issuer { get; set; } = "SmokeQuit.GraphQLAPIServices.LocDPX";
        public string Audience { get; set; } = "SmokeQuit.GraphQLClients.BlazorWAS.LocDPX";
        public int ExpirationDays { get; set; } = 7;
    }
    public interface IServiceProviders
    {
        SystemUserAccountService UserAccountService { get; }

        CoachesLocDpxService CoachesService { get; }
        IChatsLocDpxService ChatsService { get; }
    }

    public class ServiceProviders : IServiceProviders
    {
        private SystemUserAccountService _userAccountService;
        private CoachesLocDpxService _coachesService;
        private IChatsLocDpxService _chatsService;

        public ServiceProviders() { }

        public SystemUserAccountService UserAccountService
        {
            get { return _userAccountService ??= new SystemUserAccountService(); }
        }

        public CoachesLocDpxService CoachesService
        {
            get { return _coachesService ??= new CoachesLocDpxService(); }
        }

        public IChatsLocDpxService ChatsService
        {
            get { return _chatsService ??= new ChatsLocDpxService(); }
        }
    }
}