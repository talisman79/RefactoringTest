using LegacyApp.Models;

namespace LegacyApp.Repositories
{
    public interface IClientRepository
    {
        Client GetById(int id);
    }
}