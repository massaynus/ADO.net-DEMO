namespace ADO.net.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        public int id { get; set; }

        [StringLength(30)]
        public string name { get; set; }

        public DateTime? expirydate { get; set; }
    }
}
