using Maturaball_Tischreservierung.Models;
using Microsoft.EntityFrameworkCore;
using static Maturaball_Tischreservierung.UnitOfWork;

namespace Maturaball_Tischreservierung.Services
{
    public class ClientService : UsesDatabase
    {
        private readonly EmailDeliveryService _emailDeliveryService;
        private readonly RandomKeyService _randomKeyService;

        public ClientService(ApiConfig apiConfig, EmailDeliveryService emailDeliveryService, RandomKeyService randomKeyService) : base(apiConfig)
        {
            _emailDeliveryService = emailDeliveryService;
            _randomKeyService = randomKeyService;
        }

        public async Task<IServiceResult<ClientSession>> CreateClientSession(Guid clientId)
        {
            var clients = await _unitOfWork.Query<Client>().Where(c => c.Id == clientId).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<ClientSession>("Id", "Client not found");

            var client = clients[0];

            var clientSession = new ClientSession()
            {
                ClientId = client.Id,
            };

            await _unitOfWork.InsertAsync(clientSession);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<ClientSession>(clientSession);
        }

        public async Task<IServiceResult<ClientSession>> GetClientSession(Guid clientSessionId)
        {
            var clientSessions = await _unitOfWork.Query<ClientSession>().Where(c => c.Id == clientSessionId).ToListAsync();

            if (clientSessions.Count != 1)
                return new ServiceResult<ClientSession>("Id", "ClientSession not found");

            return new ServiceResult<ClientSession>(clientSessions[0]);
        }

        public async Task<IServiceResult> DeleteClientSession(Guid clientSessionId)
        {
            var clientSessions = await _unitOfWork.Query<ClientSession>().Where(c => c.Id == clientSessionId).ToListAsync();

            if (clientSessions.Count != 1)
                return new ServiceResult<ClientSession>("Id", "ClientSession not found");

            await _unitOfWork.DeleteAsync(clientSessions[0]);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult();
        }

        public async Task<IServiceResult<Client>> GetClient(Guid clientId)
        {
            var clients = await _unitOfWork.Query<Client>().Where(c => c.Id == clientId).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<Client>("Id", "Client not found");

            return new ServiceResult<Client>(clients[0]);
        }

        public async Task<IServiceResult<Client>> GetClient(string email)
        {
            var clients = await _unitOfWork.Query<Client>().Where(c => c.Email == email).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<Client>("Email", "Client not found");

            return new ServiceResult<Client>(clients[0]);
        }

        public async Task<IServiceResult<Client>> CreateClient(Client client)
        {
            var clients = await _unitOfWork.Query<Client>().Where(c => c.Email == client.Email).ToListAsync();

            if (clients.Count != 0)
                return new ServiceResult<Client>("Email", "Client already exists");

            await _unitOfWork.InsertAsync(client);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<Client>(client);
        }

        public async Task<IServiceResult<Client>> UpdateClient(Client client)
        {
            var clients = await _unitOfWork.Query<Client>().Where(c => c.Id == client.Id).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<Client>("Id", "Client not found");

            var clientFromDb = clients[0];

            clientFromDb.Name = client.Name;
            clientFromDb.Email = client.Email;
            clientFromDb.Class = client.Class;
            clientFromDb.EmailVerified = client.EmailVerified;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ConcurrencyException)
            {
                return new ServiceResult<Client>("Concurrency", "ConcurrencyException");
            }

            return new ServiceResult<Client>(clientFromDb);
        }

        public async Task<IServiceResult> DeleteClient(Guid clientId)
        {
            var clients = await _unitOfWork.Query<Client>().Where(c => c.Id == clientId).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<Client>("Id", "Client not found");

            await _unitOfWork.DeleteAsync(clients[0]);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult();
        }

        public async Task<IServiceResult> RequestAccountAccess(string client)
        {
            // send email to users email with a newly create account access code

            var clients = await _unitOfWork.Query<Client>().Where(c => c.Email == client).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<Client>("Email", "Client not found");

            var clientFromDb = clients[0];

            if (!clientFromDb.EmailVerified)
                return new ServiceResult<Client>("Email", "Email not verified yet, check your inbox");

            var accountAccessCode = _randomKeyService.GetRandomKey(64);

            ClientAccessCode clientAccessCode = new ClientAccessCode()
            {
                ClientId = clientFromDb.Id,
                AccessCode = accountAccessCode,
                ExpiresAt = DateTime.Now.AddMinutes(5)
            };

            await _unitOfWork.InsertAsync(clientAccessCode);

            await _unitOfWork.SaveChangesAsync();

            await _emailDeliveryService.SendEmailAsync(clientFromDb.Email,
                "Account Access Code", 
                $"Hello!" +
                $"You requested access to your Maturaball account." +
                $"If you did not request anything, or did not excpect this email," +
                $"ignore it please." +
                $"Your access code is {accountAccessCode}");

            return new ServiceResult();
        }

        public async Task<IServiceResult<(Client, Guid)>> UseAccessCode(string code)
        {
            var clientAccessCodes = await _unitOfWork.Query<ClientAccessCode>().Where(c => c.AccessCode == code).ToListAsync();

            if (clientAccessCodes.Count != 1)
                return new ServiceResult<(Client, Guid)>("Code", "Code not found");

            var clientAccessCode = clientAccessCodes[0];

            if (clientAccessCode.ExpiresAt < DateTime.Now)
                return new ServiceResult<(Client, Guid)>("Code", "Code expired");

            if (clientAccessCode.UsedAt != null)
                return new ServiceResult<(Client, Guid)>("Code", "Code has already been redeemed");

            var clients = await _unitOfWork.Query<Client>().Where(c => c.Id == clientAccessCode.ClientId).ToListAsync();

            if (clients.Count != 1)
                return new ServiceResult<(Client, Guid)>("Id", "Client not found");

            var client = clients[0];

            // if client is not verified, verify it
            client.EmailVerified = true;

            var clientSessionResult = await CreateClientSession(client.Id);

            if (clientSessionResult.Status != InternalStatusCode.Success)
                return new ServiceResult<(Client, Guid)>("ClientSession", "ClientSession creation failed");

            clientAccessCode.UsedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<(Client, Guid)>((client, clientSessionResult.Value!.Id));
        }
    }
}
