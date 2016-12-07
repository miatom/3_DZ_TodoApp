using NewTodoItem;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoDb
{
    public class TodoDbContext : System.Data.Entity.DbContext
    {
        public IDbSet<TodoItem> TodoItem { get; set; }  // relacija
        public TodoDbContext(string connectionString) : base(connectionString) //connection string, konstruktor
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>().HasKey(s => s.Id); //ključ
            modelBuilder.Entity<TodoItem>().Property(s => s.Text).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(s => s.IsCompleted).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(s => s.DateCompleted); //nije obavezno
            modelBuilder.Entity<TodoItem>().Property(s => s.DateCreated).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(s => s.UserId).IsRequired();
        }
    }
}
