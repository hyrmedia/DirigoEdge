using System;
using System.Linq;
using System.Web.Security;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Membership;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.CustomUtils
{
    public class WebUserUtils
    {
        public WebDataContext Context { get; set; }
        public ILog Log = LogFactory.GetLog(typeof(WebUserUtils));

        public WebUserUtils(WebDataContext context)
        {
            Context = context;
        }

        public void UpdateUser(User user)
        {
            if (String.IsNullOrEmpty(user.UserId.ToString()))
            {
                throw new Exception("There was an error processing your request.");
            }

            var cfrp = new CodeFirstRoleProvider(Context);
            var editUser = Context.Users.FirstOrDefault(x => x.UserId == user.UserId);
            var currentUsername = UserUtils.CurrentMembershipUsername();

            VerifyChangeIsValid(user, editUser, currentUsername);
            UpdateUserValues(user, editUser);

            if (user.Username != currentUsername)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(user.Username, false);
            }

            if (user.Roles == null)
            {
                throw new Exception("There was an error processing your request.");
            }

            // Modify the user roles
            // First delete existing roles
            foreach (var role in editUser.Roles.ToList())
            {
                RemoveUserFromFole(user, role, cfrp);
            }
            
            foreach (var role in user.Roles)
            {
                AssignUserToRole(user, role, cfrp);
            }

            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                Log.Debug(e);
                throw new Exception("There was an error processing your request.");
            }
        }

        public void ChangePassword(Guid userId, String newPassword)
        {
            var userToUpdate = Context.Users.First(x => x.UserId == userId);
            var cfmp = new CodeFirstMembershipProvider();

            cfmp.ChangePassword(userToUpdate.Username, newPassword);
            Context.SaveChanges();
        }

        public void AddNewUser(User user)
        {
            var cfrp = new CodeFirstRoleProvider(Context);
            
            VerfiyNewUserIsValid(user);

            WebSecurity.CreateUserAndAccount(user.Username, user.Password, user.TimeZone, user.Email);
            var newlyAddedUser = Context.Users.First(x => x.Username == user.Username);

            newlyAddedUser.CreateDate = DateTime.UtcNow;
            UpdateUserValues(user, newlyAddedUser);

            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                throw new Exception("There was an error processing your request.");
            }

            // Add the asigned roles
            if (user.Roles == null || !user.Roles.Any())
            {
                return;
            }

            foreach (var role in user.Roles)
            {
                WebUserUtils.AssignUserToRole(user, role, cfrp);
            }
        }

        public void DeleteUser(Guid userId)
        {
            if (userId == null)
            {
                throw new Exception("There was an error processing your request.");
            }
            var userToDelete = Context.Users.FirstOrDefault(x => x.UserId == userId);

            // Make sure user even exists
            if (userToDelete == null)
            {
                throw new Exception("There was an error processing your request.");
            }

            // Clean up Roles First
            foreach (var role in Roles.GetRolesForUser(userToDelete.Username))
            {
                Roles.RemoveUserFromRole(userToDelete.Username, role);
            }
            
            var eventModule = Context.EventAdminModules.Where(x => x.User.UserId == userToDelete.UserId);
            Context.EventAdminModules.RemoveRange(eventModule);

            var blogModule = Context.BlogAdminModules.Where(x => x.User.UserId == userToDelete.UserId);
            Context.BlogAdminModules.RemoveRange(blogModule);

            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                throw new Exception("There was an error processing your request.");
            }

            // Finally Delete From Membership
            WebSecurity.DeleteUser(userToDelete.Username);
        }

        private static void VerfiyNewUserIsValid(User user)
        {
            if (UserUtils.UserExistsByEmail(user.Email))
            {
                throw new Exception("An account already exists for this email address.");
            }

            if (UserUtils.UserExistsByUsername(user.Username))
            {
                throw new Exception("An account already exists for this username.");
            }

            if (String.IsNullOrEmpty(user.Username))
            {
                throw new Exception("There was an error processing your request.");
            }
        }

        public static void UpdateUserValues(User user, User editUser)
        {
            editUser.Username = user.Username;
            editUser.FirstName = user.FirstName;
            editUser.LastName = user.LastName;
            editUser.Email = user.Email;
            editUser.UserImageLocation = user.UserImageLocation;
            editUser.IsApproved = user.IsApproved;
        }

        public static void AssignUserToRole(User user, Role role, CodeFirstRoleProvider cfrp)
        {
            if (!Roles.IsUserInRole(user.Username, role.RoleName))
            {
                Roles.AddUserToRole(user.Username, role.RoleName);
            }
            
            cfrp.AddUsersToRoles(new[] {user.Username}, new[] {role.RoleName});
        }

        public void RemoveUserFromFole(User user, Role role, RoleProvider cfrp)
        {
            var rolesList = user.Roles.Select(a => a.RoleName).ToList();
            var userRoles = Context.Roles.Where(x => rolesList.Contains(role.RoleName)).ToList();
            var foundRole = !userRoles.Contains(role);
            
            if (foundRole)
            {
                Roles.RemoveUserFromRole(user.Username, role.RoleName);
                cfrp.RemoveUsersFromRoles(new[] {user.Username}, new[] {role.RoleName});
            }
        }

        public static void VerifyChangeIsValid(User user, User editUser, String currentUsername)
        {
            if (editUser == null)
            {
                throw new Exception("There was an error processing your request.");
            }

            if (!user.IsApproved && currentUsername == editUser.Username)
            {
                throw new Exception("Current user cannot be deactivated.");
            }

            // Changed email, if email address already taken, return
            if (user.Email != editUser.Email && UserUtils.UserExistsByEmail(user.Email))
            {
                throw new Exception("An account already exists for this email address.");
            }

            // Changed username, if username already taken, return
            if (user.Username != editUser.Username && UserUtils.UserExistsByUsername(user.Username))
            {
                throw new Exception("An account already exists for this username.");
            }
        }
    }
}