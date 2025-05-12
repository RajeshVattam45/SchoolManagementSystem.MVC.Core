using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ViewModels
{
    public class BulkAssignViewModel
    {
        public List<Class> Classes { get; set; } = new List<Class> ();
        public List<Subject> Subjects { get; set; } = new List<Subject> ();
        public Dictionary<int, List<int>> AssignedSubjects { get; set; } = new Dictionary<int, List<int>> ();
    }
}
