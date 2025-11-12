using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class ContactUsForm : IContactUsRepository
    {
        public readonly SchoolDbContext _context;

        public ContactUsForm ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task AddContactRequestAsync ( ContactRequest contact )
        {
            _context.ContactRequests.Add( contact );
            await _context.SaveChangesAsync();
        }

        public Task<List<ContactRequest>> GetAllAsync ( )
        {
           return _context.ContactRequests.OrderByDescending(c => c.SubmittedAt).ToListAsync();
        }
    }
}
