using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
       // private readonly ApplicationDbContext _db;
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        public EmailRepository(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public async Task SendAndLogEmail(PaymentUpdateResultMessage message)
        {
            await using var _db = new ApplicationDbContext(_contextOptions);

            _db.EmailLog.Add(new EmailLog
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"This Order ${message.OrderId} has been payed and confirmed"
            }) ;

            await _db.SaveChangesAsync();
        }
    }
}
