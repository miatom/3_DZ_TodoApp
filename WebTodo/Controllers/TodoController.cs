using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InterfacesSql;
using Microsoft.AspNetCore.Identity;
using WebTodo.Models;
using NewTodoItem;
using Microsoft.AspNet.Identity;

namespace WebTodo.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;
        
        //private readonly Microsoft.AspNet.Identity.UserManager<ApplicationUser> _userManager;
        // Inject user manager into constructor
        /*
        public TodoController(ITodoRepository repository,
        UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        /*
        public async Task<IActionResult> YourAction()
        {
            // Get currently logged -in user using userManager
            ApplicationUser currentUser = await
            _userManager.GetUserAsync(HttpContext.User);
            return View();
        }
        */
        public TodoController(ITodoRepository repository)
        {
            _repository = repository;
        }

        public Guid createGuidFromUserId(string user)
        {
            string padded = user.ToString().PadLeft(32, '0');
            var userId = new Guid(padded);
            return userId;
        }

        public IActionResult Index2()
        {
            var user = User.Identity.GetUserId();
            if (user == null)
            {
                return RedirectToAction("Sorry");
            }
            var userId = createGuidFromUserId(user);
            List<TodoItem> lista = _repository.GetActive(userId);
            return View(lista);
        }

        public IActionResult Sorry()
        {
            return View();
        }

        public IActionResult AddTodoItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTodoItem(string text)
        {
            try
            {
                var user = User.Identity.GetUserId();
                if (ModelState.IsValid)
                {
                    Guid userId = createGuidFromUserId(user);
                    TodoItem todo = new TodoItem(text, userId);
                    _repository.Add(todo);
                    return RedirectToAction("Index2");
                }
            }
            catch
            {
            }
            return View();
        }

        /* ne koristi se 
        public IActionResult SeeActive()
        {
            List<TodoItem> lista=null;
            if (ModelState.IsValid)
            {
                var user = User.Identity.GetUserId();
                var userId = createGuidFromUserId(user);
                lista = _repository.GetActive(userId);
             }
            return View(lista);
        }
        */
        public IActionResult SeeCompleted()
        {
            string user = User.Identity.GetUserId();
            Guid userId = createGuidFromUserId(user);
            var lista = _repository.GetCompleted(userId);
            return View(lista);
        }

        
        public IActionResult MarkAsCompleted(Guid id)
        {
            string user = User.Identity.GetUserId();
            Guid userId = createGuidFromUserId(user);
            var rezultat = _repository.MarkAsCompleted(id,userId);
            return RedirectToAction("Index2");         
        }

        public IActionResult DeleteTodo(Guid id)
        {
            string user = User.Identity.GetUserId();
            Guid userId = createGuidFromUserId(user);
            var rezultat = _repository.Remove(id, userId);
            return RedirectToAction("Index2");
        }

        //za brisanje sa completed todo liste
        public IActionResult Delete(Guid id)
        {
            string user = User.Identity.GetUserId();
            Guid userId = createGuidFromUserId(user);
            var rezultat = _repository.Remove(id, userId);
            return RedirectToAction("SeeCompleted");
        }
    }
}