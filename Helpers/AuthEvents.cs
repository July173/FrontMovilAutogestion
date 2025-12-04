using System;

namespace AutogestionSenaMaui.Helpers
{
    public static class AuthEvents
    {
        // Simple event to notify that a user logged in
        public static event EventHandler<UserLoggedInEventArgs>? UserLoggedIn;
        
        // Simple event to notify that a user logged out
        public static event EventHandler? UserLoggedOut;

        public static void NotifyUserLoggedIn(int roleId, string firstName, string accessToken)
        {
            UserLoggedIn?.Invoke(null, new UserLoggedInEventArgs
            {
                RoleId = roleId,
                FirstName = firstName,
                AccessToken = accessToken
            });
        }

        public static void NotifyUserLoggedOut()
        {
            UserLoggedOut?.Invoke(null, EventArgs.Empty);
        }
    }

    public class UserLoggedInEventArgs : EventArgs
    {
        public int RoleId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
