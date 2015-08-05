using ToDo.Infrastructure;

namespace CustomToDoManager
{
    class CustomToDoItem : IToDoItem
    {
        public int ToDoId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public string Name { get; set; }
    }
}
