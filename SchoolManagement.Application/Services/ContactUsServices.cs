using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Services
{
    public class ContactUsServices : IContactUsFormServices
    {
        public readonly IContactUsRepository _repo;

        public ContactUsServices ( IContactUsRepository repo )
        {
            _repo = repo;
        }

        public async Task SubmitContactAsync ( ContactRequestDto dto )
        {
            var model = new ContactRequest
            {
                Name = dto.Name,
                Email = dto.Email,
                Message = dto.Message
            };
            await _repo.AddContactRequestAsync ( model );
        }

        public async Task<List<ContactRequest>> GetAllContactsAsync ( )
        {
            return await _repo.GetAllAsync ( );
        }
        
    }
}
