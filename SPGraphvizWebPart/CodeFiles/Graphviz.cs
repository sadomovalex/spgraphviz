#region Copyright(c) Alexey Sadomov. All Rights Reserved.
// -----------------------------------------------------------------------------
// Copyright(c) 2010 Alexey Sadomov. All Rights Reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//   1. No Trademark License - Microsoft Public License (Ms-PL) does not grant you rights to use
//      authors names, logos, or trademarks.
//   2. If you distribute any portion of the software, you must retain all copyright,
//      patent, trademark, and attribution notices that are present in the software.
//   3. If you distribute any portion of the software in source code form, you may do
//      so only under this license by including a complete copy of Microsoft Public License (Ms-PL)
//      with your distribution. If you distribute any portion of the software in compiled
//      or object code form, you may only do so under a license that complies with
//      Microsoft Public License (Ms-PL).
//   4. The names of the authors may not be used to endorse or promote products
//      derived from this software without specific prior written permission.
//
// The software is licensed "as-is." You bear the risk of using it. The authors
// give no express warranties, guarantees or conditions. You may have additional consumer
// rights under your local laws which this license cannot change. To the extent permitted
// under your local laws, the authors exclude the implied warranties of merchantability,
// fitness for a particular purpose and non-infringement.
// -----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SPGraphvizWebPart
{
    public static class Graphviz
    {
        private const string LIB_GVC = "gvc.dll";
        private const string LIB_GRAPH = "graph.dll";
        private const int SUCCESS = 0;

        [DllImport(LIB_GVC)]
        private static extern IntPtr gvContext();

        [DllImport(LIB_GVC)]
        private static extern int gvFreeContext(IntPtr gvc);

        [DllImport(LIB_GRAPH)]
        private static extern IntPtr agmemread(string data);

        [DllImport(LIB_GRAPH)]
        private static extern void agclose(IntPtr g);

        [DllImport(LIB_GVC)]
        private static extern int gvLayout(IntPtr gvc, IntPtr g, string engine);

        [DllImport(LIB_GVC)]
        private static extern int gvFreeLayout(IntPtr gvc, IntPtr g);

        [DllImport(LIB_GVC)]
        private static extern int gvRenderFilename(IntPtr gvc, IntPtr g,
            string format, string fileName);

        [DllImport(LIB_GVC)]
        private static extern int gvRenderData(IntPtr gvc, IntPtr g,
            string format, out IntPtr result, out int length);

        public static byte[] RenderImage(string source, string layout, string format)
        {
            IntPtr gvc = IntPtr.Zero;
            IntPtr g = IntPtr.Zero;
            try
            {
                // Create a Graphviz context 
                gvc = gvContext();
                if (gvc == IntPtr.Zero)
                    throw new Exception("Failed to create Graphviz context.");

                // Load the DOT data into a graph 
                g = agmemread(source);
                if (g == IntPtr.Zero)
                    throw new Exception("Failed to create graph from source. Check for syntax errors.");

                // Apply a layout 
                if (gvLayout(gvc, g, layout) != SUCCESS)
                    throw new Exception("Layout failed.");

                IntPtr result;
                int length;

                // Render the graph 
                if (gvRenderData(gvc, g, format, out result, out length) != SUCCESS)
                    throw new Exception("Render failed.");

                // Create an array to hold the rendered graph
                byte[] bytes = new byte[length];
                // Copy the image from the IntPtr 
                Marshal.Copy(result, bytes, 0, length);
                return bytes;
            }
            finally
            {
                // Free up the resources 
                gvFreeLayout(gvc, g);
                agclose(g);
                gvFreeContext(gvc);
            }
        }
    }
}
