using Microsoft.AspNetCore.Identity;

namespace RR.Data
{
    public class SeedAdmin
    {
        private readonly IdentityRole _roles;
        private readonly IdentityUser _users;
        private readonly IdentityUserRole<string> _userRoles;
        public SeedAdmin()
        {
            _roles = GetRoles();
            _users = GetUsers();
            _userRoles = GetUserRoles(_users, _roles);
        }

        public IdentityRole Roles { get { return _roles; } }
        public IdentityUser Users { get { return _users; } }
        public IdentityUserRole<string> UserRoles { get { return _userRoles; } }

        private IdentityRole GetRoles()
        {

            // Seed Roles
            var adminRole = new IdentityRole("Admin");
            adminRole.NormalizedName = adminRole.Name!.ToUpper();

            return adminRole;
        }

        private IdentityUser GetUsers()
        {

            string pwd = "P@ssw0rd";
            var passwordHasher = new PasswordHasher<IdentityUser>();

            // Seed Users
            var adminUser = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true,
            };
            adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
            adminUser.NormalizedEmail = adminUser.Email.ToUpper();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, pwd);

            return adminUser;
        }

        private IdentityUserRole<string> GetUserRoles(IdentityUser users, IdentityRole roles)
        {
            var userRole = new IdentityUserRole<string>
            {
                UserId = users.Id,
                RoleId = roles.Id
            };

            return userRole;
        }
    }
}
