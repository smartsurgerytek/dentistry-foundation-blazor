using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Foundation.Entities
{
    public class ExaminationReport : Entity<Guid>
    {
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public bool Carries18 { get; set; }
        public bool Carries17 { get; set; }
        public bool Carries16 { get; set; }
        public bool Carries15 { get; set; }
        public bool Carries14 { get; set; }
        public bool Carries13 { get; set; }
        public bool Carries12 { get; set; }
        public bool Carries11 { get; set; }
        public bool Carries21 { get; set; }
        public bool Carries22 { get; set; }
        public bool Carries23 { get; set; }
        public bool Carries24 { get; set; }
        public bool Carries25 { get; set; }
        public bool Carries26 { get; set; }
        public bool Carries27 { get; set; }
        public bool Carries28 { get; set; }
        public bool Carries48 { get; set; }
        public bool Carries47 { get; set; }
        public bool Carries46 { get; set; }
        public bool Carries45 { get; set; }
        public bool Carries44 { get; set; }
        public bool Carries43 { get; set; }
        public bool Carries42 { get; set; }
        public bool Carries41 { get; set; }
        public bool Carries31 { get; set; }
        public bool Carries32 { get; set; }
        public bool Carries33 { get; set; }
        public bool Carries34 { get; set; }
        public bool Carries35 { get; set; }
        public bool Carries36 { get; set; }
        public bool Carries37 { get; set; }
        public bool Carries38 { get; set; }

        // PE (int)
        public int? PE18 { get; set; }
        public int? PE17 { get; set; }
        public int? PE16 { get; set; }
        public int? PE15 { get; set; }
        public int? PE14 { get; set; }
        public int? PE13 { get; set; }
        public int? PE12 { get; set; }
        public int? PE11 { get; set; }
        public int? PE21 { get; set; }
        public int? PE22 { get; set; }
        public int? PE23 { get; set; }
        public int? PE24 { get; set; }
        public int? PE25 { get; set; }
        public int? PE26 { get; set; }
        public int? PE27 { get; set; }
        public int? PE28 { get; set; }
        public int? PE48 { get; set; }
        public int? PE47 { get; set; }
        public int? PE46 { get; set; }
        public int? PE45 { get; set; }
        public int? PE44 { get; set; }
        public int? PE43 { get; set; }
        public int? PE42 { get; set; }
        public int? PE41 { get; set; }
        public int? PE31 { get; set; }
        public int? PE32 { get; set; }
        public int? PE33 { get; set; }
        public int? PE34 { get; set; }
        public int? PE35 { get; set; }
        public int? PE36 { get; set; }
        public int? PE37 { get; set; }
        public int? PE38 { get; set; }


        [StringLength(200)]
        public string Desc18 { get; set; }

        [StringLength(200)]
        public string Desc17 { get; set; }

        [StringLength(200)]
        public string Desc16 { get; set; }

        [StringLength(200)]
        public string Desc15 { get; set; }

        [StringLength(200)]
        public string Desc14 { get; set; }

        [StringLength(200)]
        public string Desc13 { get; set; }

        [StringLength(200)]
        public string Desc12 { get; set; }
        [StringLength(200)]
        public string Desc11 { get; set; }

        [StringLength(200)]
        public string Desc21 { get; set; }

        [StringLength(200)]
        public string Desc22 { get; set; }

        [StringLength(200)]
        public string Desc23 { get; set; }

        [StringLength(200)]
        public string Desc24 { get; set; }

        [StringLength(200)]
        public string Desc25 { get; set; }

        [StringLength(200)]
        public string Desc26 { get; set; }

        [StringLength(200)]
        public string Desc27 { get; set; }

        [StringLength(200)]
        public string Desc28 { get; set; }

        [StringLength(200)]
        public string Desc48 { get; set; }

        [StringLength(200)]
        public string Desc47 { get; set; }

        [StringLength(200)]
        public string Desc46 { get; set; }

        [StringLength(200)]
        public string Desc45 { get; set; }

        [StringLength(200)]
        public string Desc44 { get; set; }

        [StringLength(200)]
        public string Desc43 { get; set; }

        [StringLength(200)]
        public string Desc42 { get; set; }

        [StringLength(200)]
        public string Desc41 { get; set; }

        [StringLength(200)]
        public string Desc31 { get; set; }

        [StringLength(200)]
        public string Desc32 { get; set; }

        [StringLength(200)]
        public string Desc33 { get; set; }

        [StringLength(200)]
        public string Desc34 { get; set; }

        [StringLength(200)]
        public string Desc35 { get; set; }

        [StringLength(200)]
        public string Desc36 { get; set; }

        [StringLength(200)]
        public string Desc37 { get; set; }

        [StringLength(200)]
        public string Desc38 { get; set; }
    }
}
