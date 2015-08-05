using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomToDoManager
{
    public class User
    {
        public User()
        {
            toDoList = new List<Item>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Item> toDoList { get; set; }

    }

    public class Item
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }

}
