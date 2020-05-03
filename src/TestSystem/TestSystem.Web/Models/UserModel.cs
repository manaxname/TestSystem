using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSystem.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public Guid ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
