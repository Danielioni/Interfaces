using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    class HL7Messages
    {
        class Segment
        {
            string __code;
            string __description;
            string __type;
            string __opt;
            int __table;
            int __len;
            string __default;
        }

        class MSH  // Message Header
        {

        }
        class ORC // Common Order
        {
            string __id;  // ORC-1

            struct __placer_order_number // EI, ORC-2
            {
                string __entiry_id;         // ORC-2-1
                string __namespace_id;      // ORC-2-2
                string __universal_id;      // ORC-2-3
                string __universal_id_type; // ORC-2-4
            }

            string __filler_order_number;

        }

        class PID // Patient Identification
        {

        }
        class EVN // Pharmacy Order / Treatment
        {
            string __event_type_code;
            string __recorded_time_date;
            string __date_time_planned_event;
            string __event_reason_code;
            string __operator_id;
            string __event_occured;
            string __event_facility;
        }

        class RAS
        {

        }
    }

    class HL7Parser
    {

    }
}
