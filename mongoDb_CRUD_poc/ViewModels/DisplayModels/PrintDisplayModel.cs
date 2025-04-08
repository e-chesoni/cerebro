﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDb_CRUD_poc.ViewModels.DisplayModels;
public class PrintDisplayModel
{
    public string id { get; set; }
    public string directoryPath { get; set; }
    public string duration { get; set; }
    public string startTimeLocal { get; set; }
    public string endTimeLocal { get; set; }
    public int totalSlices { get; set; }
    public bool complete { get; set; }

}

