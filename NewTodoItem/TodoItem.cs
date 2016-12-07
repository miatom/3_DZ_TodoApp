using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTodoItem
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid UserId { get; set; }

        public TodoItem(string text, Guid userId)
        {
            this.Id = Guid.NewGuid(); // Generates new unique identifier
            this.Text = text;
            this.IsCompleted = false;
            this.DateCreated = DateTime.Now; // Set creation date as current time
            this.UserId = userId;
        }

        public TodoItem()
        {
            // entity framework needs this one
            // not for use :)
        }

    }
}
