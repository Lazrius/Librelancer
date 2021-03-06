﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibreLancer.Shaders
{
    using System;
    
    public class Polyline
    {
        private static byte[] vertex_bytes = new byte[185] {
27, 75, 1, 128, 28, 7, 118, 99, 215, 129, 150, 179, 69, 160, 8, 3, 188, 84, 35, 105, 6, 204, 171, 175, 164, 139, 147, 154, 156, 231,
70, 61, 216, 10, 9, 46, 123, 215, 91, 40, 246, 16, 251, 255, 157, 237, 163, 208, 37, 193, 166, 41, 141, 102, 74, 83, 146, 36, 68, 186,
77, 208, 163, 232, 232, 54, 179, 45, 12, 162, 51, 143, 194, 21, 69, 144, 100, 65, 85, 167, 177, 227, 183, 72, 152, 240, 1, 201, 11, 223,
177, 87, 240, 29, 159, 239, 241, 95, 123, 7, 109, 0, 84, 6, 97, 43, 200, 168, 245, 253, 121, 142, 146, 20, 183, 207, 187, 152, 34, 231,
133, 102, 3, 213, 123, 65, 228, 34, 214, 159, 237, 91, 253, 171, 216, 119, 73, 58, 94, 89, 229, 1, 125, 125, 114, 163, 92, 253, 116, 191,
189, 96, 184, 38, 208, 209, 103, 36, 246, 39, 11, 105, 34, 165, 105, 62, 234, 215, 15, 53, 20, 109, 14, 221, 6, 6, 137, 33, 156, 43,
12, 238, 116, 204, 21
};
        private static byte[] fragment_bytes = new byte[138] {
27, 193, 0, 0, 140, 212, 70, 117, 72, 126, 139, 145, 133, 117, 229, 91, 73, 139, 39, 53, 8, 95, 33, 218, 236, 192, 2, 133, 43, 106,
81, 216, 90, 214, 214, 146, 23, 125, 215, 135, 215, 27, 230, 16, 110, 107, 37, 137, 88, 27, 255, 24, 182, 78, 84, 4, 101, 63, 238, 98,
199, 15, 90, 56, 108, 79, 75, 148, 224, 53, 146, 41, 188, 221, 100, 31, 31, 203, 143, 168, 193, 246, 132, 175, 229, 92, 198, 209, 242, 6,
214, 120, 24, 25, 169, 227, 157, 15, 231, 91, 228, 249, 249, 208, 14, 111, 211, 181, 227, 156, 163, 5, 37, 179, 67, 193, 31, 8, 134, 163,
81, 101, 31, 207, 219, 146, 172, 118, 177, 82, 20, 210, 0, 179, 177, 94, 1, 8
};
        static ShaderVariables[] variants;
        private static bool iscompiled = false;
        private static int GetIndex(ShaderFeatures features)
        {
            ShaderFeatures masked = (features & ((ShaderFeatures)(0)));
            return 0;
        }
        public static ShaderVariables Get(ShaderFeatures features)
        {
            Compile();
            return variants[GetIndex(features)];
        }
        public static ShaderVariables Get()
        {
            Compile();
            return variants[0];
        }
        public static void Compile()
        {
            if (iscompiled)
            {
                return;
            }
            iscompiled = true;
            ShaderVariables.Log("Compiling Polyline");
            string vertsrc;
            string fragsrc;
            vertsrc = ShCompHelper.FromArray(vertex_bytes);
            fragsrc = ShCompHelper.FromArray(fragment_bytes);
            variants = new ShaderVariables[1];
            variants[0] = ShaderVariables.Compile(vertsrc, fragsrc, "");
        }
    }
}
