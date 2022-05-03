using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalWebApp
{
    public class MedicineTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public string location { get; set; }
        public string providertype { get; set; }
         public string StoreName {get; set;}
        public string BatchID {get; set;}
    }
}
