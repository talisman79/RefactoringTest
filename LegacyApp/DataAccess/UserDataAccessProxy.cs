using LegacyApp.Models;

namespace LegacyApp.DataAccess
{
    public class UserDataAccessProxy : IUserDataAccess
    {
        public void AddUser(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}