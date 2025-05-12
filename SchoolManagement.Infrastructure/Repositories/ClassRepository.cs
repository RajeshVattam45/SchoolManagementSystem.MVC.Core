using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;


namespace SchoolManagement.Infrastructure.Repositories
{
    // Concrete implementation of IClassRepository interface for interacting with Class entities in the database.
    public class ClassRepository : IClassRepository
    {
        private readonly SchoolDbContext _context;

        // Constructor that injects the application's database context.
        public ClassRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        // Retrieves all classes from the database.
        public IEnumerable<Class> GetAllClasses ( )
        {
            return _context.Classes.ToList ();
        }

        // Retrieves a single class by its ID.
        public Class GetClassById ( int id )
        {
            return _context.Classes.AsNoTracking ().FirstOrDefault ( c => c.Id == id );
        }

        // Adds a new class to the database.
        public void AddClass ( Class cls )
        {
            _context.Classes.Add ( cls );
            _context.SaveChanges ();
        }

        // Updates an existing class in the database.
        public void UpdateClass ( Class cls )
        {
            // Retrieve the existing class from the database.
            var existingClass = _context.Classes.Find ( cls.Id );
            if (existingClass != null)
            {
                // Overwrite current values with new values from the input object.
                _context.Entry ( existingClass ).CurrentValues.SetValues ( cls );
                _context.SaveChanges ();
            }
        }

        // Deletes a class from the database based on its ID.
        public void DeleteClass ( int id )
        {
            var cls = _context.Classes.Find ( id );
            if (cls != null)
            {
                _context.Classes.Remove ( cls );
                _context.SaveChanges ();
            }
        }
    }
}
