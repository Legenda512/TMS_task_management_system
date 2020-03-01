using System.Data.Entity;

namespace TMS_task_management_system
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<Task> Tasks { get; set; }
    }
}
