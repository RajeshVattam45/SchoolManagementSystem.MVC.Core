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
    public class EventService : IEventService
    {
        private readonly IGenericRepository<Events> _eventRepository;

        public EventService ( IGenericRepository<Events> eventRepository )
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<Events>> GetAllEventsAsync ( )
        {
            return await _eventRepository.GetAllAsync ();
        }

        public async Task<Events?> GetEventByIdAsync ( int id )
        {
            return await _eventRepository.GetByIdAsync ( id );
        }

        public async Task AddEventAsync ( Events ev )
        {
            await _eventRepository.AddAsync ( ev );
        }

        public async Task UpdateEventAsync ( Events ev )
        {
            await _eventRepository.UpdateAsync ( ev );
        }

        public async Task DeleteEventAsync ( int id )
        {
            await _eventRepository.DeleteAsync ( id );
        }
    }
}
