using ModelLayer.RegistrationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Helper
{
    public class KafkaDetailsPublish
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public KafkaDetailsPublish(RegisterUserModel model)
        {
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
        }
    }
}
