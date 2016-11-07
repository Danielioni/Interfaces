using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace motCommonLib
{
    /// <summary>
    /// motRecordSequencedQueue<T>
    /// 
    /// A collection of queues to collect and write collections of motRecords in the proper order
    /// The first queue Write is to a timer event and each queue is emptied in sequence when the 
    /// current queue is exausted.  When the final queue is empty, the timer starts again.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class motSequencecdQueue
    {
        public Dictionary<int, Queue<object>> __d_queues; // Sequence/Queue

        /*
        public Queue<motStoreRecord>        __q_stores;
        public Queue<motLocationRecord>     __q_locations;
        public Queue<motPatientRecord>      __q_patients;
        public Queue<motPrescriberRecord>   __q_prescribers;
        public Queue<motPrescriptionRecord> __q_prescriptions;
        public Queue<motDrugRecord>         __q_drugs;
        public Queue<motTimeQtysRecord>     __q_tqs;
        */

        public Timer __t_timer;

        public motSequencecdQueue()
        {
            /*
            __q_stores = new Queue<motStoreRecord>();
            __q_locations = new Queue<motLocationRecord>();
            __q_patients = new Queue<motPatientRecord>();
            __q_prescribers = new Queue<motPrescriberRecord>();
            __q_prescriptions = new Queue<motPrescriptionRecord>();
            __q_drugs = new Queue<motDrugRecord>();
            __q_tqs = new Queue<motTimeQtysRecord>();
            */

            __d_queues = new Dictionary<int, Queue<object>>();
        }

        public void Write<T>(motSocket __socket)
        {
        }

        public void AddQueue<T>(T __queue)
        {
            Queue<T> __sequence = new Queue<T>();
        }

        public void Add<T>(T __item)
        { }

        public void EmptyQueues()
        {

        }
    }
}
