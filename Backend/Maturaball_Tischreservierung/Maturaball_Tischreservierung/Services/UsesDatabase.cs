using Maturaball_Tischreservierung.Models;

namespace Maturaball_Tischreservierung.Services
{
    public class UsesDatabase
    {
        protected readonly UnitOfWork _unitOfWork;
        protected readonly ApiConfig _apiConfig;

        public UsesDatabase(ApiConfig apiConfig)
        {
            _unitOfWork = new UnitOfWork(apiConfig);
            _apiConfig = apiConfig;
        }
    }
}
