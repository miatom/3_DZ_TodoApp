
using Interfaces;
using NewTodoItem;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoDb;

namespace InterfacesSql
{
    public interface ITodoRepository
    {
        TodoItem Get(Guid todoId, Guid userID);
        void Add(TodoItem todoItem);
        bool Remove(Guid todoId, Guid userID);
        void Update(TodoItem todoItem, Guid userID);
        bool MarkAsCompleted(Guid todoId, Guid userID);
        List<TodoItem> GetAll(Guid userID);
        List<TodoItem> GetActive(Guid userID);
        List<TodoItem> GetCompleted(Guid userID);
        List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userID);

    }

    public class TodoSqlRepository : ITodoRepository
    {
        private const string ConnectionString =
            "Server=(localdb)\\mssqllocaldb;Database=TodoDb2;Trusted_Connection=True;MultipleActiveResultSets=true";

        private TodoDbContext _context; // relacijska baza podataka

        public TodoSqlRepository(TodoDbContext context) // konstruktor repozitorija
        {
            _context = context;
        }

        public TodoItem Get(Guid todoId, Guid userID)
        {
            if (userID == null || todoId == null)
            {
                throw new ArgumentException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> rezultat = _context.TodoItem.Where(s => s.Id == todoId && s.UserId == userID).Select(s => s);
                if (rezultat.Count() == 0)
                {
                    throw new TodoAccessDeniedException("User is not the owner of the Todo item");
                }
                return rezultat.SingleOrDefault();
            }
                
        }

        public void Add(TodoItem todoItem)
        {
            if (todoItem == null)
            {
                throw new ArgumentNullException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> _pretraziId = _context.TodoItem.Where(s => s.Id == todoItem.Id).Select(s => s);
                var rez = _pretraziId.FirstOrDefault();

                if (rez != null)
                {
                    throw new DuplicateTodoItemException(" duplicate id: {0}", todoItem.Id);
                }
                _context.TodoItem.Add(todoItem);
                _context.SaveChanges();
            }
                
            
        }

        public bool Remove(Guid todoId, Guid userID)
        {
            if (userID == null || todoId == null)
            {
                throw new ArgumentException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> rezultat = _context.TodoItem.Where(s => s.Id == todoId && s.UserId == userID).Select(s => s);
                if (rezultat.Count() == 0)
                {
                    throw new TodoAccessDeniedException("User is not the owner of the Todo item");
                }
                else
                {
                    _context.TodoItem.Remove(rezultat.First());
                    _context.SaveChanges();
                    return true;
                }

            }

        }

        public void Update(TodoItem todoItem, Guid userID)
        {
            if (userID == null || todoItem==null)
            {
                throw new ArgumentException();
            }

            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> _pretraziId = _context.TodoItem.Where(s => s.Id == todoItem.Id).Select(s => s); //pronalazimo todoItem
                var entitet = _pretraziId.FirstOrDefault();
                if (entitet == null) // ne postoji u bazi
                {
                    _context.TodoItem.Add(todoItem); //dodajemo todoItem u bazu
                                                     
                }
                else // postoji u bazi, provjera prava pristupa korisnika
                {

                    if (todoItem.UserId == userID) //ima  pravo pristupa
                    {
                        entitet.UserId = todoItem.UserId;
                        entitet.DateCreated = todoItem.DateCreated;
                        entitet.DateCompleted = todoItem.DateCompleted;
                        entitet.Id = todoItem.Id;
                        entitet.Text = todoItem.Text;
                        entitet.IsCompleted = todoItem.IsCompleted;

                        //_context.SaveChanges();
                    }
                    else
                    {
                        throw new TodoAccessDeniedException("User is not the owner of the Todo item");
                    }
                }
                _context.SaveChanges();
            }
                
        }

        public bool MarkAsCompleted(Guid todoId, Guid userID)
        {
            if (userID == null || todoId == null)
            {
                throw new ArgumentException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> rezultat = _context.TodoItem.Where(s => s.Id == todoId && s.UserId == userID).Select(s => s);
                var entitet = rezultat.FirstOrDefault();
                if (entitet == null)
                {
                    throw new TodoAccessDeniedException("User is not the owner of the Todo item");

                }
                else
                {
                    entitet.IsCompleted = true;
                    entitet.DateCompleted = DateTime.Now;
                    _context.SaveChanges();
                    return true;
                }
            }
                
        }

        public List<TodoItem> GetAll(Guid userID)
        {
            if (userID==null)
            {
                throw new ArgumentNullException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> _pronadiSve = _context.TodoItem.Where(s => s.UserId == userID).OrderByDescending(s => s.DateCreated).Select(s => s);
                var _rezultat = _pronadiSve.ToList();
                return _rezultat;
            }
               
        }

        public List<TodoItem> GetActive(Guid userID)
        {
            
            if (userID == null)
            {
                throw new ArgumentNullException();
            }
            
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> _pronadiAktivne = _context.TodoItem.Where(s => s.IsCompleted == false && s.UserId == userID).Select(s => s);
                var _rezultat = _pronadiAktivne.ToList();
                return _rezultat;
            }
                
        }

        public List<TodoItem> GetCompleted(Guid userID)
        {
            if (userID == null)
            {
                throw new ArgumentNullException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> _pronadiNeAktivne = _context.TodoItem.Where(s => s.IsCompleted == true && s.UserId == userID).Select(s => s);
                var _rezultat = _pronadiNeAktivne.ToList();
                return _rezultat;
            }
                
        }

        public List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userID)
        {
            if (userID == null || filterFunction==null)
            {
                throw new ArgumentNullException();
            }
            using (_context=new TodoDbContext(ConnectionString))
            {
                IQueryable<TodoItem> _lista = _context.TodoItem.Where(s => filterFunction(s) == true && s.UserId == userID).Select(s => s);
                return _lista.ToList();
            }
                
        }

    }




    public class TodoAccessDeniedException : Exception
    {
        public TodoAccessDeniedException(string message) : base(message)
        {

        }
    }
    public class DuplicateTodoItemException : Exception
    {
        public DuplicateTodoItemException(string message, Guid number) : base(message)
        {
        }
    }
}


    