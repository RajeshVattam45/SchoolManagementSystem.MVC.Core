using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IContactUsFormServices
    {
        Task SubmitContactAsync ( ContactRequestDto dto );
        Task<List<ContactRequest>> GetAllContactsAsync ( );
    }
}
