using IdentityModel;
using Mango.Services.Identity.DbContext;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mango.Services.Identity.Initialize
{
    public class DbInitializer : IDbInitializer
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }
                ApplicationUser admin = new ApplicationUser
                {
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = "34567890",
                    UserName = "admin@gmail.com"
                };
                _userManager.CreateAsync(admin, "admin123").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(admin, SD.Admin).GetAwaiter().GetResult();
            var temp1 = _userManager.AddClaimsAsync(admin, new Claim[] {
                    new Claim(JwtClaimTypes.Name, admin.FirstName + " " + admin.LastName),
                    new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                    new Claim(JwtClaimTypes.Role, SD.Admin)
                }).Result;


            ApplicationUser customer = new ApplicationUser
                {
                    Email = "customer@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Customer",
                    LastName = "Customer",
                    PhoneNumber = "34567890",
                    UserName = "customer@gmail.com"
                };
                _userManager.CreateAsync(customer, "customer123").GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(customer, SD.Customer).GetAwaiter().GetResult();

                var temp2 = _userManager.AddClaimsAsync(customer, new Claim[] {
                    new Claim(JwtClaimTypes.Name, customer.FirstName + " " + customer.LastName),
                    new Claim(JwtClaimTypes.GivenName, customer.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, customer.LastName),
                    new Claim(JwtClaimTypes.Role, SD.Customer)
                }).Result;
            
        }
    }
 }
 

