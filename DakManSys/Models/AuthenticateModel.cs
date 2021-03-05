using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DakManSys.Models
{
    public class AuthenticateModel
    {

        //hey appan
        [Required]
        public string Username { set; get; }
        [Required]
        public string Password { set; get; }
    }
}