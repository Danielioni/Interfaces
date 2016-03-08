using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    public class addScrip : IMOTInbound
    {
        void IMOTInbound.addScrip()
        {
        }
    }

    public class addPrescriber : IMOTInbound
    {
        void IMOTInbound.addPrescriber()
        {
        }
    }

    interface IMOTInbound
    {
        bool addScrip();

        bool addPrescriber();

        // Add Scrip
        // Add Prescriber
        // Add Patient
        // Add Facility
        // Change Scrip / replace Scrip
        // Change Prescriber
        // Change Patient
        // Change Facility
        // Delete Scrip
        // Delete Prescriber
        // Delete Patient
        // Delete Facility
    }

    interface motOutbound
    {
    }
}
