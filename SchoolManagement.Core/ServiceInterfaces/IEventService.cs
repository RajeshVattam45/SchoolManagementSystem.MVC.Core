using SchoolManagement.Core.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IEventService
    {
        Task<IEnumerable<Events>> GetAllEventsAsync ( );
        Task<Events?> GetEventByIdAsync ( int id );
        Task AddEventAsync ( Events ev );
        Task UpdateEventAsync ( Events ev );
        Task DeleteEventAsync ( int id );
    }
}
