using SchoolManagement.Core.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IContactUsRepository
    {
        Task AddContactRequestAsync ( ContactRequest contact );
        Task<List<ContactRequest>> GetAllAsync ( );
    }
    public class ContactRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
