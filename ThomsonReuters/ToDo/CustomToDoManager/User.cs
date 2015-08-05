using System.Collections.Generic;

namespace CustomToDoManager
{
    /// <summary>
    /// User class fo file storage
    /// </summary>
    public class User
    {
        public User()
        {
            ToDoList = new List<FileStorageItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FileStorageItem> ToDoList { get; set; }
    }
}
