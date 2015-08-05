using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomToDoManager
{
    /// <summary>
    /// Item class for file storage
    /// </summary>
    public class FileStorageItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }
}
