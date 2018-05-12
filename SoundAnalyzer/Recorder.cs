using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundAnalyzer
{
    class Recorder
    {
        /// <summary>
        /// DLL methods.
        /// </summary>
        [DllImport("Assignment3.dll", CharSet = CharSet.Auto)]
        public static extern void DlgBox();
        [DllImport("Assignment3.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern IntPtr getRecorded();
        [DllImport("Assignment3.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern uint getSize();
        [DllImport("Assignment3.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern uint SetDataSize(uint size);
    }
}
