using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrgDAL
{
    [Table("Department")]
    public class Department
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Did { get; set; }
        public string DName { get; set; }
        public string Description { get; set; }
    }
}
