using Webhooks.Api.Models;

namespace Webhooks.Api.Repositories
{
    public class InMemoryStudentRepository
    {
        List<Student> _students = [];
        public InMemoryStudentRepository() { }

        public void Add(Student student) { _students.Add(student); }
        public void Remove(Student student) { _students.Remove(student); }
        public List<Student> GetAll() { return _students; }
    }
}
