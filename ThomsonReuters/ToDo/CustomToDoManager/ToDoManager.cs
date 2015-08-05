using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using CustomToDoManager.UserService;
using ToDo.Infrastructure;
using IToDoManager = ToDo.Infrastructure.IToDoManager;

namespace CustomToDoManager
{
    public class ToDoManager : IToDoManager
    {
        private bool _isFirst = true;
        private int _currentUserId;

        public List<IToDoItem> GetTodoList(int userId)
        {
            List<User> users;
            string json;
            if (_isFirst)
            {
                ToDoItem[] items;
                using (var client = new ToDoManagerClient())
                {
                    items = client.GetTodoList(userId);
                }

                json = File.ReadAllText("ItemsFile.json");
                users = new JavaScriptSerializer().Deserialize<List<User>>(json);
                User currentUser = users.First(u => u.Id == userId);

                currentUser.ToDoList.Clear();
                currentUser.ToDoList.AddRange(items.Select(i => new FileStorageItem()
                {
                    Id = i.ToDoId,
                    Description = i.Name,
                    IsComplete = i.IsCompleted
                }));

                json = new JavaScriptSerializer().Serialize(users);
                File.WriteAllText("ItemsFile.json", json);

                _isFirst = false;

                return items.Select(u =>
                    new CustomToDoItem
                    {
                        ToDoId = u.ToDoId,
                        IsCompleted = u.IsCompleted,
                        Name = u.Name,
                        UserId = u.UserId
                    }).ToList<IToDoItem>(); 
            }

            json = File.ReadAllText("ItemsFile.json");
            users = new JavaScriptSerializer().Deserialize<List<User>>(json);

            return users.First(u => u.Id == userId).ToDoList.Select(u =>
                new CustomToDoItem()
                {
                    ToDoId = u.Id,
                    IsCompleted = u.IsComplete,
                    Name = u.Description,
                    UserId = userId
                }).ToList<IToDoItem>();
        }

        public async void UpdateToDoItem(IToDoItem todo)
        {
            string json = File.ReadAllText("ItemsFile.json");
            List<User> users = new JavaScriptSerializer().Deserialize<List<User>>(json);

            FileStorageItem item = users.First(u => u.Id == todo.UserId).ToDoList.First(i => i.Id == todo.ToDoId);
            item.Description = todo.Name;
            item.IsComplete = todo.IsCompleted;

            json = new JavaScriptSerializer().Serialize(users);
            File.WriteAllText("ItemsFile.json", json);

            using (var client = new ToDoManagerClient())
            {
                await client.UpdateToDoItemAsync(new ToDoItem
                {
                    IsCompleted = todo.IsCompleted,
                    Name = todo.Name,
                    ToDoId = todo.ToDoId,
                    UserId = todo.UserId
                });
            }
        }

        public async void CreateToDoItem(IToDoItem todo)
        {
            int id;
            using (var client = new ToDoManagerClient())
            {
                await client.CreateToDoItemAsync(new ToDoItem
                {
                    IsCompleted = todo.IsCompleted,
                    Name = todo.Name,
                    UserId = todo.UserId
                });
                ToDoItem[] items = await client.GetTodoListAsync(_currentUserId);
                id = items.First(i => i.UserId == todo.UserId && i.Name == todo.Name).ToDoId;
            }

            string json = File.ReadAllText("ItemsFile.json");
            List<User> users = new JavaScriptSerializer().Deserialize<List<User>>(json);

            users
                .First(u => u.Id == todo.UserId)
                .ToDoList.Add(new FileStorageItem { Description = todo.Name, Id = id, IsComplete = todo.IsCompleted });

            json = new JavaScriptSerializer().Serialize(users);
            File.WriteAllText("ItemsFile.json", json);
        }

        public async void DeleteToDoItem(int todoItemId)
        {
            var json = File.ReadAllText("ItemsFile.json");
            var users = new JavaScriptSerializer().Deserialize<List<User>>(json);

            List<FileStorageItem> items = users.First(u => u.Id == _currentUserId).ToDoList;
            var item = items.First(i => i.Id == todoItemId);
            items.Remove(item);

            json = new JavaScriptSerializer().Serialize(users);
            File.WriteAllText("ItemsFile.json", json);

            using (var client = new ToDoManagerClient())
            {
                await client.DeleteToDoItemAsync(todoItemId);
            }
        }

        public int CreateUser(string name)
        {
            using (var client = new ToDoManagerClient())
            {
                _currentUserId = client.CreateUser(name);
            }

            List<User> users;
            string json;
            if (File.Exists("ItemsFile.json"))
            {
                json = File.ReadAllText("ItemsFile.json");
                users = new JavaScriptSerializer().Deserialize<List<User>>(json);
                User user = users.FirstOrDefault(u => u.Id == _currentUserId);
                if (user == null)
                {
                    users.Add(new User { Id = _currentUserId, Name = name });
                }            
            }
            else
            {
                users = new List<User> { new User { Id = _currentUserId, Name = name } };
            }

            json = new JavaScriptSerializer().Serialize(users);
            File.WriteAllText("ItemsFile.json", json);

            _isFirst = true;

            return _currentUserId;
        }
    }
}
