using Maturaball_Tischreservierung.Models;
using Microsoft.EntityFrameworkCore;
using static Maturaball_Tischreservierung.UnitOfWork;

namespace Maturaball_Tischreservierung.Services
{
    public class ServerSettingsService : UsesDatabase
    {
        public ServerSettingsService(ApiConfig apiConfig) : base(apiConfig)
        {
        }

        public async Task<IServiceResult<ServerSettings>> GetServerSettingsAsync()
        {
            await EnsureServerSettingsIntegrityAsync();

            var serverSettings = await _unitOfWork.Query<ServerSettings>().SingleAsync();

            return new ServiceResult<ServerSettings>(serverSettings);
        }

        public async Task<IServiceResult> UpdateServerSettingsAsync(ServerSettings serverSettings)
        {
            await EnsureServerSettingsIntegrityAsync();

            var serverSettingsFromDb = await _unitOfWork.Query<ServerSettings>().SingleAsync();

            serverSettingsFromDb.GreetMessage = serverSettings.GreetMessage;
            serverSettingsFromDb.IsReservationOpen = serverSettings.IsReservationOpen;
            serverSettingsFromDb.IsRegistrationOpen = serverSettings.IsRegistrationOpen;
            serverSettingsFromDb.IsPageOpen = serverSettings.IsPageOpen;
            serverSettingsFromDb.IsInEarlyAccess = serverSettings.IsInEarlyAccess;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ConcurrencyException)
            {
                return new ServiceResult("Concurrency", "ConcurrencyException");
            }
            return new ServiceResult();
        }
        
        /// <summary>
        /// Ensures that the server settings are valid and complete.
        /// Only one db entry should exist.
        /// </summary>
        /// <returns></returns>
        private async Task EnsureServerSettingsIntegrityAsync()
        {
            var serverSettings = await _unitOfWork.Query<ServerSettings>().ToListAsync();
            if (serverSettings.Count == 0)
            {
                await _unitOfWork.InsertAsync(new ServerSettings());
                await _unitOfWork.SaveChangesAsync();
            }
            else if (serverSettings.Count > 1)
            {
                foreach (var item in serverSettings)
                {
                    await _unitOfWork.DeleteAsync(item);
                }
                await _unitOfWork.InsertAsync(new ServerSettings());
            }
        }
    }
}
