using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CustomToDoManager.UserService;
using ToDo.Infrastructure;
using IToDoManager = ToDo.Infrastructure.IToDoManager;

namespace CustomToDoManager
{
    public class ToDoManager : IToDoManager
    {
        private bool isFirst = true;
        private int currentUserId;


        public List<IToDoItem> GetTodoList(int userId)
        {
            List<User> users;
            string json;
            if (isFirst)
            {
                ToDoManagerClient client = new ToDoManagerClient();
                List<CustomToDoItem> list = client.GetTodoList(userId).Select(u =>
                    new CustomToDoItem()
                    {
                        ToDoId = u.ToDoId,
                        IsCompleted = u.IsCompleted,
                        Name = u.Name,
                        UserId = u.UserId
                    }).ToList();

                client.Close();

                json = File.ReadAllText("ItemsFile.json");
                users = new JavaScriptSerializer().Deserialize<List<User>>(json);

                var currentUser = users.First(u => u.Id == userId);

                foreach (var item in list)
                {
                    currentUser.toDoList.Add(new Item(){Id = item.ToDoId, Description = item.Name, IsComplete = item.IsCompleted});
                }
                json = new JavaScriptSerializer().Serialize(users);
                File.WriteAllText("ItemsFile.json", json);

                //var returnedItem = list as List<IToDoItem>;

                return (List<IToDoItem>) list.Cast<IToDoItem>();
            }


            json = File.ReadAllText("ItemsFile.json");
            users = new JavaScriptSerializer().Deserialize<List<User>>(json);
            List<CustomToDoItem> itemsList = users.First(u => u.Id == userId).toDoList.Select(u =>
                new CustomToDoItem()
                {
                    ToDoId = u.Id,
                    IsCompleted = u.IsComplete,
                    Name = u.Description,
                    UserId = userId
                }).ToList();

            return (List<IToDoItem>)itemsList.Cast<IToDoItem>();
        }

        public async void UpdateToDoItem(IToDoItem todo)
        {
            var json = File.ReadAllText("ItemsFile.json");
            var users = new JavaScriptSerializer().Deserialize<List<User>>(json);
            var item = users.First(u => u.Id == todo.UserId).toDoList.First(i => i.Id == todo.ToDoId);
            item.Description = todo.Name;
            item.IsComplete = todo.IsCompleted;

            ToDoManagerClient client = new ToDoManagerClient();
            await client.UpdateToDoItemAsync(new UserService.ToDoItem
            {
                IsCompleted = todo.IsCompleted,
                Name = todo.Name,
                ToDoId = todo.ToDoId,
                UserId = todo.UserId
            });
        }

        public void CreateToDoItem(IToDoItem todo)
        {
            string json = File.ReadAllText("ItemsFile.json");
            List<User> users = new JavaScriptSerializer().Deserialize<List<User>>(json);

            users
                .First(u => u.Id == todo.UserId)
                .toDoList.Add(new Item {Description = todo.Name, Id = todo.ToDoId, IsComplete = todo.IsCompleted});

            json = new JavaScriptSerializer().Serialize(todo);
            File.WriteAllText("ItemsFile.json", json);

            ToDoManagerClient client = new ToDoManagerClient();

            UserService.ToDoItem toDoItem = new UserService.ToDoItem
            {
                IsCompleted = todo.IsCompleted,
                Name = todo.Name,
                ToDoId = todo.ToDoId,
                UserId = todo.UserId
            };
            client.CreateToDoItem(toDoItem);
        }

        public async void DeleteToDoItem(int todoItemId)
        {
            var json = File.ReadAllText("ItemsFile.json");
            var users = new JavaScriptSerializer().Deserialize<List<User>>(json);
            var item = users.First(u => u.Id == currentUserId).toDoList.First(i => i.Id == todoItemId);
            var userItem = users.First(u => u.Id == currentUserId);
            userItem.toDoList.Remove(item);

            ToDoManagerClient client = new ToDoManagerClient();
            await client.DeleteToDoItemAsync(todoItemId);
        }

        //private Task<int> CreateUserAsync(int id)
        //{
        //}

        public int CreateUser(string name)
        {
            ToDoManagerClient client = new ToDoManagerClient();
            int id = client.CreateUser(name);
            currentUserId = id;
            client.Close();

            List<User> users;
            string json;
            if (File.Exists("ItemsFile.json"))
            {
                json = File.ReadAllText("ItemsFile.json");
                users = new JavaScriptSerializer().Deserialize<List<User>>(json);
                User user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    users.Add(new User {Id = id, Name = name});
                }            
            }
            else
            {
                users = new List<User>(){new User{Id = id, Name = name}};
            }

            json = new JavaScriptSerializer().Serialize(users);
            File.WriteAllText("ItemsFile.json", json);

            //ToDoItem[] list = client.GetTodoList(userId);
            //List<User> users = new List<User>();
            //users.Add(new User { Id = userId });
            //users.First().toDoList.

            //isFirst = true;

            //User user = new User {Name = name};          

            ////File.WriteAllText("UserFile.json", json);

            //ToDoManagerClient client = new ToDoManagerClient();
            //int id = client.CreateUser(name);
            //client.Close();

            //user.Id = id;

            //string json = new JavaScriptSerializer().Serialize(user);

            //File.WriteAllText("UserFile.json", json);

            
            return id;
        }
    }
}
