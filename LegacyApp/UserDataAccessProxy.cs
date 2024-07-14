using LegacyApp;

public class UserDataAccessProxy : IUserDataAccessProxy
{
    public void AddUser(User user)
    {
        UserDataAccess.AddUser(user);
    }
}
