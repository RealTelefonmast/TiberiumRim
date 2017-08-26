using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TiberiumRim
{
    public static class CheckLists
    {
        public static List<IntVec3> ProtectedCells = new List<IntVec3>();

        public static List<IntVec3> SuppressedCells = new List<IntVec3>();

        public static List<IntVec3> AllowedCells = new List<IntVec3>();

        public static int producerAmt = new int();

    }
}
