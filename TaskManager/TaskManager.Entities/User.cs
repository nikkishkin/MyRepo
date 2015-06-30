//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskManager.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.TasksForWorker = new HashSet<Task>();
            this.Comments = new HashSet<Comment>();
            this.TeamsForManager = new HashSet<Team>();
        }
    
        public int Id { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public Nullable<bool> IsManager { get; set; }
        public Nullable<int> TeamId { get; set; }
    
        public virtual ICollection<Task> TasksForWorker { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Team> TeamsForManager { get; set; }
        public virtual Team TeamForMember { get; set; }
    }
}