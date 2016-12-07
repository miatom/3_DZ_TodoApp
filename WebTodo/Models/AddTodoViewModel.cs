using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebTodo.Models
{
    public class AddTodoViewModel 
    {
        [Required(ErrorMessage ="Minimum 3 characters required"), MinLength(3)]
        public string Text { get; set; }
    }
}
