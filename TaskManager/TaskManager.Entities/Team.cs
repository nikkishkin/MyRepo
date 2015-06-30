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
    
    public partial class Team
    {
        public Team()
        {
            this.Tasks = new HashSet<Task>();
            this.Members = new HashSet<User>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ManagerId { get; set; }
    
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual User Manager { get; set; }
        public virtual ICollection<User> Members { get; set; }
    }
}