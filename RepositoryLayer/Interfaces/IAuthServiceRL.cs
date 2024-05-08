using RepositoryLayer.Entities;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Register = RepositoryLayer.Entities.Register;

namespace RepositoryLayer.Interfaces
{
    public interface IAuthServiceRL
    {
        public string GenerateJwtToken(Register user);
    }
}
