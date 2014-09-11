using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class SecurityQuestion
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string userName { get; set; }

        public List<SECURITY_QUESTIONS> Questions { get; set; }
        public int UserId { get; set; }
        
        [Required]
        public int QuestionId1 { get; set;}

        [Required]
        [StringLength(100)]
        public string Answer1 { get; set; }
        
        [Required]
        public int QuestionId2 { get; set; }

        [Required]
        [StringLength(100)]
        public string Answer2 { get; set; }

        [Required]
        public int QuestionId3 { get; set; }

        [Required]
        [StringLength(100)]
        public string Answer3 { get; set; }
    }
}