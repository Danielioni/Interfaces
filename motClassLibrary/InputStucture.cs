using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace motInboundLib
{
    public enum InputStucture
    {
        __inputDelimted = 0,
        __inputTagged,
        __inputXML,
        __inputJSON,
        __inputUndefined
    }

    public enum Action
    {
        __Add = 0,
        __Change,
        __Delete
    }

    public enum RecordType
    {
        Prescriber = 0,
        Prescription,
        Drug,
        Facility,
        Store,
        TimeQty
    }
}