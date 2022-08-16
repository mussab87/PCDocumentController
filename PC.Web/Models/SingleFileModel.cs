using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Web.Models
{
    public class SingleFileModel
    {
        [Required(ErrorMessage = "Please Enter File Name")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "Please Select File")]
        public IFormFile File { get; set; }
    }
}
