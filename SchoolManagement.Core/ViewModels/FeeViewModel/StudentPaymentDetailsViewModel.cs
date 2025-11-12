using SchoolManagement.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels.FeeViewModel
{
    public class StudentPaymentDetailsViewModel
    {
        public List<FeePaymentDto> Payments { get; set; } = new ();
        public StudentFeeStatusDto Status { get; set; } = new ();
    }
}
